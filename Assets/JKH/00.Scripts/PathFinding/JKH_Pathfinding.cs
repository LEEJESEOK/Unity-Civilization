using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JKH_Pathfinding : MonoBehaviour
{
    public Transform seeker, target;
    JKH_Grid grid;
    private void Awake()
    {
        grid = GetComponent<JKH_Grid>();
    }

    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //시작값, 타겟값
        JKH_Node startNode = grid.NodeFromWorldPoint(startPos);
        JKH_Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<JKH_Node> openSet = new List<JKH_Node>();
        HashSet<JKH_Node> closeSet = new HashSet<JKH_Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            JKH_Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if ((openSet[i].fCost < currentNode.fCost)
                    || (openSet[i].fCost == currentNode.fCost)
                    && (openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (JKH_Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                {
                    continue;
                }
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);

                    }
                }


            }
        }
    }

    void RetracePath(JKH_Node startNode, JKH_Node endNode)
    {
        List<JKH_Node> path = new List<JKH_Node>();
        JKH_Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

    }

    int GetDistance(JKH_Node nodeA, JKH_Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
