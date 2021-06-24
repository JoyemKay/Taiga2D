using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AstarGrid),typeof(AstarPathRequestManager))]
public class AstarPathfinding : MonoBehaviour {
    AstarNode startLastNode;

    AstarPathRequestManager requestManager;
    AstarGrid grid;

    void Awake() {
        requestManager = GetComponent<AstarPathRequestManager>();
        grid = GetComponent<AstarGrid>();
    }

    public void NullGrid(){
        grid.NullGrid();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        grid.CreateGrid(startPos, targetPos);
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        AstarNode startNode = grid.NodeFromWorldPoint(startPos);
        AstarNode targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable) {
            Heap<AstarNode> openSet = new Heap<AstarNode>(grid.MaxSize);
            HashSet<AstarNode> closedSet = new HashSet<AstarNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0) {
                AstarNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode) {
                    startLastNode = startNode;
                    pathSuccess = true;

                    break;
                }

                foreach (AstarNode neighbour in grid.GetNeighbours(currentNode)) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) { continue; }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {

                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour)) { openSet.Add(neighbour); }
                        else { openSet.UpdateItem(neighbour); }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(AstarNode startNode, AstarNode endNode) {
        List<AstarNode> path = new List<AstarNode>();
        AstarNode currentNode = endNode;

        while(currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Debug.Log("Found path, simplifying from :" + path.Count + " nodes.");
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<AstarNode> path) {
        List <Vector3>  waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {

            Vector2 directionNew = new Vector2(path[i - 1].gridPos.x - path[i].gridPos.x, path[i - 1].gridPos.y - path[i].gridPos.y);
            if(directionNew != directionOld) {
                waypoints.Add(path[i-1].worldPosition);
            }
            directionOld = directionNew;
        }
        Debug.Log("Path simplified, number of nodes: " + waypoints.Count);
        return waypoints.ToArray();
    }

    int GetDistance(AstarNode nodeA, AstarNode nodeB) {
        int distX = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int distY = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);

        if(distX > distY){
            return (14 * distY + 10 * (distX - distY));
        }
        return (14 * distX + 10 * (distY - distX));
    }
}
