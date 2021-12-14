using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : Singleton<MapManager>
{
    // 선택한 유닛
    public Unit selectedUnit;
    public int mapWidth, mapHeight;
    List<TerrainData> terrainDataMap;
    List<JKH_Node> nodeMap;
    //public JKH_Grid grid;
    
    //FindPath()'s variable
    float moveResult = 0;

    //path
    public List<JKH_Node> path= new List<JKH_Node>();

    //createGrid
    TerrainData[] terrainDatas;
    //public bool walkable;


    void Start()
    {
        terrainDataMap = new List<TerrainData>(GetComponentsInChildren<TerrainData>());
        InitNodeMap(); //前 createGrid

        //grid = GetComponent<JKH_Grid>();        

    
    }

    private void Awake()
    {
        //grid = new JKH_Node[gridSizeX, gridSizeY];
    }
    void Update()
    {
        getUnitInfo();
    }

    private void InitNodeMap()
    {
        for (int i = 0; i < terrainDataMap.Count; ++i)
        {
            TerrainData data = terrainDataMap[i];

            TerrainType terrainType = data.terrainType;
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

            JKH_Node node = new JKH_Node(walkable, data.transform.position ,data.x, data.y, data.output.movePower);
            nodeMap.Add(node);
        }        
    }


    // GetUnitSelect()
    // 클릭하면 레이캐스트로 충돌한 오브젝트의 Unit 컴포넌트를 받아옴
    // selectedUnit에 저장


    //유닛에대한 정보를 가져오는 함수
    public void getUnitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        LayerMask layer = LayerMask.GetMask("Unit");
        //LayerMask layer = 1 << LayerMask.NameToLayer("Unit");

        //마우스 클릭한다
        if (Input.GetButton("Fire1") && !UIManager.IsPointerOverUIObject())
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, layer))
            {

                //unitMove
                selectedUnit = hitInfo.transform.GetComponent<Unit>();

                // 유닛 정보 출력(확인용) 
                print(selectedUnit.gameObject.name);
                print("이동력: " + selectedUnit.movePower);
                print("체력: " + selectedUnit.Hp);

                //유닛이있는 타일의 정보를 가져온다.
                 

            }
        }
    }

    // FindPath
    public JKH_Node FindPath(JKH_Node start, JKH_Node end)
    {
        List<JKH_Node> openSet = new List<JKH_Node>();
        HashSet<JKH_Node> closeSet = new HashSet<JKH_Node>();
        openSet.Add(start);

        //calcualting required MovePower
        float requiredMovePower = 0;
        //subtract first Node
        requiredMovePower -= start.requiredMovePower;

        while (openSet.Count > 0)
        {
            JKH_Node currentNode = openSet[0];

            for(int i = 1; i < openSet.Count; i++)
            {
                //만족한 값으로 이동
                if ((openSet[i].fCost < currentNode.fCost)
                    || ((openSet[i].fCost == currentNode.fCost) && (openSet[i].hCost < currentNode.hCost)))
                {
                    currentNode = openSet[i];

                }
            }
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            //if (currentNode == end)
            if (currentNode.gridX == end.gridX && currentNode.gridY == end.gridY)
            {
                path = RetracePath(currentNode);
                //moveResult = requiredMovePower; //최종값

                moveResult = 0;
                for (int i = 0; i < path.Count; ++i)
                {
                    moveResult += path[i].requiredMovePower;
                }
                print("최종값: " + moveResult);
                return path[0];
            }

            //이웃노드 검사한다.
            foreach (JKH_Node neighbour in GetNeighboursAdd(currentNode)) //getneighboursAdd
            {
                //걸을수없는 위치거나, 이웃이 closeSet에 있다면
                if (!neighbour.walkable || closeSet.Contains(neighbour))
                {
                    continue;
                }
                //g(x)+ 현재노드와 이웃간의 거리
                float newCostToNeighbour = currentNode.gCost + Vector3.Distance(currentNode.worldPosition, neighbour.worldPosition);
                //int newCostToNeighbour = 옆타일의 이동력
                //만약 이웃의 gCost가 더 크거나 이웃이 포함되어있지 않다면
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {

                    //gCost갱신
                    neighbour.gCost = newCostToNeighbour;
                    //hCost갱신
                    neighbour.hCost = Vector3.Distance(neighbour.worldPosition, end.worldPosition);
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
        return null;
    }

    List<JKH_Node> RetracePath(JKH_Node end)
    {
        List<JKH_Node> result = new List<JKH_Node>();

        JKH_Node prev = null;
        JKH_Node current = end;

        while (current != null)
        {
            JKH_Node next = current.parent;
            current.parent = prev;
            prev = current;
            current = next;
        }

        while (prev != null)
        {
            result.Add(prev);
            prev = prev.parent;
        }

        for (int i = 0; i < result.Count; ++i)
        {
            print(result[i]);
        }

        return result;
    }



    // 선택한 유닛의 movePower만큼 Physics.OverlapSphere 각각 FindPath
    public void CheckAbleToGo()
    {
        LayerMask layerMask = LayerMask.GetMask("GrassLand") | LayerMask.GetMask("Plains") | LayerMask.GetMask("Desert");

        Collider[] cols = Physics.OverlapSphere(transform.position, selectedUnit.movePower, layerMask);
        //float[] x = new float[cols.Length];


        KeyValuePair<int, int> mypos = CheckMyPos(); //클릭한유닛의 좌표.
        JKH_Node start = nodeMap[mypos.Key * mapWidth + mypos.Value];
        
        //JKH_Node start = new JKH_Node(JKH_Move.instance.grid.grid[mypos.Key, mypos.Value].walkable,
        //transform.position, mypos.Key, mypos.Value, JKH_Move.instance.grid.grid[mypos.Key, mypos.Value].requiredMovePower);

        for (int i = 0; i < cols.Length; i++)
        {

            TerrainData terrainData = cols[i].GetComponent<TerrainData>();
            //print(CheckMyPos());
            int x = terrainData.x;
            int y = terrainData.y;

         
            JKH_Node path = JKH_Move.instance.FindPath(start, end);
            float movePower = 0;

            print(string.Format("-> x : {0}, y : {1}", path.gridX, path.gridY));

            //while (path.parent != null)
            //    movePower += path.requiredMovePower;

            print(movePower);
            if (movePower > selectedUnit.movePower)
                print(string.Format("-> x : {0}, y : {1}", x, y));
        }
    }



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
                    TerrainData data = hitInfo.transform.gameObject.GetComponent<TerrainData>();
                    int idx = terrainDataMap.IndexOf(data);
                    //해당 x,y좌표 저장
                    neighbours.Add(nodeMap[idx]);
                }
            }
        }

        return neighbours;
    }

    //unit tilePos
    public GameObject myTilePos;
    public int posX, posY;

    public KeyValuePair<int, int> CheckMyPos()
    {
        //transform.pos대신 선택된유닛의 위치로 바꾼다.
        int fogLayer = LayerMask.GetMask("HexFog");

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);
        if (Physics.Raycast(ray, out hit, 1, ~fogLayer))
        {
            myTilePos = hit.transform.gameObject;

            posX = myTilePos.GetComponent<TerrainData>().x;
            posY = myTilePos.GetComponent<TerrainData>().y;
        }

        return new KeyValuePair<int, int>(posX, posY);
    }      

    
    // 결과 이동력이 유닛의 이동력보다 낮으면 이번 턴에 이동할 수 있는 타일
    // -> 해당하는 타일 print로 출력 -> OnDrawGizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //
    }
}
