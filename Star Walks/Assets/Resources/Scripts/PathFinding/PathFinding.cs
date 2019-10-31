using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour {

    CustomGrid grid;
    public Transform thisTransform;
    public Transform targetTransform;

    public List<Node> FinalPath;

    private void Awake() {

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<CustomGrid>();

    }

    private void Start() {
        thisTransform = this.transform;
    }

    private void Update() {

        if (targetTransform != null)
            FindPath(thisTransform.position, targetTransform.position);

    }

    public void FindPath(Vector3 _startPosition, Vector3 _targetPosition) {

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
                    if (neighbourNode.isWall || ClosedList.Contains(neighbourNode)) {
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
