using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public LayerMask walkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public int obstacleProximityPenalty = 10;
    private Vector2 pos;
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    [HideInInspector]
    public Node[,] grid;

    float nodeDiameter;
    [HideInInspector]
    public int gridSizeX, gridSizeY;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;


    public void InitializeGrid(float x, float y, float maxX, float maxY)
    {
        nodeDiameter = nodeRadius * 2;
        gridWorldSize.x = maxX;
        gridWorldSize.y = maxY;
        transform.position = new Vector2(x, y);
        pos = transform.position;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        var bgc = References.rf.levelGenerator.backgroundCorners;
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        RoomGrid[,] rooms = References.rf.levelGenerator.roomGrid;
        Tilemap temp = References.rf.levelGenerator.backgroundCorners;
        Vector2Int roomGridSize = new Vector2Int(References.rf.levelGenerator.worldSizeX, References.rf.levelGenerator.worldSizeY);
        Vector2 multiplier = new Vector2(gridSizeX / roomGridSize.x, gridSizeY / roomGridSize.y);

        //Initialize grid with empty nodes
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                grid[x, y] = new Node(0, worldPoint, x, y, 0);
            }
        }
        // Set floor
        for (int x = 0; x < roomGridSize.x; x++)
        {
            for (int y = 0; y < roomGridSize.y; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                TileType tile = rooms[x, y].tileType;
                if (tile == TileType.Middle || tile == TileType.Corridor || tile == TileType.Object)
                {
                    int lX = Mathf.RoundToInt(x * multiplier.x);
                    int lY = Mathf.RoundToInt(y * multiplier.y);
                    grid[lX - 1, lY - 1].walkable = 2;
                    grid[lX - 1, lY].walkable = 2;
                    grid[lX - 1, lY + 1].walkable = 2;
                    grid[lX, lY - 1].walkable = 2;
                    grid[lX, lY].walkable = 2;
                    grid[lX, lY + 1].walkable = 2;
                    grid[lX + 1, lY - 1].walkable = 2;
                    grid[lX + 1, lY].walkable = 2;
                    grid[lX + 1, lY + 1].walkable = 2;
                }
            }
        }
        // Find and walls and set appropriate unwalkable nodes
        for (int x = 0; x < roomGridSize.x; x++)
        {
            for (int y = 0; y < roomGridSize.y; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                int walkable = 0;
                int lX = Mathf.RoundToInt(x * multiplier.x);
                int lY = Mathf.RoundToInt(y * multiplier.y);

                TileType tile = rooms[x, y].tileType;
                var bCorner = bgc.GetTile(new Vector3Int(x, y, 0));
                if (bCorner != null)
                {
                    if (bCorner.name == "BigCornerTop")
                    {
                        var flipped = bgc.GetTransformMatrix(new Vector3Int(x, y, 0));
                        if (flipped.lossyScale.x == 1)
                        {
                            grid[lX, lY].walkable = 1;
                            grid[lX, lY - 1].walkable = 1;
                            grid[lX - 1, lY - 1].walkable = 1;
                            grid[lX - 1, lY].walkable = 0;
                            grid[lX - 1, lY + 1].walkable = 0;
                            continue;
                        }
                        else
                        {
                            grid[lX + 1, lY].walkable = 1;
                            grid[lX + 1, lY + 1].walkable = 1;
                            continue;
                        }

                    }
                }

                if (tile == TileType.Middle || tile == TileType.Corridor) continue;
                else if (rooms[x, y].tileType == TileType.Nothing) walkable = 0;
                else walkable = 1;
                switch (tile)
                {
                    case TileType.Left:
                        grid[lX, lY + 1].walkable = walkable;
                        grid[lX, lY - 1].walkable = walkable;
                        grid[lX, lY].walkable = walkable;
                        break;
                    case TileType.Right:
                        grid[lX, lY].walkable = 2;
                        grid[lX + 1, lY].walkable = walkable;
                        grid[lX + 1, lY + 1].walkable = walkable;
                        grid[lX + 1, lY - 1].walkable = walkable;
                        grid[lX - 1, lY].walkable = 2;
                        grid[lX - 1, lY - 1].walkable = 2;
                        grid[lX, lY - 1].walkable = 2;
                        break;
                    case TileType.Bottom:
                        grid[lX, lY].walkable = 2;
                        grid[lX - 1, lY].walkable = 2;
                        grid[lX, lY - 1].walkable = walkable;
                        grid[lX - 1, lY - 1].walkable = walkable;
                        grid[lX + 1, lY - 1].walkable = walkable;
                        break;
                    case TileType.TopTwo:
                        grid[lX, lY].walkable = 2;
                        grid[lX - 1, lY].walkable = 2;
                        grid[lX + 1, lY].walkable = 2;
                        grid[lX - 1, lY - 1].walkable = 2;
                        grid[lX, lY - 1].walkable = 2;
                        grid[lX + 1, lY - 1].walkable = 2;
                        grid[lX - 1, lY + 1].walkable = walkable;
                        grid[lX, lY + 1].walkable = walkable;
                        break;
                    case TileType.Top:
                        grid[lX, lY].walkable = 0;
                        break;
                    case TileType.TopLeftTwo:
                        grid[lX, lY].walkable = 1;
                        grid[lX, lY + 1].walkable = 1;
                        break;
                    case TileType.TopRightTwo:
                        grid[lX, lY].walkable = 2;
                        grid[lX, lY - 1].walkable = 2;
                        grid[lX - 1, lY + 1].walkable = 1;
                        grid[lX, lY + 1].walkable = 1;
                        grid[lX + 1, lY + 1].walkable = 1;
                        grid[lX + 1, lY].walkable = 1;
                        break;
                    case TileType.TopRightExtra:
                        grid[lX - 1, lY - 1].walkable = 1;
                        grid[lX, lY - 1].walkable = 1;
                        grid[lX, lY - 2].walkable = 2;
                        break;
                    case TileType.TopLeftExtra:
                        grid[lX, lY - 1].walkable = 1;
                        break;
                    case TileType.BottomLeft:
                        grid[lX, lY].walkable = walkable;
                        grid[lX, lY - 1].walkable = walkable;
                        grid[lX, lY + 1].walkable = walkable;
                        grid[lX + 1, lY - 1].walkable = walkable;
                        break;
                    case TileType.BottomRight:
                        grid[lX, lY].walkable = 2;
                        grid[lX - 1, lY].walkable = 2;
                        grid[lX, lY - 1].walkable = walkable;
                        grid[lX + 1, lY].walkable = walkable;
                        grid[lX + 1, lY - 1].walkable = walkable;
                        break;



                }



            }
        }
        // Find all floor objects (boxes, shelves etc.) and set appropriate unwalkable nodes for them
        for (int x = 0; x < roomGridSize.x; x++)
        {
            for (int y = 0; y < roomGridSize.y; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                TileType tile = rooms[x, y].tileType;
                int lX = Mathf.RoundToInt(x * multiplier.x);
                int lY = Mathf.RoundToInt(y * multiplier.y);
                if (tile == TileType.Object)
                {
                    Vector2Int tileSize = rooms[x, y].objectSize;
                    for (int oX = 0; oX < tileSize.x; oX++)
                    {
                        for (int oY = 0; oY < tileSize.y; oY++)
                        {
                            grid[oX + lX, oY + lY].walkable = 1;
                        }
                    }
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node GetWalkableNeighbor(Node targetNode)
    {
        if (targetNode.walkable == 2) return targetNode;
        Node tempNode = targetNode;
        int count = 0;
        while (tempNode.walkable < 2)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    int posX = targetNode.gridX + (count * x);
                    int posY = targetNode.gridY + (count * y);
                    posX = Mathf.Clamp(posX, 0, gridSizeX);
                    posY = Mathf.Clamp(posY, 0, gridSizeY);
                    tempNode = grid[posX, posY];
                    if (tempNode.walkable == 2) break;
                }
                if (tempNode.walkable == 2) break;
            }
            count++;
        }
        return tempNode;
    }
    public Vector2 WorldPointFromNode(Node node)
    {
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        Vector2 position = worldBottomLeft + Vector2.right * (node.gridX * nodeDiameter) + Vector2.up * (node.gridY * nodeDiameter);
        return position;
    }
    public Vector2 WorldPointFromNode(int x, int y)
    {
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        Vector2 position = worldBottomLeft + Vector2.right * (x * nodeDiameter) + Vector2.up * (y * nodeDiameter);
        return position;
    }
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = ((worldPosition.x - pos.x - nodeRadius) + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = ((worldPosition.y - pos.y - nodeRadius) + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt(gridSizeX * percentX);
        int y = Mathf.RoundToInt(gridSizeY * percentY);

        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1f));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                if (n.walkable > 0)
                {
                    Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                    Gizmos.color = n.walkable == 2 ? Gizmos.color : Color.red;
                    Gizmos.DrawCube(n.worldPosition, Vector2.one * (nodeDiameter));
                }
            }
        }
    }
}