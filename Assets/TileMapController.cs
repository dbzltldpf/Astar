using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapController : MonoBehaviour
{
    public Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();
   
    public Tilemap tilemap;
    public Tilemap tilemap2;


    public Transform enemy; // Start
    public Transform target; // End

    public Tilemap colorTileMap;
    public Tile colorTile;
    public Color pathColor;

    private void Start()
    {

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Node node = new Node(pos, true);
                nodes.Add(pos, node);
            }
        }

        foreach (var pos in tilemap2.cellBounds.allPositionsWithin)
        {
            if (tilemap2.HasTile(pos))
            {
                nodes[pos].Walkable = false;
            }
        }

    }

 
    public void FindPath()
    {
        Vector3Int start = tilemap.WorldToCell(enemy.position);
        Vector3Int end = tilemap.WorldToCell(target.position);

        StartCoroutine(GetPath(start, end));
    }

    IEnumerator GetPath(Vector3Int start, Vector3Int end)
    {

        Stack<Node> wayNodes = new Stack<Node>();
        bool pathSuccess = false;
 
        Heap<Node> openSet = new Heap<Node>(nodes.Count);
        HashSet<Node> closeSet = new HashSet<Node>();

        openSet.Add(nodes[start]);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closeSet.Add(currentNode);
            //(1,2,0)
            if (currentNode == nodes[end])
            {
                
                pathSuccess = true;
                break;
            }
     
            foreach (var neighbourNode in GetNeighbours(currentNode.Position))
            {
                

                if (closeSet.Contains(neighbourNode) || neighbourNode.Walkable == false)
                {
                    continue;
                }

                int newNeighbourNodeGvalue = currentNode.G + GetDistance(currentNode, neighbourNode);

                if (!openSet.Contains(neighbourNode) || newNeighbourNodeGvalue < neighbourNode.G)
                {
                    neighbourNode.G = newNeighbourNodeGvalue;
                    neighbourNode.Parent = currentNode;

                    if (!openSet.Contains(neighbourNode))
                    {
                        openSet.Add(neighbourNode);
                     
                        neighbourNode.H = GetDistance(neighbourNode, nodes[end]);
                    }
                }
            }
        }

        if (pathSuccess)
        {
            wayNodes = GetPath(nodes[start], nodes[end]);
           
        }
       
        FinishedProcessingPath(wayNodes, pathSuccess);

        yield return null;
    }

   

    private List<Node> GetNeighbours(Vector3Int parentPosition)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 || y != 0)
                {
                    Vector3Int neighbourPos = new Vector3Int(parentPosition.x - x, parentPosition.y - y, parentPosition.z);

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

    private void FinishedProcessingPath(Stack<Node> path, bool Successed)
    {
        if (Successed)
        {
            while (path.Count > 0)
            {
                Node node = path.Pop();
                ColorTile(node.Position, pathColor);
              
            }
        }
    }

    private void ColorTile(Vector3Int position, Color color)
    {
        colorTileMap.SetTile(position, colorTile);
        colorTileMap.SetTileFlags(position, TileFlags.None);
        colorTileMap.SetColor(position, color);
    }

}