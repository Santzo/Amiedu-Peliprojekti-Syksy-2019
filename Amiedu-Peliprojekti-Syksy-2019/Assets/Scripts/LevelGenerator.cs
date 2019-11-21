using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    Tilemap backgroundTilemap;
    Tilemap backgroundCorners;
    Tilemap foregroundTilemap;
    Tilemap floorTilemap;
    Tilemap foregroundCorners;
    Tile[] cellarTiles;
    GameObject[] floorObjects;
    Tile black;
    int numberOfRooms;
    int worldSizeX, worldSizeY;
    int worldStartX, worldStartY, worldEndX, worldEndY;
    int maxRoomSizeX, maxRoomSizeY;
    int maxAttempts = 20;
    float pixelsPerUnit = 1f;
    public List<AllRooms> allRooms = new List<AllRooms>();
    public RoomGrid[,] roomGrid;

    private void Awake()
    {
        numberOfRooms = 30;
        cellarTiles = Resources.LoadAll<Tile>("Cellar/Tiles");
        black = Resources.Load<Tile>("Cellar/Tiles/Black");
        floorObjects = Resources.LoadAll<GameObject>("FloorObjects");
        backgroundTilemap = GameObject.Find("BackgroundTilemap").GetComponent<Tilemap>();
        backgroundCorners = GameObject.Find("BackgroundCorners").GetComponent<Tilemap>();
        foregroundTilemap = GameObject.Find("ForegroundTilemap").GetComponent<Tilemap>();
        foregroundCorners = GameObject.Find("ForegroundCorners").GetComponent<Tilemap>();
        floorTilemap = GameObject.Find("FloorTilemap").GetComponent<Tilemap>();
        worldSizeX = worldSizeY = 120;
        backgroundTilemap.size = foregroundTilemap.size = foregroundCorners.size = backgroundCorners.size = floorTilemap.size = new Vector3Int(worldSizeX, worldSizeY, 0);
        
        worldStartX = 0 - worldSizeX / 2;
                worldStartY = 0 - worldSizeY / 2;
        worldEndX = worldSizeX / 2;
        worldEndY = worldSizeY / 2;
        maxRoomSizeX = maxRoomSizeY = 20;
        roomGrid = new RoomGrid[worldSizeX, worldSizeY];
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                roomGrid[x, y] = new RoomGrid();
                roomGrid[x, y].tileType = TileType.Nothing;
            }
        }

        CreateRooms();
        CreateCorridors();
        DrawRooms();
    }

    private void Start()
    {
        Events.onFieldInitialized(new Vector2(worldStartX, worldStartY), new Vector2(worldEndX, worldEndY));
    }

    private void CreateCorridors()
    {
        AllRooms[] closestRoom = new AllRooms[allRooms.Count];
        int corridorSize = 4;
        for (int i = 0; i < allRooms.Count - 1; i++)
        {
            int distX = 10000, distY = 10000;
            foreach (var room in allRooms)
            {
                int tempX = 10000, tempY = 10000;
                if (room == allRooms[i] || room.hasCorridor) continue;

                if (room.startX > allRooms[i].endX)
                    tempX = room.startX - allRooms[i].endX;
                else if (room.startX <= allRooms[i].endX)
                    tempX = allRooms[i].endX - room.startX;

                if (room.startY > allRooms[i].endY)
                    tempY = room.startY - allRooms[i].endY;
                else if (room.startX <= allRooms[i].endX)
                    tempY = allRooms[i].endY - room.startY;

                if (tempX + tempY < distX + distY)
                {
                    distX = tempX;
                    distY = tempY;
                    closestRoom[i] = room;
                }
            }
            allRooms[i].hasCorridor = true;
        }

        for (int i = 0; i < allRooms.Count - 1; i++)
        {
            int distX, distY;

            bool allRoomsBiggerX = allRooms[i].startX >= closestRoom[i].endX;
            bool allRoomsBiggerY = allRooms[i].startY >= closestRoom[i].endY;

            distX = allRooms[i].startX >= closestRoom[i].endX ? allRooms[i].startX - closestRoom[i].endX : closestRoom[i].startX - allRooms[i].endX;
            distY = allRooms[i].startY >= closestRoom[i].endY ? allRooms[i].startY - closestRoom[i].endY : closestRoom[i].startY - allRooms[i].endY;

            if (distX >= distY)
            {
                allRooms[i].exit.exitType = allRoomsBiggerX ? ExitType.Left : ExitType.Right;
                closestRoom[i].exit.exitType = allRooms[i].exit.exitType == ExitType.Left ? ExitType.Right : ExitType.Left;
            }
            else
            {
                allRooms[i].exit.exitType = allRoomsBiggerY ? ExitType.Bottom : ExitType.Top;
                closestRoom[i].exit.exitType = allRooms[i].exit.exitType == ExitType.Bottom ? ExitType.Top : ExitType.Bottom;
            }


            CalculateExit(allRooms[i], corridorSize);
            CalculateExit(closestRoom[i], corridorSize);
            int middlePoint = 0;
            ExitType final = ExitType.Top;
            if (allRooms[i].exit.exitType == ExitType.Right)
            {
                middlePoint = closestRoom[i].exit.x - allRooms[i].exit.x;
                final = allRoomsBiggerY ? ExitType.Bottom : ExitType.Top;
            }
            else if (allRooms[i].exit.exitType == ExitType.Left)
            {
                middlePoint = allRooms[i].exit.x - closestRoom[i].exit.x;
                final = allRoomsBiggerY ? ExitType.Bottom : ExitType.Top;
            }
            else if (allRooms[i].exit.exitType == ExitType.Bottom)
            {
                middlePoint = allRooms[i].exit.y - closestRoom[i].exit.y;
                final = allRoomsBiggerX ? ExitType.Left : ExitType.Right;
            }
            else if (allRooms[i].exit.exitType == ExitType.Top)
            {
                middlePoint = closestRoom[i].exit.y - allRooms[i].exit.y;
                final = allRoomsBiggerX ? ExitType.Left : ExitType.Right;
            }
            middlePoint /= 2;


            ConnectCorridors(allRooms[i].exit, closestRoom[i].exit, middlePoint, final, corridorSize);
        }

    }

    private void ConnectCorridors(Exit allRooms, Exit closestRoom, int middlePoint, ExitType final, int corridorSize = 3)
    {
        Vector2Int startPos = ConnectMiddlePoint(allRooms, middlePoint, corridorSize);
        Vector2Int endPos = ConnectMiddlePoint(closestRoom, middlePoint, corridorSize);
        if (startPos == Vector2Int.zero || endPos == Vector2Int.zero || startPos.x == 0 || startPos.y == 0 || endPos.x == 0 || endPos.y == 0) return;
        int extra = Mathf.CeilToInt(corridorSize / 2);
        Vector2Int dest = Vector2Int.zero;
        Vector2Int start = Vector2Int.zero;
        switch (allRooms.exitType)
        {
            case ExitType.Right:
            case ExitType.Left:
                if (startPos.y > endPos.y)
                {
                    dest = startPos;
                    start = endPos;
                }
                else if (startPos.y <= endPos.y)
                {
                    dest = endPos;
                    start = startPos;

                }
                for (int y = start.y; y <= dest.y; y++)
                {
                    for (int x = startPos.x; x < startPos.x + corridorSize; x++)
                    {
                        roomGrid[x, y].tileType = TileType.Middle;
                    }
                }
                break;

            case ExitType.Bottom:
            case ExitType.Top:
                if (startPos.x > endPos.x)
                {
                    dest = startPos;
                    start = endPos;
                }
                else if (startPos.x <= endPos.x)
                {
                    dest = endPos;
                    start = startPos;

                }
                for (int x = start.x; x <= dest.x; x++)
                {
                    for (int y = startPos.y; y < startPos.y + corridorSize; y++)
                    {
                        roomGrid[x, y].tileType = TileType.Middle;
                    }
                }
                break;
        }
    }

    private Vector2Int ConnectMiddlePoint(Exit exit, int middlePoint, int corridorSize = 3)
    {
        int gridX = exit.x + Mathf.Abs(worldStartX);
        int gridY = exit.y + Mathf.Abs(worldStartY);
        int extra = Mathf.CeilToInt(corridorSize / 2);
        for (int i = 1; i <= middlePoint + extra; i++)
        {
            for (int a = 0; a < corridorSize; a++)
            {
                if (exit.exitType == ExitType.Right)
                {
                    roomGrid[gridX + i, gridY + a].tileType = TileType.Middle;
                }
                else if (exit.exitType == ExitType.Left)
                {
                    roomGrid[gridX - i, gridY + a].tileType = TileType.Middle;
                }
                else if (exit.exitType == ExitType.Top)
                {
                    roomGrid[gridX + a, gridY + i].tileType = TileType.Middle;
                }
                else if (exit.exitType == ExitType.Bottom)
                {
                    roomGrid[gridX + a, gridY - i].tileType = TileType.Middle;
                }
            }
        }
        if (exit.exitType == ExitType.Right) return new Vector2Int(gridX + middlePoint, gridY);
        if (exit.exitType == ExitType.Left) return new Vector2Int(gridX - (middlePoint + extra), gridY);
        if (exit.exitType == ExitType.Top) return new Vector2Int(gridX + extra, gridY + middlePoint);
        if (exit.exitType == ExitType.Bottom) return new Vector2Int(gridX, gridY - (middlePoint + extra));
        return Vector2Int.zero;
    }

    void CalculateExit(AllRooms room, int size = 3)
    {
        int spotX = 0, spotY = 0;
        if (room.exit.exitType == ExitType.Top)
        {
            spotX = Random.Range(room.startX + 1, room.endX - 1 - size);
            spotY = room.endY - 1;
        }
        else if (room.exit.exitType == ExitType.Bottom)
        {
            spotX = Random.Range(room.startX + 1, room.endX - 1 - size);
            spotY = room.startY;
        }
        else if (room.exit.exitType == ExitType.Left)
        {
            spotX = room.startX;
            spotY = Random.Range(room.startY + 1, room.endY - 2 - size);
        }
        else if (room.exit.exitType == ExitType.Right)
        {
            spotX = room.endX;
            spotY = Random.Range(room.startY + 1, room.endY - 2 - size);
        }
        int gridX = spotX + Mathf.Abs(worldStartX);
        int gridY = spotY + Mathf.Abs(worldStartY);
        for (int x = 0; x < size; x++)
        {
            switch (room.exit.exitType)
            {
                case ExitType.Bottom:
                case ExitType.Top:
                    roomGrid[gridX + x, gridY].tileType = TileType.Middle;
                    break;
                case ExitType.Left:
                case ExitType.Right:
                    roomGrid[gridX, gridY + x].tileType = TileType.Middle;
                    break;
            }
        }
        room.exit.x = spotX;
        room.exit.y = spotY;
    }

    void CreateRooms()
    {
        bool addRooms = true;
        int i = 0;
        while (addRooms && i < numberOfRooms)
        {
            int attempts = 0;
            while (attempts < maxAttempts)
            {
                int startX = Random.Range(worldStartX + 2, worldEndX - maxRoomSizeX - 3);
                int startY = Random.Range(worldStartY + 2, worldEndY - maxRoomSizeY - 3);
                int roomSizeX = Random.Range(7, maxRoomSizeX + 1);
                int roomSizeY = Random.Range(7, maxRoomSizeY + 1);
                if (CheckIfRoomFits(startX, startY, startX + roomSizeX, startY + roomSizeY))
                {
                    i++;
                    break;
                }
                else
                {
                    attempts++;
                    if (attempts >= maxAttempts)
                    {
                        addRooms = false;
                        break;
                    }
                }
            }
        }
    }
    void DrawRooms()
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        Tile middleTile = Array.Find(cellarTiles, _tile => _tile.name == "Middle");
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (roomGrid[x, y] != null)
                {
                    string piece = roomGrid[x, y].tileType.ToString();
                    switch (piece)
                    {
                        case "Nothing":
                            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), black);
                            break;
                        case "Middle":
                            floorTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), middleTile);
                            break;
                    }
                }
            }
        }

        RoomGrid[,] tempGrid = new RoomGrid[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                tempGrid[x, y] = new RoomGrid();
                tempGrid[x, y].tileType = roomGrid[x, y].tileType;
            }
        }
        SetWalls(tempGrid, startX, startY);
        SetCorners(startX, startY);
    }

    private void SetCorners(int startX, int startY)
    {
        Matrix4x4 horizontalFlip = Matrix4x4.TRS(new Vector3(1f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), new Vector3(-1f, 1f, 1f));
        Tile bigCornerTop = Array.Find(cellarTiles, _tile => _tile.name == "BigCornerTop");
        Tile bigCornerBottom = Array.Find(cellarTiles, _tile => _tile.name == "BigCornerBottom");
        Tile smallCorner = Array.Find(cellarTiles, _tile => _tile.name == "SmallCorner");
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (roomGrid[x, y] != null)
                {
                    string piece = roomGrid[x, y].tileType.ToString();
                    switch (piece)
                    {
                        case "Middle":
                            if (roomGrid[x, y - 1].tileType == TileType.Right && roomGrid[x + 1, y].tileType == TileType.Bottom)
                                foregroundCorners.SetTile(new Vector3Int(x - startX, y - startY, 0), smallCorner);
                            else if (roomGrid[x, y - 1].tileType == TileType.BottomRight && roomGrid[x + 1, y].tileType == TileType.Bottom)
                                foregroundCorners.SetTile(new Vector3Int(x - startX, y - startY, 0), smallCorner);
                            else if (roomGrid[x, y - 1].tileType == TileType.Right && roomGrid[x + 1, y].tileType == TileType.BottomRight)
                                foregroundCorners.SetTile(new Vector3Int(x - startX, y - startY, 0), smallCorner);
                            else if (roomGrid[x, y - 1].tileType == TileType.Left && roomGrid[x - 1, y].tileType == TileType.Bottom)
                            {
                                foregroundCorners.SetTile(new Vector3Int(x - startX, y - startY, 0), smallCorner);
                                foregroundCorners.SetTransformMatrix(new Vector3Int(x - startX, y - startY, 0), horizontalFlip);
                            }
                            else if (roomGrid[x, y - 1].tileType == TileType.Left && roomGrid[x - 1, y].tileType == TileType.BottomLeft)
                            {
                                foregroundCorners.SetTile(new Vector3Int(x - startX, y - startY, 0), smallCorner);
                                foregroundCorners.SetTransformMatrix(new Vector3Int(x - startX, y - startY, 0), horizontalFlip);
                            }
                            else if (roomGrid[x, y - 1].tileType == TileType.BottomLeft && roomGrid[x - 1, y].tileType == TileType.Bottom)
                            {
                                foregroundCorners.SetTile(new Vector3Int(x - startX, y - startY, 0), smallCorner);
                                foregroundCorners.SetTransformMatrix(new Vector3Int(x - startX, y - startY, 0), horizontalFlip);
                            }
                            break;
                        case "Top":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x - 1, y].tileType == TileType.Top &&
                              roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                              roomGrid[x + 1, y].tileType == TileType.Middle &&
                              roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x - 1, y].tileType == TileType.Top &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                            roomGrid[x + 1, y].tileType == TileType.Middle &&
                            roomGrid[x + 1, y + 1].tileType == TileType.TopLeftTwo)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopLeftTwo &&
                               roomGrid[x - 1, y].tileType == TileType.TopLeft &&
                               roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                               roomGrid[x + 1, y].tileType == TileType.Middle &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                 roomGrid[x - 1, y].tileType == TileType.Middle &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                 roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                 roomGrid[x + 1, y].tileType == TileType.Middle &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.TopLeftTwo)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                            roomGrid[x - 1, y].tileType == TileType.Middle &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                            roomGrid[x + 1, y].tileType == TileType.Middle &&
                            roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);

                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                roomGrid[x - 1, y].tileType == TileType.Middle &&
                                roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                roomGrid[x + 1, y].tileType == TileType.TopTwo &&
                                roomGrid[x + 1, y + 1].tileType == TileType.TopLeftExtra)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);

                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                               roomGrid[x - 1, y].tileType == TileType.Middle &&
                               roomGrid[x - 1, y + 1].tileType == TileType.TopRightTwo &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                               roomGrid[x + 1, y].tileType == TileType.Middle &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);

                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                               roomGrid[x - 1, y].tileType == TileType.Middle &&
                               roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x + 1, y].tileType == TileType.Top &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                            roomGrid[x - 1, y].tileType == TileType.Middle &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x + 1, y].tileType == TileType.TopRightExtra &&
                            roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                           roomGrid[x - 1, y].tileType == TileType.Middle &&
                           roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                           roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                           roomGrid[x, y + 1].tileType == TileType.Nothing &&
                           roomGrid[x + 1, y - 1].tileType == TileType.TopRightTwo &&
                           roomGrid[x + 1, y].tileType == TileType.TopRight &&
                           roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                               roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                               roomGrid[x - 1, y + 1].tileType == TileType.TopRightExtra &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                               roomGrid[x + 1, y].tileType == TileType.Middle &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                 roomGrid[x - 1, y].tileType == TileType.Middle &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.TopRightTwo &&
                                 roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                                 roomGrid[x + 1, y].tileType == TileType.Top &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                              roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                              roomGrid[x - 1, y + 1].tileType == TileType.TopRightExtra &&
                              roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x + 1, y].tileType == TileType.Top &&
                              roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                             roomGrid[x - 1, y].tileType == TileType.TopLeft &&
                             roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                             roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                             roomGrid[x, y + 1].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                             roomGrid[x + 1, y].tileType == TileType.Middle &&
                             roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopLeftTwo &&
                                   roomGrid[x - 1, y].tileType == TileType.TopLeft &&
                                   roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                   roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                   roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                   roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                   roomGrid[x + 1, y].tileType == TileType.TopTwo &&
                                   roomGrid[x + 1, y + 1].tileType == TileType.TopLeftExtra)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x - 1, y].tileType == TileType.Top &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                            roomGrid[x + 1, y].tileType == TileType.TopTwo &&
                            roomGrid[x + 1, y + 1].tileType == TileType.TopLeftExtra)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);
                            }
                            break;
                        case "TopLeft":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.Nothing &&
                                 roomGrid[x - 1, y].tileType == TileType.Nothing &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x, y - 1].tileType == TileType.TopLeftTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                 roomGrid[x + 1, y].tileType == TileType.Middle &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);

                            }
                            break;
                        case "TopLeftExtra":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.Top &&
                            roomGrid[x - 1, y].tileType == TileType.Nothing &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x + 1, y - 1].tileType == TileType.Middle &&
                            roomGrid[x + 1, y].tileType == TileType.Middle &&
                            roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x - startX + 1, y - startY - 1, 0), bigCornerBottom);

                            }
                            break;
                        case "TopRightExtra":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                roomGrid[x - 1, y].tileType == TileType.Middle &&
                                roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                roomGrid[x + 1, y - 1].tileType == TileType.TopRight &&
                                roomGrid[x + 1, y].tileType == TileType.Nothing &&
                                roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            break;
                        case "TopRight":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                 roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.TopRight &&
                                 roomGrid[x, y - 1].tileType == TileType.TopRightTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                              roomGrid[x - 1, y].tileType == TileType.Middle &&
                              roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                              roomGrid[x, y - 1].tileType == TileType.TopRightTwo &&
                              roomGrid[x, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                  roomGrid[x - 1, y].tileType == TileType.Middle &&
                                  roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                  roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                  roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                  roomGrid[x + 1, y - 1].tileType == TileType.TopRight &&
                                  roomGrid[x + 1, y].tileType == TileType.Nothing &&
                                  roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.Middle &&
                             roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                             roomGrid[x - 1, y + 1].tileType == TileType.TopRightExtra &&
                             roomGrid[x, y - 1].tileType == TileType.TopRightTwo &&
                             roomGrid[x, y + 1].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - startX - 1, y - startY, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - startX - 1, y - startY - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            break;

                    }
                }
            }
        }
    }

    void SetWalls(RoomGrid[,] tempGrid, int startX, int startY)
    {

        Tile left = Array.Find(cellarTiles, _tile => _tile.name == "Left");
        Tile right = Array.Find(cellarTiles, _tile => _tile.name == "Right");
        Tile top = Array.Find(cellarTiles, _tile => _tile.name == "Top");
        Tile topTwo = Array.Find(cellarTiles, _tile => _tile.name == "TopTwo");
        Tile topLeft = Array.Find(cellarTiles, _tile => _tile.name == "TopLeft");
        Tile topLeftTwo = Array.Find(cellarTiles, _tile => _tile.name == "TopLeftTwo");
        Tile topRight = Array.Find(cellarTiles, _tile => _tile.name == "TopRight");
        Tile topRightTwo = Array.Find(cellarTiles, _tile => _tile.name == "TopRightTwo");
        Tile bottom = Array.Find(cellarTiles, _tile => _tile.name == "Bottom");
        Tile bottomLeft = Array.Find(cellarTiles, _tile => _tile.name == "BottomLeft");
        Tile bottomRight = Array.Find(cellarTiles, _tile => _tile.name == "BottomRight");

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (tempGrid[x, y] != null)
                {
                    string piece = tempGrid[x, y].tileType.ToString();
                    switch (piece)
                    {
                        case "Middle":
                            CheckForLeftPiece(tempGrid, x, y, left);
                            CheckForRightPiece(tempGrid, x, y, right);
                            CheckForBottomLeft(tempGrid, x, y, bottomLeft);
                            CheckForBottomRight(tempGrid, x, y, bottomRight);
                            CheckForTopRight(tempGrid, x, y, topRight);
                            CheckForTopLeft(tempGrid, x, y, topLeft);
                            CheckForBottom(tempGrid, x, y, bottom);

                            if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                tempGrid[x - 1, y].tileType == TileType.Middle &&
                                tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                tempGrid[x, y - 1].tileType == TileType.Middle &&
                                tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                tempGrid[x + 1, y].tileType == TileType.Middle &&
                                tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), top);

                            }


                            else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                  tempGrid[x - 1, y].tileType == TileType.Middle &&
                                   tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                   tempGrid[x, y - 1].tileType == TileType.Middle &&
                                   tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                   tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                   tempGrid[x + 1, y].tileType == TileType.Middle &&
                                   tempGrid[x + 1, y + 1].tileType == TileType.Middle)
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), top);

                            }
                            else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                               tempGrid[x - 1, y].tileType == TileType.Middle &&
                                tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
                                tempGrid[x, y - 1].tileType == TileType.Middle &&
                                tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                tempGrid[x + 1, y].tileType == TileType.Middle &&
                                tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), top);
                            }
                            else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                                   tempGrid[x - 1, y].tileType == TileType.Middle &&
                                    tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
                                    tempGrid[x, y - 1].tileType == TileType.Middle &&
                                    tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                    tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
                                    tempGrid[x + 1, y].tileType == TileType.Middle &&
                                    tempGrid[x + 1, y + 1].tileType == TileType.Middle)
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), top);
                            }
                            break;
                    }
                }
            }
        }
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (roomGrid[x, y] != null)
                {
                    if (roomGrid[x, y].tileType == TileType.Top)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x - startX, y - 1 - startY, 0), topTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopLeft)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopLeftTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x - startX, y - 1 - startY, 0), topLeftTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopRight)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopRightTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x - startX, y - 1 - startY, 0), topRightTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopLeftExtra)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x - startX, y - 1 - startY, 0), topTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopRightExtra)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x - startX, y - 1 - startY, 0), topTwo);
                    }
                }
            }
        }
    }
    private bool CheckIfRoomFits(int startX, int startY, int endX, int endY)
    {
        int tStartX = startX - 3;
        int tEndX = endX + 3;
        int tStartY = startY - 3;
        int tEndY = endY + 3;
        foreach (var room in allRooms)
        {
            if (tStartX >= room.startX && tStartX <= room.endX && tStartY >= room.startY && tStartY <= room.endY) return false;
            if (tStartX >= room.startX && tStartX <= room.endX && tEndY >= room.startY && tEndY <= room.endY) return false;
            if (tEndX >= room.startX && tEndX <= room.endX && tStartY >= room.startY && tStartY <= room.endY) return false;
            if (tEndX >= room.startX && tEndX <= room.endX && tEndY >= room.startY && tEndY <= room.endY) return false;

            if (room.startX >= tStartX && room.startX <= tEndX && room.startY >= tStartY && room.startY <= tEndY) return false;
            if (room.startX >= tStartX && room.startX <= tEndX && room.endY >= tStartY && room.endY <= tEndY) return false;
            if (room.endX >= tStartX && room.endX <= tEndX && room.startY >= tStartY && room.startY <= tEndY) return false;
            if (room.endX >= tStartX && room.endX <= tEndX && room.endY >= tStartY && room.endY <= tEndY) return false;
        }
        allRooms.Add(new AllRooms { startX = startX, startY = startY, endX = endX, endY = endY });
        UpdateRoomGrid(startX, startY, endX, endY);
        return true;
    }

    void UpdateRoomGrid(int startX, int startY, int endX, int endY)
    {
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                int locX = x + Mathf.Abs(worldStartX);
                int locY = y + Mathf.Abs(worldStartY);
                roomGrid[locX, locY].tileType = TileType.Middle;
            }
        }
    }


    void CheckForLeftPiece(RoomGrid[,] tempGrid, int x, int y, Tile left)
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
                               tempGrid[x - 1, y].tileType == TileType.Nothing &&
                               tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                               tempGrid[x, y - 1].tileType == TileType.Middle &&
                               tempGrid[x, y + 1].tileType == TileType.Middle &&
                               tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
                               tempGrid[x + 1, y].tileType == TileType.Middle &&
                               tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                 tempGrid[x - 1, y].tileType == TileType.Nothing &&
                 tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                 tempGrid[x, y - 1].tileType == TileType.Middle &&
                 tempGrid[x, y + 1].tileType == TileType.Middle &&
                 tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
                 tempGrid[x + 1, y].tileType == TileType.Middle &&
                 tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
               tempGrid[x - 1, y].tileType == TileType.Nothing &&
               tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
               tempGrid[x, y - 1].tileType == TileType.Middle &&
               tempGrid[x, y + 1].tileType == TileType.Middle &&
               tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
               tempGrid[x + 1, y].tileType == TileType.Middle &&
               tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
             tempGrid[x - 1, y].tileType == TileType.Nothing &&
             tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
             tempGrid[x, y - 1].tileType == TileType.Middle &&
             tempGrid[x, y + 1].tileType == TileType.Middle &&
             tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
             tempGrid[x + 1, y].tileType == TileType.Middle &&
             tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
         tempGrid[x - 1, y].tileType == TileType.Nothing &&
         tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
         tempGrid[x, y - 1].tileType == TileType.Middle &&
         tempGrid[x, y + 1].tileType == TileType.Middle &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y].tileType == TileType.Middle &&
         tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x - 1, y].tileType == TileType.Nothing &&
         tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
         tempGrid[x, y - 1].tileType == TileType.Middle &&
         tempGrid[x, y + 1].tileType == TileType.Middle &&
         tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
         tempGrid[x + 1, y].tileType == TileType.Middle &&
         tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), left);
        }
    }

    void CheckForRightPiece(RoomGrid[,] tempGrid, int x, int y, Tile right)
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                              tempGrid[x - 1, y].tileType == TileType.Middle &&
                              tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
                              tempGrid[x, y - 1].tileType == TileType.Middle &&
                              tempGrid[x, y + 1].tileType == TileType.Middle &&
                              tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                              tempGrid[x + 1, y].tileType == TileType.Nothing &&
                              tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), right);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
         tempGrid[x - 1, y].tileType == TileType.Middle &&
         tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
         tempGrid[x, y - 1].tileType == TileType.Middle &&
         tempGrid[x, y + 1].tileType == TileType.Middle &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y].tileType == TileType.Nothing &&
         tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), right);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
           tempGrid[x - 1, y].tileType == TileType.Middle &&
           tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
           tempGrid[x, y - 1].tileType == TileType.Middle &&
           tempGrid[x, y + 1].tileType == TileType.Middle &&
           tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
           tempGrid[x + 1, y].tileType == TileType.Nothing &&
           tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), right);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
        tempGrid[x - 1, y].tileType == TileType.Middle &&
        tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
        tempGrid[x, y - 1].tileType == TileType.Middle &&
        tempGrid[x, y + 1].tileType == TileType.Middle &&
        tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
        tempGrid[x + 1, y].tileType == TileType.Nothing &&
        tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), right);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
             tempGrid[x - 1, y].tileType == TileType.Middle &&
             tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
             tempGrid[x, y - 1].tileType == TileType.Middle &&
             tempGrid[x, y + 1].tileType == TileType.Middle &&
             tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
             tempGrid[x + 1, y].tileType == TileType.Nothing &&
             tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), right);
        }
    }

    void CheckForBottomLeft(RoomGrid[,] tempGrid, int x, int y, Tile bottomLeft)
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        if (
            tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            tempGrid[x, y + 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Middle &&
            tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
        tempGrid[x - 1, y].tileType == TileType.Nothing &&
         tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
         tempGrid[x, y - 1].tileType == TileType.Nothing &&
         tempGrid[x, y + 1].tileType == TileType.Middle &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y].tileType == TileType.Middle &&
         tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
       tempGrid[x - 1, y].tileType == TileType.Nothing &&
        tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
        tempGrid[x, y - 1].tileType == TileType.Nothing &&
        tempGrid[x, y + 1].tileType == TileType.Middle &&
        tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
        tempGrid[x + 1, y].tileType == TileType.Middle &&
        tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
          tempGrid[x - 1, y].tileType == TileType.Nothing &&
           tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
           tempGrid[x, y - 1].tileType == TileType.Nothing &&
           tempGrid[x, y + 1].tileType == TileType.Middle &&
           tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
           tempGrid[x + 1, y].tileType == TileType.Middle &&
           tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
        tempGrid[x - 1, y].tileType == TileType.Nothing &&
          tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
          tempGrid[x, y - 1].tileType == TileType.Nothing &&
          tempGrid[x, y + 1].tileType == TileType.Middle &&
          tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
          tempGrid[x + 1, y].tileType == TileType.Middle &&
          tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
          tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            tempGrid[x, y + 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Middle &&
            tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomLeft);
        }
    }
    void CheckForBottomRight(RoomGrid[,] tempGrid, int x, int y, Tile bottomRight)
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            tempGrid[x, y + 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
                tempGrid[x - 1, y].tileType == TileType.Middle &&
                tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
                tempGrid[x, y - 1].tileType == TileType.Nothing &&
                tempGrid[x, y + 1].tileType == TileType.Middle &&
                tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                tempGrid[x + 1, y].tileType == TileType.Nothing &&
                tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
              tempGrid[x - 1, y].tileType == TileType.Middle &&
              tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
              tempGrid[x, y - 1].tileType == TileType.Nothing &&
              tempGrid[x, y + 1].tileType == TileType.Middle &&
              tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y].tileType == TileType.Nothing &&
              tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottomRight);
        }
    }
    void CheckForTopRight(RoomGrid[,] tempGrid, int x, int y, Tile topRight)
    {

        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Middle &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Middle &&
            tempGrid[x, y + 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
        tempGrid[x - 1, y].tileType == TileType.Middle &&
        tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
        tempGrid[x, y - 1].tileType == TileType.Middle &&
        tempGrid[x, y + 1].tileType == TileType.Middle &&
        tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
        tempGrid[x + 1, y].tileType == TileType.Nothing &&
        tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
              tempGrid[x - 1, y].tileType == TileType.Middle &&
              tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
              tempGrid[x, y - 1].tileType == TileType.Middle &&
              tempGrid[x, y + 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y].tileType == TileType.Nothing &&
              tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
          tempGrid[x - 1, y].tileType == TileType.Middle &&
          tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
          tempGrid[x, y - 1].tileType == TileType.Middle &&
          tempGrid[x, y + 1].tileType == TileType.Nothing &&
          tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
          tempGrid[x + 1, y].tileType == TileType.Middle &&
          tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
              tempGrid[x - 1, y].tileType == TileType.Middle &&
              tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
              tempGrid[x, y - 1].tileType == TileType.Middle &&
              tempGrid[x, y + 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
              tempGrid[x + 1, y].tileType == TileType.Nothing &&
              tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
            tempGrid[x, y - 1].tileType == TileType.Middle &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
           tempGrid[x - 1, y].tileType == TileType.Middle &&
           tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
           tempGrid[x, y - 1].tileType == TileType.Middle &&
           tempGrid[x, y + 1].tileType == TileType.Nothing &&
           tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
           tempGrid[x + 1, y].tileType == TileType.Nothing &&
           tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
         tempGrid[x - 1, y].tileType == TileType.Middle &&
         tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
         tempGrid[x, y - 1].tileType == TileType.Middle &&
         tempGrid[x, y + 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y].tileType == TileType.Nothing &&
         tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topRight);
        }
    }
    void CheckForTopLeft(RoomGrid[,] tempGrid, int x, int y, Tile topLeft)
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Middle &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y].tileType == TileType.Middle &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopLeft;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Middle &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y].tileType == TileType.Middle &&
            tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.TopLeft;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
       tempGrid[x - 1, y].tileType == TileType.Nothing &&
       tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
       tempGrid[x, y - 1].tileType == TileType.Middle &&
       tempGrid[x, y + 1].tileType == TileType.Nothing &&
       tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
       tempGrid[x + 1, y].tileType == TileType.Middle &&
       tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopLeftExtra;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
             tempGrid[x - 1, y].tileType == TileType.Nothing &&
             tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
             tempGrid[x, y - 1].tileType == TileType.Middle &&
             tempGrid[x, y + 1].tileType == TileType.Nothing &&
             tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
             tempGrid[x + 1, y].tileType == TileType.Middle &&
             tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.TopLeftExtra;
            backgroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), topLeft);
        }
    }
    void CheckForBottom(RoomGrid[,] tempGrid, int x, int y, Tile bottom)
    {
        int startX = Mathf.Abs(worldStartX);
        int startY = Mathf.Abs(worldStartY);

        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            tempGrid[x, y + 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Middle &&
            tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottom);
        }

        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
           tempGrid[x - 1, y].tileType == TileType.Middle &&
            tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            tempGrid[x, y + 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
            tempGrid[x + 1, y].tileType == TileType.Middle &&
            tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottom);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
        tempGrid[x - 1, y].tileType == TileType.Middle &&
        tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
        tempGrid[x, y - 1].tileType == TileType.Nothing &&
        tempGrid[x, y + 1].tileType == TileType.Middle &&
        tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
        tempGrid[x + 1, y].tileType == TileType.Middle &&
        tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottom);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Middle &&
           tempGrid[x - 1, y].tileType == TileType.Middle &&
           tempGrid[x - 1, y + 1].tileType == TileType.Middle &&
           tempGrid[x, y - 1].tileType == TileType.Nothing &&
           tempGrid[x, y + 1].tileType == TileType.Middle &&
           tempGrid[x + 1, y - 1].tileType == TileType.Middle &&
           tempGrid[x + 1, y].tileType == TileType.Middle &&
           tempGrid[x + 1, y + 1].tileType == TileType.Middle)
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x - startX, y - startY, 0), bottom);
        }
    }
    public void SpawnFloorObject(string oname, int x, int y)
    {
        GameObject obj = Array.Find(floorObjects, fo => fo.name == oname);
        if (obj != null)
        {
            var spawnedObj = Instantiate(obj);
            spawnedObj.transform.position = PathRequestManager.instance.grid.WorldPointFromNode(x, y);
            spawnedObj.GetComponent<SortingGroup>().sortingOrder = Info.SortingOrder(spawnedObj.transform.position.y);
        }
    }
}


public class RoomGrid
{
    public TileType tileType;
}

public enum TileType
{
    TopLeft,
    Top,
    TopRight,
    TopLeftTwo,
    TopTwo,
    TopRightTwo,
    TopRightExtra,
    TopLeftExtra,
    Left,
    Right,
    BottomLeft,
    Bottom,
    BottomRight,
    Middle,
    Nothing
}
public enum ExitType
{
    Top,
    Right,
    Bottom,
    Left,

}
public struct Exit
{
    public ExitType exitType;
    public int x, y;
}
public class AllRooms
{
    public int startX, startY, endX, endY;
    public Exit exit;
    public bool hasCorridor;
}
