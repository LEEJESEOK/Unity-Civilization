using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_Grid : MonoBehaviour
{
    //public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public JKH_Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;

    //
    public GameObject environment;
    TerrainData[] terrainDatas;
    //

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        terrainDatas = environment.GetComponentsInChildren<TerrainData>();

        CreateGrid();
    }

    private void CreateGrid()
    {
        #region test
        JKH_Move move = GetComponent<JKH_Move>();
        #endregion
        grid = new JKH_Node[gridSizeX, gridSizeY];
        //Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 -
        //    Vector3.forward * gridWorldSize.y / 2;
        //for (int x = 0; x < gridSizeX; x++)
        //{
        //    for (int y = 0; y < gridSizeX; y++)
        //    {
        //        //기존에 있던것@@@@
        //        Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) +
        //            Vector3.forward * (y * nodeDiameter + nodeRadius);
        //        bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius,unwalkableMask)); //
        //        //grid[x, y] = new JKH_Node(walkable, worldPoint);
        //        grid[x, y] = new JKH_Node(walkable, worldPoint, x, y);


        //        //새로 만든것@@@@@@@
        //        //TerrainData에있는 layer 정보 넣는다(walkable, unwalkable).



        //    }
        //}
        for (int i = 0; i < terrainDatas.Length; i++)
        {
            TerrainType terrainType = terrainDatas[i].terrainType;
            bool walkable = false;
            //gameObject 대신 넣어야하는게 그 정보인데..? 어떻게 너ㅎ음 ㅠㅠ
            if (terrainType == TerrainType.Mountain ||
                terrainType == TerrainType.Coast ||
                terrainType == TerrainType.Ocean)
            {
                walkable = false;
            }
            else
            {
                walkable = true;
            }
            //TerrainData에있는 x,y 좌표 넣는다.
            int x = terrainDatas[i].x;
            int y = terrainDatas[i].y;

            grid[x, y] = new JKH_Node(walkable, terrainDatas[i].transform.position, x, y);
        }
    }

    #region 노트
    //Note Todo..
    //6각형으로 바꾸어야한다
    //추가된 좌표 (x-1, y-1) (x+1, y-1)
    //
    #endregion


    //들고 가야할것.
    //좌표 기준으로 계산한다.
    //?: currentNode기준 주변좌표 계산. 여기서도 사용되지않나..
    public List<JKH_Node> GetNeighbours(JKH_Node node)
    {
        List<JKH_Node> neighbours = new List<JKH_Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == 0 && y == 0) || (x == -1 && y == -1) || (x == 1 && y == -1))
                {
                    //체크 안해도되는 버릴것.
                    continue;
                }
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    //Bring Neighbours /위에거 건드려본거임. 
    public List<JKH_Node> GetNeighboursAdd(JKH_Node node)
    {
        List<JKH_Node> neighbours = new List<JKH_Node>();

        RaycastHit hitInfo;
        Quaternion originRotation = transform.rotation;

        //검사
        for (int i = 0; i < 6; i++)
        {
            Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
            var pos = node.worldPosition;
            pos.y = -0.95f;
            Ray ray = new Ray(pos, rotation * transform.forward);

            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                if (hitInfo.transform.gameObject.tag == "Map")
                {
                    //해당 x,y좌표 저장
                    neighbours.Add(grid[hitInfo.transform.GetComponent<TerrainData>().x, hitInfo.transform.GetComponent<TerrainData>().y]);
                }
            }
        }
        return neighbours;
    }

    //들고가야할것@@@@@
    public JKH_Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<JKH_Node> path;

    //void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

    //    if (grid != null)
    //    {
    //        foreach (JKH_Node n in grid)
    //        {
    //            Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //            if (path != null)
    //                if (path.Contains(n))
    //                    Gizmos.color = Color.black;
    //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
    //        }
    //    }
    //}
}
