using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AstarGrid : MonoBehaviour
{

    public bool displayGridGizmos;
    public LayerMask[] unwalkableMasks;
    public Vector2 gridWorldSize;
    public float nodeRadius, checkRadius, gridMargin;
    AstarNode[,] grid;
    float nodeDiameter;
    Vector2Int gridSize;

    void Awake(){
        nodeDiameter = nodeRadius * 2;
    }

    public int MaxSize{
        get{
            return gridSize.x * gridSize.y;
        }
    }

    public void CreateGrid(Vector3 from, Vector3 to){

            gridWorldSize = new Vector2(Mathf.Abs(from.x-to.x) + gridMargin, 
                                        Mathf.Abs(from.y-to.y) + gridMargin);
            transform.position = Vector2.Lerp(from,to,0.5f);
            gridSize = new Vector2Int(
                Mathf.RoundToInt((gridWorldSize.x / nodeDiameter)),
                Mathf.RoundToInt((gridWorldSize.y / nodeDiameter)));
            
        grid = new AstarNode[gridSize.x,gridSize.y];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                bool walkable = true;
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                foreach (LayerMask mask in unwalkableMasks){
                    walkable = !Physics2D.OverlapCircle(worldPoint, checkRadius, mask);
                    if (!walkable) { break; }
                }
                grid[x, y] = new AstarNode(walkable, worldPoint, new Vector2Int(x,y));
            }
        }
    }

    public void NullGrid(){
        grid = null;
        gridWorldSize = Vector3.zero;
        transform.position = Vector3.zero;
    }

    public List<AstarNode> GetNeighbours(AstarNode node){
        List<AstarNode> neighbours = new List<AstarNode>();

        for (int x = -1; x <= 1; x++){
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) { continue; }

                int checkX = node.gridPos.x + x;
                int checkY = node.gridPos.y + y;

                if(checkX >= 0 && checkX < gridSize.x && checkY >= 0 && checkY < gridSize.y){
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public AstarNode NodeFromWorldPoint(Vector3 worldPosition){
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y - transform.position.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null && displayGridGizmos) {
            foreach (AstarNode n in grid) {
                Gizmos.color = (n.walkable) ? new Color(1, 1, 1, 0.5f) : new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(n.worldPosition, Vector3.one * nodeDiameter * 0.9f);
            }
        }
    }
}
