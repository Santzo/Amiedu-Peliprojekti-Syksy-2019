using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class Pathfinding {

    Random random = new System.Random();
	public Vector2[] StartFindPath(Grid grid, Vector2 startPos, Vector2 targetPos) {
		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable < 2)
        {
            startNode = grid.GetWalkableNeighbor(startNode);
        }
        if (targetNode.walkable < 2)
        {
            targetNode = grid.GetWalkableNeighbor(targetNode);
        }
        startNode.parent = startNode;
		
		
		if (startNode.walkable == 2 && targetNode.walkable == 2) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
            while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				if (currentNode == targetNode) {
                    pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (neighbour.walkable < 2|| closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else 
							openSet.UpdateItem(neighbour);
					}
				}
			}
		}

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        return waypoints;
        
	}
		
	
	Vector2[] RetracePath(Node startNode, Node endNode) {
		List<Vector2> path = new List<Vector2>();
		Node currentNode = endNode;
        Vector2 checkPath = endNode.worldPosition;
        Vector2 curPath, updatedPath = Vector2.zero;
        path.Add(checkPath);
        while (currentNode != startNode) {
            curPath = checkPath - currentNode.worldPosition;
            if (curPath != updatedPath && checkPath != endNode.worldPosition || currentNode.parent == startNode)
            {
                Vector2 pathOffset = new Vector2(RandomNumber(-0.1f, 0.1f), RandomNumber(-0.1f, 0.1f));
                path.Add(checkPath);
                updatedPath = curPath;
            }
            checkPath = currentNode.worldPosition;
            currentNode = currentNode.parent;
        }
        
        Vector2[] readyPath = path.ToArray();
		Array.Reverse(readyPath);
		return readyPath;
	}
	
	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

    public float RandomNumber(float min, float max)
    {
        int a = random.Next(0, 100);
        float num = a * 0.01f;
        return min + (num * (max-min));

    }
}
