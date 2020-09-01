﻿using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {
    public class CustomGrid : MonoBehaviour {

        [Space(10)]
        public bool onlyDisplayPathGizmos;
        public LayerMask walkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;
        public float gapBetweenNodes;

        Node[,] nodeArray;
        public List<Node> FinalPath;

        //private Vector3 gridOffset;
        private Vector2 bottomLeft;
        private float nodeDiameter;
        private int gridSizeX, gridSizeY;

        //public Vector3 GetGridOffset { get => gridOffset; }
        public Vector2 GetBottomLeft { get => bottomLeft; }
        public int GetGridSize { get => gridSizeX * gridSizeY; }
        public int GetGridSizeX { get => gridSizeX; }
        public int GetGridSizeY { get => gridSizeY; }
        public int MaxSize { get { return gridSizeX * gridSizeY; } }

        public bool IsGenerated { get { return isGenerated; } private set {} }

        private bool isGenerated;
        private Ship ship;

        public void Generate(Ship _ship) {
            //ship = GetComponentInParent<Ship>();
            ship = _ship;

            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            //gridOffset = GridOffset();
            //gridOffset = ship.transform.position;

            CreateGrid();

            isGenerated = true;

        }

        void CreateGrid() {
            nodeArray = new Node[gridSizeX, gridSizeY];

            bottomLeft = (Vector2)ship.transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

            for (int x = 0; x < gridSizeX; x++) {
                for (int y = 0; y < gridSizeY; y++) {
                    Vector2 nodeWorldPosition = bottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                    bool isWalkable = false;

                    if (Physics.CheckSphere(nodeWorldPosition, nodeRadius, walkableMask)) {
                        isWalkable = true;
                    }

                    nodeArray[x, y] = new Node(isWalkable, nodeWorldPosition, x, y);

                }
            }
        }

        public List<Node> GetNeighbourNodes(Node _node) {
            List<Node> NeighbourNodes = new List<Node>();

            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    //if we are on the node that was passed in, skip this iteration.
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

        private Vector3 GridOffset() {

            float offsetX = 40f;
            float offsetY = 0f;

            if (GetGridSizeX % 2 == 0)
                offsetX = -0.5f;

            if (GetGridSizeY % 2 == 0)
                offsetY = 0.5f;

            return new Vector3(offsetX, offsetY, 0);

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
                        if (!node.isWalkable) {
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
}