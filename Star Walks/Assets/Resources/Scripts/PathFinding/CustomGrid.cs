using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour {

    public bool onlyDisplayPathGizmos;
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float gapBetweenNodes;

    Node[,] nodeArray;
    public List<Node> FinalPath;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    
    public int GetGridSize { get => gridSizeX * gridSizeY; }
    public int GetGridSizeX { get => gridSizeX; }
    public int GetGridSizeY { get => gridSizeY; }

    private void Awake() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid() {
        nodeArray = new Node[gridSizeX, gridSizeY];
        Vector2 bottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector2 nodeWorldPosition = bottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool wall = false;

                if (Physics.CheckSphere(nodeWorldPosition, nodeRadius, obstacleMask)) {
                    wall = true;
                }

                nodeArray[x, y] = new Node(wall, nodeWorldPosition, x, y);

            }
        }
    }

    public List<Node> GetNeighbourNodes(Node _node) {
        List<Node> NeighbourNodes = new List<Node>();

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                //if we are on the node tha was passed in, skip this iteration.
                if (x == 0 && y == 0) {
                    continue;
                }

                int checkX = _node.gridX + x;
                int checkY = _node.gridY + y;

                //Make sure the node is within the grid.
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    NeighbourNodes.Add(nodeArray[checkX, checkY]); //Adds to the neighbours list.
                }

            }
        }

        return NeighbourNodes;
    }

    public Node NodeFromWorldPosition(Vector3 _worldPosition) {
        float xPosition = ((_worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float yPosition = ((_worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y);

        xPosition = Mathf.Clamp01(xPosition);
        yPosition = Mathf.Clamp01(yPosition);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPosition);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPosition);

        return nodeArray[x, y];
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (onlyDisplayPathGizmos) {
            if (FinalPath != null) {
                foreach (Node node in FinalPath) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - gapBetweenNodes));
                }
            }
        } else {
            if (nodeArray != null) {
                foreach (Node node in nodeArray) {
                    if (node.isWall) {
                        Gizmos.color = Color.yellow;
                    } else {
                        Gizmos.color = Color.white;
                    }
                    if (FinalPath != null) {
                        if (FinalPath.Contains(node)) {
                            Gizmos.color = Color.black;
                        }
                    }
                    Gizmos.DrawCube(node.position, Vector3.one * (nodeDiameter - gapBetweenNodes));
                }
            }
        }
    }
}