using UnityEngine;
using System;
using System.Collections;

public class Node : IHeapItem<Node> {

    public int gridX;
    public int gridY;

    public bool isWall;
    public Vector3 position;

    public Node Parent;

    public int gCost;
    public int hCost;

    public int fCost { get { return gCost + hCost; } }

    private int heapIndex;

    public Node(bool _isWall, Vector2 _position, int _gridX, int _gridY) {
        isWall = _isWall;
        position = _position;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(object obj) {
        Node nodeToCompare = obj as Node;
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

}