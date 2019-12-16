using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    Tilemap backgroundTilemap;
    [HideInInspector]
    public Tilemap backgroundCorners;
    Tilemap foregroundTilemap;
    Tilemap floorTilemap;
    Tilemap foregroundCorners;
    Tile[] cellarTiles;
    GameObject[] floorObjects;
    Tile wallShade, black, smallWallShade;
    [HideInInspector]
    public Tile cornerTop;
    int numberOfRooms;
    [HideInInspector]
    public int worldSizeX, worldSizeY;
    int maxRoomSizeX, maxRoomSizeY;
    int maxAttempts = 30;
    public string[] bookShelves, barrels;
    public List<AllRooms> allRooms = new List<AllRooms>();
    public RoomGrid[,] roomGrid, objectGrid;
    public GameObject startCircle;
    private void Awake()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        numberOfRooms = 20;
        cellarTiles = Resources.LoadAll<Tile>("Cellar/Tiles");
        black = Resources.Load<Tile>("GenericTiles/Black");
        cornerTop = Array.Find(cellarTiles, _tile => _tile.name == "BigCornerTop");
        wallShade = Resources.Load<Tile>("GenericTiles/WallShade");
        smallWallShade = Resources.Load<Tile>("GenericTiles/SmallWallShade");
        floorObjects = Resources.LoadAll<GameObject>("FloorObjects");
        backgroundTilemap = GameObject.Find("BackgroundTilemap").GetComponent<Tilemap>();
        backgroundCorners = GameObject.Find("BackgroundCorners").GetComponent<Tilemap>();
        foregroundTilemap = GameObject.Find("ForegroundTilemap").GetComponent<Tilemap>();
        foregroundCorners = GameObject.Find("ForegroundCorners").GetComponent<Tilemap>();
        floorTilemap = GameObject.Find("FloorTilemap").GetComponent<Tilemap>();
        worldSizeX = worldSizeY = 100;
        backgroundTilemap.size = foregroundTilemap.size = foregroundCorners.size = backgroundCorners.size = floorTilemap.size = new Vector3Int(worldSizeX, worldSizeY, 0);

        bookShelves = (from obj in floorObjects
                       where obj.name.StartsWith("Book")
                       select obj.name).ToArray();
        barrels = (from obj in floorObjects
                   where obj.name.StartsWith("Barrel")
                   select obj.name).ToArray();
        maxRoomSizeX = maxRoomSizeY = 20;
        roomGrid = new RoomGrid[worldSizeX, worldSizeY];
        objectGrid = new RoomGrid[worldSizeX, worldSizeY];
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                roomGrid[x, y] = new RoomGrid();
                roomGrid[x, y].tileType = TileType.Nothing;
                objectGrid[x, y] = new RoomGrid();
                objectGrid[x, y].tileType = TileType.Nothing;
            }
        }
        //CreateBossRoom();
        CreateRooms();
        CreateCorridors();
        DrawRooms();
        AddExtraWallLayer();
        DrawWallShades();
        RandomizePlayerPosition();
    }

    private void AddExtraWallLayer()
    {
        Tile left = Array.Find(cellarTiles, _tile => _tile.name == "SmallLeft");
        Tile right = Array.Find(cellarTiles, _tile => _tile.name == "SmallRight");
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (roomGrid[x, y].tileType == TileType.TopLeft)
                {
                    foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
                }
                if (roomGrid[x, y].tileType == TileType.TopRight)
                {
                    foregroundTilemap.SetTile(new Vector3Int(x, y, 0), right);
                }
            }
        }
    }

    private void Start()
    {
        Events.onFieldInitialized(new Vector2(0, 0), new Vector2(worldSizeX, worldSizeY));
        MaterialModifier.InitializeValues();
        References.rf.enemyManager.GenerateEnemies(allRooms);
    }

    private void RandomizePlayerPosition()
    {
        var rooms = References.rf.levelGenerator.allRooms.ToArray();
        int x = 1000; int y = 1000;
        AllRooms startRoom = null;
        foreach (var room in rooms)
        {
            if (room.startX + room.startY < x + y)
            {
                startRoom = room;
                x = room.startX;
                y = room.startY;
            }
        }
        startRoom.roomType = RoomType.Start;
        References.rf.playerMovement.transform.position = new Vector2(x + 2, y + 2);
        GameObject a = Instantiate(startCircle);
        a.transform.position = new Vector2(x + 2, y + 2);
        References.rf.mainCamera.transform.position = new Vector3(References.rf.playerMovement.transform.position.x, References.rf.playerMovement.transform.position.y, -10f);
        //SpawnCarpets(10);
        SpawnBarrels(0);
        var chest = SpawnFloorObject("Treasure Chest", x + 3, y + 3, 0.2f, 0f, true);
        chest.GetComponent<TreasureChest>().CreateChestContent(
            new ChestContent { type = typeof(Weapon), random = true, level = 1 },
            new ChestContent { type = typeof(Headgear), random = true, level = 1},
            new ChestContent { type = typeof(Lightsource), random = true, level = 1 },
            new ChestContent { type = typeof(Consumable), random = true, level = 1, amount = 2 });
        var chestTwo = SpawnFloorObject("Treasure Chest", x + 3, worldSizeY - 5, 0.2f, 0f, true);
        chestTwo.GetComponent<TreasureChest>().CreateChestContent(
            new ChestContent { type = typeof(Weapon), random = false, name = "Flamethrower" },
            new ChestContent { type = typeof(Chestgear), random = false, name = "Rusty Hockey Chest" },
            new ChestContent { type = typeof(Ammo), random = false, name = "Gasoline", amount = 50 },
             new ChestContent { type = typeof(Ammo), random = false, name = "Gasoline", amount = 50 });
        var chestThree = SpawnFloorObject("Treasure Chest", worldSizeX - 3, worldSizeY / 2, 0.2f, 0f, true);
        chestThree.GetComponent<TreasureChest>().CreateChestContent(
            new ChestContent { type = typeof(Weapon), random = false, name = "Bone Crusher" },
            new ChestContent { type = typeof(Headgear), random = false, name = "Sunglasses"},
            new ChestContent { type = typeof(Weapon), random = false, name = "Bloodied Sword" },
             new ChestContent { type = typeof(Lightsource), random = false, name = "Blue Flamed Torch" });

        SpawnBookshelves(10);
        SpawnBoxes(25);

    }

    private void SpawnBookshelves(int v)
    {
        GameObject obj = CheckSize("Bookshelf", out Vector2Int size);
        int count = 0;
        while (count < v)
        {
            string shelf = bookShelves[Random.Range(0, bookShelves.Length)];
            bool canPlace = false;
            while (!canPlace)
            {
                int room = Random.Range(0, allRooms.Count);
                int y = allRooms[room].endY - 2;
                int x = Random.Range(allRooms[room].startX + 1, allRooms[room].endX - 1 - size.x);
                for (int aX = x; aX < x + size.x; aX++)
                {
                    canPlace = true;
                    if (roomGrid[aX, y].tileType != TileType.Middle || objectGrid[aX, y].tileType != TileType.Nothing)
                    {
                        canPlace = false;
                        break;
                    }
                    if (roomGrid[aX, y + 1].tileType != TileType.TopLeftTwo && roomGrid[aX, y + 1].tileType != TileType.TopTwo && roomGrid[aX, y + 1].tileType != TileType.TopRightTwo)
                    {
                        canPlace = false;
                        break;
                    }
                }
                if (canPlace)
                {
                    SpawnFloorObject(shelf, x, y, Random.Range(0f, 0.1f), 0.35f);
                    break;
                }
            }

            count++;
        }
    }
    private void SpawnBoxes(int v)
    {
        for (int i = 0; i < v; i++)
        {
            int x = Random.Range(0, worldSizeX - 3);
            int y = Random.Range(0, worldSizeY - 3);
            SpawnFloorObject("Box", x, y, 0.1f);
        }
    }
    private void SpawnBarrels(int v)
    {
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (Random.Range(0, 2) == 0) continue;
                if (roomGrid[x, y].tileType == TileType.TopLeftTwo)
                {
                    string barrel = barrels[Random.Range(0, barrels.Length)];
                    ForceSpawnFloorObject(barrel, x, y - 1, 0.2f, 0.6f, 0, 1);
                }
                else if (roomGrid[x, y].tileType == TileType.TopRightTwo)
                {
                    string barrel = barrels[Random.Range(0, barrels.Length)];
                    ForceSpawnFloorObject(barrel, x, y - 1, 0f, 0.6f, 0, 1);
                }
                else if (roomGrid[x, y].tileType == TileType.BottomLeft)
                {
                    string barrel = barrels[Random.Range(0, barrels.Length)];
                    ForceSpawnFloorObject(barrel, x, y, 0.1f, -0.1f, 0, 1);
                }
                else if (roomGrid[x, y].tileType == TileType.BottomRight)
                {
                    string barrel = barrels[Random.Range(0, barrels.Length)];
                    ForceSpawnFloorObject(barrel, x, y, 0.15f, -0.1f, 0, 1);
                }
            }
        }
    }
    private void SpawnCarpets(int v)
    {
        GameObject obj = Array.Find(floorObjects, _obj => _obj.name == "Carpet");
        foreach (var room in allRooms)
        {
            if (Random.Range(0, 2) == 0) continue;
            int sizeX = (room.endX - 1) - (room.startX + 2);
            int sizeY = (room.endY - 3) - (room.startY + 2);
            GameObject carpet = Instantiate(obj);
            carpet.transform.position = new Vector2(room.startX + 2, room.startY + 2);
            SpriteRenderer sr = carpet.GetComponent<SpriteRenderer>();
            sr.size = new Vector2(sizeX, sizeY);
        }
    }
    private void DrawWallShades()
    {
        Tile bg = Array.Find(cellarTiles, ct => ct.name == "BigCornerBottom");
        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (roomGrid[x, y].tileType == TileType.TopLeftTwo || roomGrid[x, y].tileType == TileType.TopTwo || roomGrid[x, y].tileType == TileType.TopRightTwo)
                {
                    backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), wallShade);
                }
                if (backgroundCorners.GetTile(new Vector3Int(x, y, 0)) == bg)
                {
                    var info = backgroundCorners.GetTransformMatrix(new Vector3Int(x, y, 0));
                    if (info.lossyScale.x == 1)
                    {
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), smallWallShade);
                    }
                    else
                    {
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), smallWallShade);
                        backgroundTilemap.SetTransformMatrix(new Vector3Int(x, y - 1, 0), info);
                    }
                }
            }
        }
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
                if (room == allRooms[i] || room.hasCorridor || room.roomType == RoomType.Boss) continue;

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
                        roomGrid[x, y].tileType = TileType.Corridor;
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
                        roomGrid[x, y].tileType = TileType.Corridor;
                    }
                }
                break;
        }
    }

    private Vector2Int ConnectMiddlePoint(Exit exit, int middlePoint, int corridorSize = 3)
    {
        int extra = Mathf.CeilToInt(corridorSize / 2);
        for (int i = 1; i <= middlePoint + extra; i++)
        {
            for (int a = 0; a < corridorSize; a++)
            {
                if (exit.exitType == ExitType.Right)
                {
                    roomGrid[exit.x + i, exit.y + a].tileType = TileType.Corridor;
                }
                else if (exit.exitType == ExitType.Left)
                {
                    roomGrid[exit.x - i, exit.y + a].tileType = TileType.Corridor;
                }
                else if (exit.exitType == ExitType.Top)
                {
                    roomGrid[exit.x + a, exit.y + i].tileType = TileType.Corridor;
                }
                else if (exit.exitType == ExitType.Bottom)
                {
                    roomGrid[exit.x + a, exit.y - i].tileType = TileType.Corridor;
                }
            }
        }
        if (exit.exitType == ExitType.Right) return new Vector2Int(exit.x + middlePoint, exit.y);
        if (exit.exitType == ExitType.Left) return new Vector2Int(exit.x - (middlePoint + extra), exit.y);
        if (exit.exitType == ExitType.Top) return new Vector2Int(exit.x + extra, exit.y + middlePoint);
        if (exit.exitType == ExitType.Bottom) return new Vector2Int(exit.x, exit.y - (middlePoint + extra));
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

        for (int x = 0; x < size; x++)
        {
            switch (room.exit.exitType)
            {
                case ExitType.Bottom:
                case ExitType.Top:
                    roomGrid[spotX, spotY].tileType = TileType.Corridor;
                    break;
                case ExitType.Left:
                case ExitType.Right:
                    roomGrid[spotX, spotY + x].tileType = TileType.Corridor;
                    break;
            }
        }
        room.exit.x = spotX;
        room.exit.y = spotY;
    }
    void CreateBossRoom()
    {
        Vector2Int size = new Vector2Int(40, 25);
        Vector2Int start = new Vector2Int(3, worldSizeY - size.y - 3);
        if (CheckIfRoomFits(start.x, start.y, start.x + size.x, start.y + size.y))
        {
            allRooms[0].roomType = RoomType.Boss;
        }
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
                int startX = Random.Range(0 + 2, worldSizeX - maxRoomSizeX - 3);
                int startY = Random.Range(0 + 2, worldSizeY - maxRoomSizeY - 3);
                int roomSizeX = Random.Range(5, maxRoomSizeX + 1);
                int roomSizeY = Random.Range(5, maxRoomSizeY + 1);
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
                            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), black);
                            break;
                        case "Middle":
                        case "Corridor":
                            floorTilemap.SetTile(new Vector3Int(x, y, 0), middleTile);
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
        SetWalls(tempGrid);
        SetCorners();
    }

    private void SetCorners()
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
                        case "Corridor":
                            if (roomGrid[x, y - 1].tileType == TileType.Right && roomGrid[x + 1, y].tileType == TileType.Bottom)
                                foregroundCorners.SetTile(new Vector3Int(x, y, 0), smallCorner);
                            else if (roomGrid[x, y - 1].tileType == TileType.BottomRight && roomGrid[x + 1, y].tileType == TileType.Bottom)
                                foregroundCorners.SetTile(new Vector3Int(x, y, 0), smallCorner);
                            else if (roomGrid[x, y - 1].tileType == TileType.Right && roomGrid[x + 1, y].tileType == TileType.BottomRight)
                                foregroundCorners.SetTile(new Vector3Int(x, y, 0), smallCorner);
                            else if (roomGrid[x, y - 1].tileType == TileType.Left && roomGrid[x - 1, y].tileType == TileType.Bottom)
                            {
                                foregroundCorners.SetTile(new Vector3Int(x, y, 0), smallCorner);
                                foregroundCorners.SetTransformMatrix(new Vector3Int(x, y, 0), horizontalFlip);
                            }
                            else if (roomGrid[x, y - 1].tileType == TileType.Left && roomGrid[x - 1, y].tileType == TileType.BottomLeft)
                            {
                                foregroundCorners.SetTile(new Vector3Int(x, y, 0), smallCorner);
                                foregroundCorners.SetTransformMatrix(new Vector3Int(x, y, 0), horizontalFlip);
                            }
                            else if (roomGrid[x, y - 1].tileType == TileType.BottomLeft && roomGrid[x - 1, y].tileType == TileType.Bottom)
                            {
                                foregroundCorners.SetTile(new Vector3Int(x, y, 0), smallCorner);
                                foregroundCorners.SetTransformMatrix(new Vector3Int(x, y, 0), horizontalFlip);
                            }
                            break;
                        case "Top":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x - 1, y].tileType == TileType.Top &&
                              roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x, y + 1].tileType == TileType.Nothing &&
                              CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                              CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                              roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x - 1, y].tileType == TileType.Top &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                            CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                            roomGrid[x + 1, y + 1].tileType == TileType.TopLeftTwo)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                        roomGrid[x - 1, y].tileType == TileType.TopLeftExtra &&
                        roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                        roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                        roomGrid[x, y + 1].tileType == TileType.Nothing &&
                        CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                        CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                        roomGrid[x + 1, y + 1].tileType == TileType.TopLeftTwo)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopLeftTwo &&
                               roomGrid[x - 1, y].tileType == TileType.TopLeft &&
                               roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                               CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                           roomGrid[x - 1, y].tileType == TileType.Top &&
                           roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                           roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                           roomGrid[x, y + 1].tileType == TileType.Nothing &&
                           roomGrid[x + 1, y - 1].tileType == TileType.Right &&
                           CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                           roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                                 CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                 roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                                 CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.TopLeftTwo)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                            CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                            CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                            roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);

                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                                CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                                roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                                roomGrid[x + 1, y].tileType == TileType.TopTwo &&
                                roomGrid[x + 1, y + 1].tileType == TileType.TopLeftExtra)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);

                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                               CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                               roomGrid[x - 1, y + 1].tileType == TileType.TopRightTwo &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                               CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);

                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                               CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                               roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x + 1, y].tileType == TileType.Top &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                            CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x + 1, y].tileType == TileType.TopRightExtra &&
                            roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                           CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                           roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                           roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                           roomGrid[x, y + 1].tileType == TileType.Nothing &&
                           roomGrid[x + 1, y - 1].tileType == TileType.TopRightTwo &&
                           roomGrid[x + 1, y].tileType == TileType.TopRight &&
                           roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                               roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                               roomGrid[x - 1, y + 1].tileType == TileType.TopRightExtra &&
                               roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                               roomGrid[x, y + 1].tileType == TileType.Nothing &&
                               CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                               CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                               roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                                 CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.TopRightTwo &&
                                 roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                                 roomGrid[x + 1, y].tileType == TileType.Top &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);
                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                              roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                              roomGrid[x - 1, y + 1].tileType == TileType.TopRightExtra &&
                              roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y - 1].tileType == TileType.TopTwo &&
                              roomGrid[x + 1, y].tileType == TileType.Top &&
                              roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
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
                             CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                             CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                             roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopLeftTwo &&
                                   roomGrid[x - 1, y].tileType == TileType.TopLeft &&
                                   roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                   roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                   roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                   CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                                   roomGrid[x + 1, y].tileType == TileType.TopTwo &&
                                   roomGrid[x + 1, y + 1].tileType == TileType.TopLeftExtra)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            else if (roomGrid[x - 1, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x - 1, y].tileType == TileType.Top &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                            roomGrid[x + 1, y].tileType == TileType.TopTwo &&
                            roomGrid[x + 1, y + 1].tileType == TileType.TopLeftExtra)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);
                            }
                            break;
                        case "TopLeft":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.Nothing &&
                                 roomGrid[x - 1, y].tileType == TileType.Nothing &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x, y - 1].tileType == TileType.TopLeftTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                                 CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);

                            }
                            break;
                        case "TopLeftExtra":
                            if (roomGrid[x - 1, y - 1].tileType == TileType.Top &&
                            roomGrid[x - 1, y].tileType == TileType.Nothing &&
                            roomGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                            roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                            roomGrid[x, y + 1].tileType == TileType.Nothing &&
                            CheckIfFloor(roomGrid[x + 1, y - 1].tileType) &&
                            CheckIfFloor(roomGrid[x + 1, y].tileType) &&
                            roomGrid[x + 1, y + 1].tileType == TileType.Left)
                            {
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y, 0), bigCornerTop);
                                backgroundCorners.SetTile(new Vector3Int(x + 1, y - 1, 0), bigCornerBottom);

                            }
                            break;
                        case "TopRightExtra":
                            if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                                CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                                roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                roomGrid[x + 1, y - 1].tileType == TileType.TopRight &&
                                roomGrid[x + 1, y].tileType == TileType.Nothing &&
                                roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            break;
                        case "TopRight":
                            if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                                 roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                                 roomGrid[x - 1, y + 1].tileType == TileType.TopRight &&
                                 roomGrid[x, y - 1].tileType == TileType.TopRightTwo &&
                                 roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y].tileType == TileType.Nothing &&
                                 roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                              CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                              roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                              roomGrid[x, y - 1].tileType == TileType.TopRightTwo &&
                              roomGrid[x, y + 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y].tileType == TileType.Nothing &&
                              roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                                  CheckIfFloor(roomGrid[x - 1, y].tileType) &&
                                  roomGrid[x - 1, y + 1].tileType == TileType.Right &&
                                  roomGrid[x, y - 1].tileType == TileType.TopTwo &&
                                  roomGrid[x, y + 1].tileType == TileType.Nothing &&
                                  roomGrid[x + 1, y - 1].tileType == TileType.TopRight &&
                                  roomGrid[x + 1, y].tileType == TileType.Nothing &&
                                  roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
                                backgroundCorners.SetTile(bigTopPos, bigCornerTop);
                                backgroundCorners.SetTile(bigBotPos, bigCornerBottom);
                                backgroundCorners.SetTransformMatrix(bigTopPos, horizontalFlip);
                                backgroundCorners.SetTransformMatrix(bigBotPos, horizontalFlip);

                            }
                            else if (CheckIfFloor(roomGrid[x - 1, y - 1].tileType) &&
                             roomGrid[x - 1, y].tileType == TileType.TopTwo &&
                             roomGrid[x - 1, y + 1].tileType == TileType.TopRightExtra &&
                             roomGrid[x, y - 1].tileType == TileType.TopRightTwo &&
                             roomGrid[x, y + 1].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y].tileType == TileType.Nothing &&
                             roomGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                Vector3Int bigTopPos = new Vector3Int(x - 1, y, 0);
                                Vector3Int bigBotPos = new Vector3Int(x - 1, y - 1, 0);
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

    void SetWalls(RoomGrid[,] tempGrid)
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

        for (int x = 1; x < worldSizeX - 1; x++)
        {
            for (int y = 1; y < worldSizeY - 1; y++)
            {
                if (tempGrid[x, y] != null)
                {
                    string piece = tempGrid[x, y].tileType.ToString();
                    switch (piece)
                    {
                        case "Middle":
                        case "Corridor":
                            CheckForLeftPiece(tempGrid, x, y, left);
                            CheckForRightPiece(tempGrid, x, y, right);
                            CheckForBottomLeft(tempGrid, x, y, bottomLeft);
                            CheckForBottomRight(tempGrid, x, y, bottomRight);
                            CheckForTopRight(tempGrid, x, y, topRight);
                            CheckForTopLeft(tempGrid, x, y, topLeft);
                            CheckForBottom(tempGrid, x, y, bottom);

                            if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                                CheckIfFloor(tempGrid[x - 1, y].tileType) &&
                                tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                CheckIfFloor(tempGrid[x, y - 1].tileType) &&
                                tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
                                CheckIfFloor(tempGrid[x + 1, y].tileType) &&
                                tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), top);

                            }


                            else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                                  CheckIfFloor(tempGrid[x - 1, y].tileType) &&
                                   tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                                   CheckIfFloor(tempGrid[x, y - 1].tileType) &&
                                   tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                   CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
                                   CheckIfFloor(tempGrid[x + 1, y].tileType) &&
                                   CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), top);

                            }
                            else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                               CheckIfFloor(tempGrid[x - 1, y].tileType) &&
                                CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
                                CheckIfFloor(tempGrid[x, y - 1].tileType) &&
                                tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
                                CheckIfFloor(tempGrid[x + 1, y].tileType) &&
                                tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), top);
                            }
                            else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                                   CheckIfFloor(tempGrid[x - 1, y].tileType) &&
                                    CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
                                    CheckIfFloor(tempGrid[x, y - 1].tileType) &&
                                    tempGrid[x, y + 1].tileType == TileType.Nothing &&
                                    CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
                                    CheckIfFloor(tempGrid[x + 1, y].tileType) &&
                                    CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
                            {
                                roomGrid[x, y].tileType = TileType.Top;
                                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), top);
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
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), topTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopLeft)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopLeftTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), topLeftTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopRight)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopRightTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), topRightTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopLeftExtra)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), topTwo);
                    }
                    else if (roomGrid[x, y].tileType == TileType.TopRightExtra)
                    {
                        roomGrid[x, y - 1].tileType = TileType.TopTwo;
                        backgroundTilemap.SetTile(new Vector3Int(x, y - 1, 0), topTwo);
                    }
                }
            }
        }
    }
    private bool CheckIfRoomFits(int startX, int startY, int endX, int endY)
    {
        int tStartX = startX - 4;
        int tEndX = endX + 4;
        int tStartY = startY - 4;
        int tEndY = endY + 4;
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
                roomGrid[x, y].tileType = TileType.Middle;
            }
        }
    }


    void CheckForLeftPiece(RoomGrid[,] tempGrid, int x, int y, Tile left)
    {
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                 tempGrid[x - 1, y].tileType == TileType.Nothing &&
                 tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
                 CheckIfFloor(tempGrid[x, y - 1].tileType) &&
                 CheckIfFloor(tempGrid[x, y + 1].tileType) &&
                 CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
                 CheckIfFloor(tempGrid[x + 1, y].tileType) &&
                 CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
               tempGrid[x - 1, y].tileType == TileType.Nothing &&
               CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
               CheckIfFloor(tempGrid[x, y - 1].tileType) &&
               CheckIfFloor(tempGrid[x, y + 1].tileType) &&
               CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
               CheckIfFloor(tempGrid[x + 1, y].tileType) &&
               CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
             tempGrid[x - 1, y].tileType == TileType.Nothing &&
             CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
             CheckIfFloor(tempGrid[x, y - 1].tileType) &&
             CheckIfFloor(tempGrid[x, y + 1].tileType) &&
             CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
             CheckIfFloor(tempGrid[x + 1, y].tileType) &&
             CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
         tempGrid[x - 1, y].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
         CheckIfFloor(tempGrid[x, y - 1].tileType) &&
         CheckIfFloor(tempGrid[x, y + 1].tileType) &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x + 1, y].tileType) &&
         CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x - 1, y].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
         CheckIfFloor(tempGrid[x, y - 1].tileType) &&
         CheckIfFloor(tempGrid[x, y + 1].tileType) &&
         CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
         CheckIfFloor(tempGrid[x + 1, y].tileType) &&
         tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.Left;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), left);
        }
    }

    void CheckForRightPiece(RoomGrid[,] tempGrid, int x, int y, Tile right)
    {
        if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                              CheckIfFloor(tempGrid[x - 1, y].tileType) &&
                              CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
                              CheckIfFloor(tempGrid[x, y - 1].tileType) &&
                              CheckIfFloor(tempGrid[x, y + 1].tileType) &&
                              tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                              tempGrid[x + 1, y].tileType == TileType.Nothing &&
                              tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), right);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
         CheckIfFloor(tempGrid[x - 1, y].tileType) &&
         CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
         CheckIfFloor(tempGrid[x, y - 1].tileType) &&
         CheckIfFloor(tempGrid[x, y + 1].tileType) &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), right);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
           CheckIfFloor(tempGrid[x, y - 1].tileType) &&
           CheckIfFloor(tempGrid[x, y + 1].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
           tempGrid[x + 1, y].tileType == TileType.Nothing &&
           tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), right);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
        CheckIfFloor(tempGrid[x - 1, y].tileType) &&
        CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
        CheckIfFloor(tempGrid[x, y - 1].tileType) &&
        CheckIfFloor(tempGrid[x, y + 1].tileType) &&
        CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
        tempGrid[x + 1, y].tileType == TileType.Nothing &&
        CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), right);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
             CheckIfFloor(tempGrid[x - 1, y].tileType) &&
             CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
             CheckIfFloor(tempGrid[x, y - 1].tileType) &&
             CheckIfFloor(tempGrid[x, y + 1].tileType) &&
             CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
             tempGrid[x + 1, y].tileType == TileType.Nothing &&
             CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Right;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), right);
        }
    }

    void CheckForBottomLeft(RoomGrid[,] tempGrid, int x, int y, Tile bottomLeft)
    {
        if (
            tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
        tempGrid[x - 1, y].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
         tempGrid[x, y - 1].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x, y + 1].tileType) &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x + 1, y].tileType) &&
         CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
       tempGrid[x - 1, y].tileType == TileType.Nothing &&
        tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
        tempGrid[x, y - 1].tileType == TileType.Nothing &&
        CheckIfFloor(tempGrid[x, y + 1].tileType) &&
        CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
        CheckIfFloor(tempGrid[x + 1, y].tileType) &&
        CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
          tempGrid[x - 1, y].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
           tempGrid[x, y - 1].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x, y + 1].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomLeft);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
        tempGrid[x - 1, y].tileType == TileType.Nothing &&
          tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
          tempGrid[x, y - 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x, y + 1].tileType) &&
          tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x + 1, y].tileType) &&
          CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomLeft);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
          CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomLeft;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomLeft);
        }
    }
    void CheckForBottomRight(RoomGrid[,] tempGrid, int x, int y, Tile bottomRight)
    {
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
                CheckIfFloor(tempGrid[x - 1, y].tileType) &&
                CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
                tempGrid[x, y - 1].tileType == TileType.Nothing &&
                CheckIfFloor(tempGrid[x, y + 1].tileType) &&
                tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
                tempGrid[x + 1, y].tileType == TileType.Nothing &&
                tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
              CheckIfFloor(tempGrid[x - 1, y].tileType) &&
              CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
              tempGrid[x, y - 1].tileType == TileType.Nothing &&
              CheckIfFloor(tempGrid[x, y + 1].tileType) &&
              tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y].tileType == TileType.Nothing &&
              CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomRight);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x - 1, y].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
           tempGrid[x, y - 1].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x, y + 1].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
           tempGrid[x + 1, y].tileType == TileType.Nothing &&
           tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.BottomRight;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottomRight);
        }
    }
    void CheckForTopRight(RoomGrid[,] tempGrid, int x, int y, Tile topRight)
    {
        if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y - 1].tileType) &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
        CheckIfFloor(tempGrid[x - 1, y].tileType) &&
        tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
        CheckIfFloor(tempGrid[x, y - 1].tileType) &&
        CheckIfFloor(tempGrid[x, y + 1].tileType) &&
        tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
        tempGrid[x + 1, y].tileType == TileType.Nothing &&
        CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
              CheckIfFloor(tempGrid[x - 1, y].tileType) &&
              CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
              CheckIfFloor(tempGrid[x, y - 1].tileType) &&
              tempGrid[x, y + 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
              tempGrid[x + 1, y].tileType == TileType.Nothing &&
              tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
          CheckIfFloor(tempGrid[x - 1, y].tileType) &&
          tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x, y - 1].tileType) &&
          tempGrid[x, y + 1].tileType == TileType.Nothing &&
          tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x + 1, y].tileType) &&
          CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.TopRight;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
              CheckIfFloor(tempGrid[x - 1, y].tileType) &&
              tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
              CheckIfFloor(tempGrid[x, y - 1].tileType) &&
              tempGrid[x, y + 1].tileType == TileType.Nothing &&
              CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
              tempGrid[x + 1, y].tileType == TileType.Nothing &&
              tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
            CheckIfFloor(tempGrid[x, y - 1].tileType) &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
            tempGrid[x + 1, y].tileType == TileType.Nothing &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
           CheckIfFloor(tempGrid[x, y - 1].tileType) &&
           tempGrid[x, y + 1].tileType == TileType.Nothing &&
           tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
           tempGrid[x + 1, y].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
         CheckIfFloor(tempGrid[x - 1, y].tileType) &&
         tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x, y - 1].tileType) &&
         tempGrid[x, y + 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
         tempGrid[x + 1, y].tileType == TileType.Nothing &&
         CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.TopRightExtra;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topRight);
        }
    }
    void CheckForTopLeft(RoomGrid[,] tempGrid, int x, int y, Tile topLeft)
    {
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y - 1].tileType) &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopLeft;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            tempGrid[x - 1, y].tileType == TileType.Nothing &&
            tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y - 1].tileType) &&
            tempGrid[x, y + 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.TopLeft;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topLeft);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
       tempGrid[x - 1, y].tileType == TileType.Nothing &&
       tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
       CheckIfFloor(tempGrid[x, y - 1].tileType) &&
       tempGrid[x, y + 1].tileType == TileType.Nothing &&
       CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
       CheckIfFloor(tempGrid[x + 1, y].tileType) &&
       tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopLeftExtra;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topLeft);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
             tempGrid[x - 1, y].tileType == TileType.Nothing &&
             tempGrid[x - 1, y + 1].tileType == TileType.Nothing &&
             CheckIfFloor(tempGrid[x, y - 1].tileType) &&
             tempGrid[x, y + 1].tileType == TileType.Nothing &&
             CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
             CheckIfFloor(tempGrid[x + 1, y].tileType) &&
             CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.TopLeftExtra;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x - 1, y].tileType) &&
          CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
          CheckIfFloor(tempGrid[x, y - 1].tileType) &&
          tempGrid[x, y + 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
          CheckIfFloor(tempGrid[x + 1, y].tileType) &&
          tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopLeft;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topLeft);
        }
        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
          tempGrid[x - 1, y].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
          CheckIfFloor(tempGrid[x, y - 1].tileType) &&
          tempGrid[x, y + 1].tileType == TileType.Nothing &&
          CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
          CheckIfFloor(tempGrid[x + 1, y].tileType) &&
          tempGrid[x + 1, y + 1].tileType == TileType.Nothing)
        {
            roomGrid[x, y].tileType = TileType.TopLeft;
            backgroundTilemap.SetTile(new Vector3Int(x, y, 0), topLeft);
        }
    }
    void CheckForBottom(RoomGrid[,] tempGrid, int x, int y, Tile bottom)
    {
        if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottom);
        }

        else if (tempGrid[x - 1, y - 1].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x - 1, y].tileType) &&
            CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
            tempGrid[x, y - 1].tileType == TileType.Nothing &&
            CheckIfFloor(tempGrid[x, y + 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y].tileType) &&
            CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottom);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
        CheckIfFloor(tempGrid[x - 1, y].tileType) &&
        CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
        tempGrid[x, y - 1].tileType == TileType.Nothing &&
        CheckIfFloor(tempGrid[x, y + 1].tileType) &&
        tempGrid[x + 1, y - 1].tileType == TileType.Nothing &&
        CheckIfFloor(tempGrid[x + 1, y].tileType) &&
        CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottom);
        }
        else if (CheckIfFloor(tempGrid[x - 1, y - 1].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y].tileType) &&
           CheckIfFloor(tempGrid[x - 1, y + 1].tileType) &&
           tempGrid[x, y - 1].tileType == TileType.Nothing &&
           CheckIfFloor(tempGrid[x, y + 1].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y - 1].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y].tileType) &&
           CheckIfFloor(tempGrid[x + 1, y + 1].tileType))
        {
            roomGrid[x, y].tileType = TileType.Bottom;
            foregroundTilemap.SetTile(new Vector3Int(x, y, 0), bottom);
        }
    }

    public GameObject CheckSize(string oname, out Vector2Int size)
    {
        GameObject obj = Array.Find(floorObjects, fo => fo.name == oname);
        var sr = obj.GetComponentInChildren<SpriteRenderer>();
        size = new Vector2Int(Mathf.CeilToInt(sr.bounds.size.x), Mathf.CeilToInt(sr.bounds.size.y));
        return obj;
    }

    public void ForceSpawnFloorObject(string oname, int x, int y, float offsetX = 0f, float offsetY = 0f, int xSizeExtra = 0, int ySizeExtra = 0)
    {
        GameObject obj = Array.Find(floorObjects, fo => fo.name == oname);
        Vector2Int node = new Vector2Int(x, y);
        if (obj != null)
        {
            var sr = obj.GetComponentInChildren<SpriteRenderer>();
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(sr.bounds.size.x), Mathf.CeilToInt(sr.bounds.size.y));

            var spawnedObj = Instantiate(obj);
            objectGrid[node.x, node.y].tileType = TileType.Object;
            for (int aX = 1; aX < size.x + xSizeExtra; aX++)
            {
                for (int aY = 1; aY < size.y + ySizeExtra; aY++)
                {
                    objectGrid[node.x + aX, node.y + aY].tileType = TileType.ObjectPlace;
                }
            }
            objectGrid[node.x, node.y].objectSize = new Vector2Int(size.x * (2 + xSizeExtra), size.y + ySizeExtra);

            spawnedObj.transform.position = new Vector2(node.x + offsetX, node.y + offsetY);
            spawnedObj.name = oname;
            spawnedObj.GetComponentInChildren<SortingGroup>().sortingOrder = Info.SortingOrder(spawnedObj.transform.position.y);
        }
    }
    public GameObject SpawnFloorObject(string oname, int x, int y, float offsetX = 0f, float offsetY = 0f, bool infiniteAttempts = false)
    {
        GameObject obj = Array.Find(floorObjects, fo => fo.name == oname);
        Vector2Int node = new Vector2Int(x, y);
        if (obj != null)
        {
            var sr = obj.GetComponentInChildren<SpriteRenderer>();
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(sr.bounds.size.x), Mathf.CeilToInt(sr.bounds.size.y));

            bool canBePlaced = false;
            int attempts = 0;
            while (infiniteAttempts || !canBePlaced && attempts < 10)
            {
                canBePlaced = true;
                
                for (int aX = 0; aX < size.x; aX++)
                {
                    if (roomGrid[node.x + aX, node.y].tileType != TileType.Middle && roomGrid[node.x + aX, node.y].tileType != TileType.Corridor || objectGrid[node.x + aX, node.y].tileType == TileType.Object || objectGrid[node.x + aX, node.y].tileType == TileType.ObjectPlace)
                        canBePlaced = false;
                }
                if (!canBePlaced)
                {
                    int newX = Mathf.Clamp(node.x + Random.Range(-15, 16), 0, worldSizeX - size.x - 1);
                    int newY = Mathf.Clamp(node.y + Random.Range(-15, 16), 0, worldSizeY - size.y - 1);
                    node = new Vector2Int(newX, newY);
                }
                if (canBePlaced) infiniteAttempts = false;
                attempts++;
            }
            if (!canBePlaced) return null;
            var spawnedObj = Instantiate(obj);
            objectGrid[node.x, node.y].tileType = TileType.Object;
            for (int aX = 1; aX < size.x; aX++)
            {
                objectGrid[node.x + aX, node.y].tileType = TileType.ObjectPlace;
            }
            objectGrid[node.x, node.y].objectSize = new Vector2Int(size.x * 2, size.y);
            spawnedObj.transform.position = new Vector2(node.x + offsetX, node.y + offsetY);
            spawnedObj.name = oname;
            spawnedObj.GetComponentInChildren<SortingGroup>().sortingOrder = Info.SortingOrder(spawnedObj.transform.position.y);
            return spawnedObj;
        }
        return null;
    }
    public void SpawnBackgroundObject(string oname, int x, int y, float offsetX = 0f, float offsetY = 0f)
    {
        GameObject obj = Array.Find(floorObjects, fo => fo.name == oname);
        Vector2Int node = new Vector2Int(x, y);
        if (obj != null)
        {
            var sr = obj.GetComponentInChildren<SpriteRenderer>();
            Vector2Int size = new Vector2Int(Mathf.CeilToInt(sr.bounds.size.x), Mathf.CeilToInt(sr.bounds.size.y));
            bool canBePlaced = false;
            while (!canBePlaced)
            {
                canBePlaced = true;
                for (int aX = 0; aX < size.x; aX++)
                {
                    for (int aY = 0; aY < size.y; aY++)
                    {
                        if (roomGrid[node.x + aX, node.y + aY].tileType != TileType.Middle && roomGrid[node.x + aX, node.y + aY].tileType != TileType.Corridor || objectGrid[node.x + aX, node.y + aY].tileType == TileType.Background)
                            canBePlaced = false;
                    }
                }
                if (!canBePlaced)
                {
                    int newX = Mathf.Clamp(node.x + Random.Range(-15, 16), 0, worldSizeX - size.x - 1);
                    int newY = Mathf.Clamp(node.y + Random.Range(-15, 16), 0, worldSizeY - size.y - 1);
                    node = new Vector2Int(newX, newY);
                }
            }
            var spawnedObj = Instantiate(obj);
            for (int aX = 0; aX < size.x; aX++)
            {
                for (int aY = 0; aY < size.y; aY++)
                {
                    objectGrid[node.x + aX, node.y + aY].tileType = TileType.Background;
                }
            }
            spawnedObj.transform.position = new Vector2(node.x + offsetX, node.y + offsetY);
            spawnedObj.name = oname;
            spawnedObj.GetComponentInChildren<SortingGroup>().sortingOrder = Info.SortingOrder(spawnedObj.transform.position.y);
        }
    }

    private bool CheckIfFloor(TileType tile)
    {
        if (tile == TileType.Middle || tile == TileType.Corridor) return true;
        return false;
    }
}


public class RoomGrid
{
    public TileType tileType;
    public Vector2Int objectSize;
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
    Nothing,
    Corridor,
    Object,
    ObjectPlace,
    Background
}
public enum ExitType
{
    Top,
    Right,
    Bottom,
    Left,
}
public enum RoomType
{
    Normal,
    Start,
    Boss
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
    public RoomType roomType;
}
