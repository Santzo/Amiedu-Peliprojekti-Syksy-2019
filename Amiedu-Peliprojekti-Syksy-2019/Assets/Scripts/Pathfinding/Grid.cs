﻿using UnityEngine;
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
        Destroy(GameObject.Find("FloorTilemap").GetComponent<TilemapCollider2D>());
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
        Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                var results = Physics2D.OverlapCircleAll(worldPoint, nodeRadius,unwalkableMask | walkableMask);
                int walkable = 0;
                foreach (var item in results)
                {
                    if (item.gameObject.layer == 16 || item.gameObject.layer == 21)
                    {
                        walkable = 1;
                        break;
                    }
                    if (item.gameObject.layer == 11)
                    {
                        walkable = 2;
                    }
                }

                int movementPenalty = 0;
                var _ray = Physics2D.Raycast(worldPoint, Vector2.zero, unwalkableMask);
                if (_ray)
                {
                    walkableRegionsDictionary.TryGetValue(_ray.collider.gameObject.layer, out movementPenalty);
                }

                if (walkable == 1)
                {
                    movementPenalty += obstacleProximityPenalty;
                }


                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);

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
        Vector2 position = worldBottomLeft + Vector2.right * (node.gridX * nodeDiameter + nodeRadius) + Vector2.up * (node.gridY * nodeDiameter + nodeRadius);
        return position;
    }
    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = ((worldPosition.x - pos.x) + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = ((worldPosition.y - pos.y) + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

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