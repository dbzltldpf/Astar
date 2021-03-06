using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class PathFinding : MonoBehaviour
{
    private Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();
    public Tilemap tilemap;

    public List<Node> pathNodes;

    private void Awake()
    {
        pathNodes = new List<Node>();
        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Node node = new Node(pos);
                nodes.Add(pos, node);
            }
        }
    }

    
    public void FindPath(Vector3 startPos, Vector3 targetPos, Action<Stack<Node>, bool> callback)
    {
        Node startNode = GetNodeFromPosition(startPos);
        Node targetNode = GetNodeFromPosition(targetPos);

        Vector3Int end = tilemap.WorldToCell(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].F < currentNode.F || (openSet[i].F == currentNode.F && openSet[i].H < currentNode.H))
                {
                    currentNode = openSet[i];
                }

            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                callback(GetPath(startNode, targetNode), true);
                pathNodes.AddRange(GetPath(startNode, targetNode));
                return;
            }

            foreach (var neighbourNode in GetNeighbours(currentNode.Position))
            {
                if (closeSet.Contains(neighbourNode))
                {
                    continue;
                }

                int newNeighbourNodeGvalue = currentNode.G + GetDistance(currentNode, neighbourNode);

                if (openSet.Contains(neighbourNode))
                {
                    if (newNeighbourNodeGvalue < neighbourNode.G)
                    {
                        neighbourNode.G = newNeighbourNodeGvalue;
                        neighbourNode.Parent = currentNode;
                    }
                }
                else
                {
                    neighbourNode.G = newNeighbourNodeGvalue;
                    neighbourNode.Parent = currentNode;
                    neighbourNode.H = GetDistance(neighbourNode, nodes[end]);
                    openSet.Add(neighbourNode);
                }
            }
        }



        callback(new Stack<Node>(), false);

    }

     private List<Node> GetNeighbours(Vector3Int currentNode)
     {
         List<Node> neighbours = new List<Node>();
    
         for (int x = -1; x <= 1; x++)
         {
             for (int y = -1; y <= 1; y++)
             {
                 if (x != 0 || y != 0)
                 {
                     Vector3Int neighbourPos = new Vector3Int(currentNode.x - x, currentNode.y - y, currentNode.z);
    
                     if (nodes.ContainsKey(neighbourPos))
                     {
                         neighbours.Add(nodes[neighbourPos]);
                     }
                 }
             }
    
         }
    
         return neighbours;
     }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int distY = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
    
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }

    public Node GetNodeFromPosition(Vector3 worldPosition)
    {
        Vector3Int pos = tilemap.WorldToCell(worldPosition);
        return nodes[pos];
    }

     private Stack<Node> GetPath(Node startNode, Node targetNode)
     {
         Stack<Node> path = new Stack<Node>();
    
         Node currentNode = targetNode;
         path.Push(currentNode);
    
         while (currentNode != startNode)
         {
             currentNode = currentNode.Parent;
             path.Push(currentNode);
         }
    
         return path;
     }

    private void OnDrawGizmos()
    {
        if (pathNodes.Count > 0)
        {
            foreach (var node in pathNodes)
            {
                Gizmos.color = Color.black * 0.5f;

                Gizmos.DrawCube(node.Position + new Vector3(0.5f, 0.5f, 0f), Vector3.one * 1);
            }
        }
    }
}
