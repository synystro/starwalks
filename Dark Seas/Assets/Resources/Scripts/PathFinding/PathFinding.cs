using System.Collections.Generic;
using UnityEngine;

namespace DarkSeas {
    public class PathFinding : MonoBehaviour {
        public Transform thisTransform;
        public Transform targetTransform;

        public List<Node> FinalPath;

        private CustomGrid grid;
        private Ship ship;

        private void Start() {

            // set this grid's ship
            ship = GetComponentInParent<Ship>();
            // get grid from ship
            grid = ship.pathGrid;

            thisTransform = this.transform;

        }

        private void Update() {

            if (targetTransform != null && ship.pathGrid.IsGenerated)
                FindPath(thisTransform.position, targetTransform.position);

        }

        public bool FindPath(Vector3 _startPosition, Vector3 _targetPosition) {

            // rount target position to int
            _targetPosition.x = Mathf.RoundToInt(_targetPosition.x);
            _targetPosition.y = Mathf.RoundToInt(_targetPosition.y);

            // set ship's position offset
            _startPosition -= ship.transform.position;
            _targetPosition -= ship.transform.position;

            Node startNode = grid.NodeFromWorldPosition(_startPosition);
            Node targetNode = grid.NodeFromWorldPosition(_targetPosition);

            Heap<Node> OpenList = new Heap<Node>(grid.MaxSize);
            HashSet<Node> ClosedList = new HashSet<Node>();

            OpenList.Add(startNode);

            while (OpenList.Count > 0) {
                Node currentNode = OpenList.RemoveFirst();
                ClosedList.Add(currentNode);

                if (currentNode == targetNode) {
                    GetFinalPath(startNode, targetNode);
                    break;
                }

                foreach (Node neighbourNode in grid.GetNeighbourNodes(currentNode)) {
                    if (!neighbourNode.isWalkable || ClosedList.Contains(neighbourNode)) {
                        continue;
                    }

                    int moveCost = currentNode.gCost + GetManhattanDistance(currentNode, neighbourNode);
                    if (moveCost < neighbourNode.gCost || !OpenList.Contains(neighbourNode)) {
                        neighbourNode.gCost = moveCost;
                        neighbourNode.hCost = GetManhattanDistance(neighbourNode, targetNode);
                        neighbourNode.Parent = currentNode;

                        if (!OpenList.Contains(neighbourNode)) {
                            OpenList.Add(neighbourNode);
                        }
                    }
                }

            }

            return targetNode.isWalkable ? true : false;

        }

        void GetFinalPath(Node _startNode, Node _endNode) {

            FinalPath = new List<Node>();
            Node currentNode = _endNode;

            while (currentNode != _startNode) {
                FinalPath.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            FinalPath.Reverse();

            grid.FinalPath = FinalPath;
        }

        int GetManhattanDistance(Node _nodeA, Node _nodeB) {
            int iX = Mathf.Abs(_nodeA.gridX - _nodeB.gridX);
            int iY = Mathf.Abs(_nodeA.gridY - _nodeB.gridY);

            return iX + iY;
        }

    }
}
