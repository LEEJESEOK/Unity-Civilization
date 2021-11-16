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
        //�� ã�´�
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //���۰�, Ÿ�ٰ�
        JKH_Node startNode = grid.NodeFromWorldPoint(startPos);
        JKH_Node targetNode = grid.NodeFromWorldPoint(targetPos);
        

        //openSet
        List<JKH_Node> openSet = new List<JKH_Node>();
        //closeSet HashSet?
        HashSet<JKH_Node> closeSet = new HashSet<JKH_Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //�������� openSet[0]
            JKH_Node currentNode = openSet[0];
            //1���� openSet�ִ� ������ŭ
            for (int i = 1; i < openSet.Count; i++)
            {
                //i���� fCost�� ���� fCost���� �۰ų�
                if ((openSet[i].fCost < currentNode.fCost)
                    //openSet�� �������� fCost�� ����, �������� hCost�� �� ũ�ٸ�?
                    || (openSet[i].fCost == currentNode.fCost)
                    && (openSet[i].hCost < currentNode.hCost))
                {
                    //i���� openSet�� ���� ���
                    currentNode = openSet[i];
                }
            }
            //�������� openSet ����� 
            openSet.Remove(currentNode);
            //closeSet�� �߰��Ѵ�
            closeSet.Add(currentNode);

            //��ǥ������ �����ߴٸ�
            if (currentNode == targetNode)
            {
                //�̰Ź��� 
                RetracePath(startNode, targetNode);
                return;
            }

            //�������� �̿��� �˻��Ѵ�
            foreach (JKH_Node neighbour in grid.GetNeighbours(currentNode))
            {
                //���������� ��ġ�ų�, �̿��� closeSet�� �ִٸ� 
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                {
                    continue;
                }
                //g(x)+ ������� �̿����� �Ÿ�
                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                //���� �̿��� gCost�� �� ũ�ų� �̿��� ���ԵǾ����� �ʴٸ�
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //gCost����
                    neighbour.gCost = newCostToNeighbour;
                    //hCost����
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    //�̿������������ʴ´ٸ�
                    if (!openSet.Contains(neighbour))
                    {
                        //�̿��� �߰��Ѵ�
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

        //�밢���̵�
        //���� X�� Y���� ũ�ٸ�
        if (dstX > dstY)
            //Y ��ŭ �밢�� �̵�, X-Y��ŭ �����̵�
            return 14 * dstY + 10 * (dstX - dstY);
        //�ݴ��
        return 14 * dstX + 10 * (dstY - dstX);



        //+++++6�����ΰ�� ���Ǳ��̰� 10�϶�, �� �Ÿ��� 10��3�̴�.+++++
    }
}
