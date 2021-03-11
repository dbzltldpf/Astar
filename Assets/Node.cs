using UnityEngine;
using System;

[Serializable]
public class Node : IHeapItem<Node>
{
    public bool Walkable { get; set; }

    public int G { get; set; }
    public int H { get; set; }
    public int F { get { return G + H; } }

    public Node Parent { get; set; }
    public Vector3Int Position { get; set; }

    public Node(Vector3Int position, bool walkable)
    {
        Position = position;
        Walkable = walkable;
    }

    public int HeapIndex { get; set; }

    public int CompareTo(Node other)
    {
        //int 값이 작은경우 우선순위가 높음

        //-1        : Fcost <  other.Fcost (우선순위가 내것 높음)
        //0         : Fcost == other.Fcost (우선순위가 같음)
        //1         : Fcost >  other.Fcost (우선순위가 다른것이 높음)
        int compare = F.CompareTo(other.F);

        //Fcost의 우선순위가 같을경우
        if (compare == 0)
        {
            //-1        : hCost <  other.hCost (우선순위가 내것 높음)
            //0         : hCost == other.hCost (우선순위가 같음)
            //1         : hCost >  other.hCost (우선순위가 다른것이 높음)
            compare = H.CompareTo(other.H);
        }

        return compare;
    }
}