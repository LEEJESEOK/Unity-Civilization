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
        //길 찾는다
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //시작값, 타겟값
        JKH_Node startNode = grid.NodeFromWorldPoint(startPos);
        JKH_Node targetNode = grid.NodeFromWorldPoint(targetPos);
        

        //openSet
        List<JKH_Node> openSet = new List<JKH_Node>();
        //closeSet HashSet?
        HashSet<JKH_Node> closeSet = new HashSet<JKH_Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //시작지점 openSet[0]
            JKH_Node currentNode = openSet[0];
            //1부터 openSet최대 갯수만큼
            for (int i = 1; i < openSet.Count; i++)
            {
                //i번쨰 fCost가 현재 fCost보다 작거나
                if ((openSet[i].fCost < currentNode.fCost)
                    //openSet과 현재노드의 fCost가 같고, 현재노드의 hCost가 더 크다면?
                    || (openSet[i].fCost == currentNode.fCost)
                    && (openSet[i].hCost < currentNode.hCost))
                {
                    //i번쨰 openSet은 현재 노드
                    currentNode = openSet[i];
                }
            }
            //현재노드의 openSet 지우고 
            openSet.Remove(currentNode);
            //closeSet은 추가한다
            closeSet.Add(currentNode);

            //목표지점에 도달했다면
            if (currentNode == targetNode)
            {
                //이거뭐냐 
                RetracePath(startNode, targetNode);
                return;
            }

            //현재노드의 이웃들 검사한다
            foreach (JKH_Node neighbour in grid.GetNeighbours(currentNode))
            {
                //걸을수없는 위치거나, 이웃이 closeSet에 있다면 
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                {
                    continue;
                }
                //g(x)+ 현재노드와 이웃간의 거리
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //만약 이웃의 gCost가 더 크거나 이웃이 포함되어있지 않다면
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //gCost갱신
                    neighbour.gCost = newCostToNeighbour;
                    //hCost갱신
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    //이웃을포함하지않는다면
                    if (!openSet.Contains(neighbour))
                    {
                        //이웃을 추가한다
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
        // |X|,|Y|
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //대각선이동
        //만약 X가 Y보다 크다면
        if (dstX > dstY)
            //Y 만큼 대각선 이동, X-Y만큼 직선이동
            return 14 * dstY + 10 * (dstX - dstY);
        //반대로
        return 14 * dstX + 10 * (dstY - dstX);



        //+++++6각형인경우 변의길이가 10일때, 각 거리는 10√3이다.+++++
    }
}
