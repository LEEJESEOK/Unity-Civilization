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

    //FindPath()'s variable
    float moveResult = 0;

    //createGrid
    TerrainData[] terrainDatas;

    #region test
    List<JKH_Node> testAbleGoList = new List<JKH_Node>();
    #endregion


    public bool ableToMove = false;

    void Start()
    {
        terrainDataMap = new List<TerrainData>(GetComponentsInChildren<TerrainData>());
        InitNodeMap(); //前 createGrid
    }

    void Update()
    {
        //set
        getUnitInfo();
        SelectedUnitMove();
    }

    private void InitNodeMap()
    {

        nodeMap = new List<JKH_Node>();
        for (int i = 0; i < terrainDataMap.Count; ++i)
        {
            TerrainData data = terrainDataMap[i];

            TerrainType terrainType = data.terrainType;
            bool walkable = false;

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

            JKH_Node node = new JKH_Node(walkable, data.transform.position, data.x, data.y, data.output.movePower);
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
        if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject())
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

                CheckAbleToGo();
            }
        }
    }

    // FindPath
    public List<JKH_Node> FindPath(int startX, int startY, int endX, int endY)
    {
        InitNodeMap();

        List<JKH_Node> openSet = new List<JKH_Node>();
        HashSet<JKH_Node> closeSet = new HashSet<JKH_Node>();
        JKH_Node start = nodeMap[startX + startY * mapWidth];
        JKH_Node end = nodeMap[endX + endY * mapWidth];
        openSet.Add(start);

        //calcualting required MovePower
        //subtract first Node
        while (openSet.Count > 0)
        {
            JKH_Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                //만족한 값으로 이동
                if ((openSet[i].fCost < currentNode.fCost)
                    || (openSet[i].fCost == currentNode.fCost) && (openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];

                }
            }
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            //if (currentNode == end)
            //if (currentNode.gridX == end.gridX && currentNode.gridY == end.gridY)
            if (currentNode == end)
            {

                List<JKH_Node> path = RetracePath(currentNode);

                return path;
            }

            //이웃노드 검사한다.
            foreach (JKH_Node neighbour in GetNeighboursAdd(currentNode)) //getneighboursAdd
            {
                //걸을수없는 위치거나, 이웃이 closeSet에 있다면
                if (neighbour.walkable == false || closeSet.Contains(neighbour))
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
        print("error");
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

        return result;
    }

    void InitNodeParent()
    {
        for (int i = 0; i < nodeMap.Count; ++i)
        {
            nodeMap[i].parent = null;
        }
    }



    //need to call    
    // 선택한 유닛의 movePower만큼 Physics.OverlapSphere 각각 FindPath
    public float movePower = 0;
    public void CheckAbleToGo()
    {
        LayerMask layerMask = LayerMask.GetMask("GrassLand") | LayerMask.GetMask("Plains") | LayerMask.GetMask("Desert");

        List<Collider> cols = new List<Collider>(Physics.OverlapSphere(selectedUnit.transform.position, selectedUnit.movePower, layerMask));

        Vector2Int startPos = CheckMyPos(); //클릭한유닛의 좌표.
        testAbleGoList.Clear();

        for (int i = 0; i < cols.Count; i++)
        {
            TerrainData terrainData = cols[i].GetComponent<TerrainData>();
            Vector2Int endPos = new Vector2Int(terrainData.x, terrainData.y);
            //if (start.gridX == end.gridX && start.gridY == end.gridY)
            //    print("equal");
            if (startPos == endPos)
                continue;

            List<JKH_Node> path = FindPath(startPos.x, startPos.y, endPos.x, endPos.y);

            movePower = 0;
            string pathStr = string.Format("({0}, {1})", path[0].gridX, path[0].gridY);
            for (int j = 1; j < path.Count; ++j)
            {
                movePower += path[j].requiredMovePower;
                pathStr += string.Format("-> ({0}, {1})", path[j].gridX, path[j].gridY);
            }
            pathStr += "(이동력 :" + movePower + ")";
            print(pathStr);
            // TODO


            if (selectedUnit.movePower >= movePower)
            {
                //그려주기해야함
                testAbleGoList.Add(path[path.Count - 1]);  
            }



            //[ToDo] 여기다가 UnitMove를 넣어야하는가?
            //이동할수있는타일을 gizmos를 통하여 알려준다. 
            //표시된 타일을 클릭할수있게한다. bool? 이런걸로?
            //만약 그 부분을 누른다면?
            //+(확인버튼만들던가)
            //(유닛의 이동력- 이동하기까지 이동력)을 한 후에 이동한다   

        }

        // TODO
        //ableToMove = true;
    }

    //Move Selected Unit 0
    //move버튼 클릭 0
    //-> OnClick 함수 0
    //bool 변수 / true  0
    //마우스가위치한 타일의 경로를 표시 x

    //Input.GetMouseButtonDown
    //타일로 이동
    public void onClickMove()
    {
        //NullCheck
        if (selectedUnit != null)
        {
            ableToMove = true;
        }
    }

    public void SelectedUnitMove()
    {
        if (ableToMove)
        {

            print("Get Selected Function");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            print(movePower);
            
            if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject())
                if (Physics.Raycast(ray, out hitInfo, 1000))
                {
                    //for (int i = 0; i < testAbleGoList.Count - 1;i++) 
                    if (hitInfo.transform.gameObject.tag == "Map")  // && testAbleGoList.Contains(hitInfo) -- testAbleGoList에 포함되어있는가.?
                    {
                        //마우스포인터가 위치한 좌표 말해주기? 
                        {
                            
                            //선택된 유닛의 이동력이, [해당 타일까지 가는데 요구되는 이동력]*****보다 크거나 같다면?
                            //movePower 미리 설정된것같은데 Selected의 movePower를 가져와야하지 않을까?@@@@
                            if (selectedUnit.movePower >=  movePower)
                            {
                                selectedUnit.transform.position = hitInfo.transform.position;
                                selectedUnit.movePower -= (int)movePower;
                                ableToMove = false;
                                print("moveFinished");
                            }

                            else
                            {
                                print("moveFailed");
                                ableToMove = false;
                            }
                        }
                    }
                }
        }

    }


    public List<JKH_Node> GetNeighboursAdd(JKH_Node node)
    {
        List<JKH_Node> neighbours = new List<JKH_Node>();

        RaycastHit hitInfo;
        Quaternion originRotation = selectedUnit.transform.rotation;

        //검사
        for (int i = 0; i < 6; i++)
        {
            Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
            var pos = node.worldPosition;
            pos.y = -0.95f;
            Ray ray = new Ray(pos, rotation * selectedUnit.transform.forward);

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
    public Vector2Int CheckMyPos()
    {
        //transform.pos대신 선택된유닛의 위치로 바꾼다.
        int fogLayer = LayerMask.GetMask("HexFog");

        Ray ray = new Ray(selectedUnit.transform.position, transform.up * -1);
        RaycastHit hit;
        Vector2Int result = new Vector2Int();
        if (Physics.Raycast(ray, out hit, 1, ~fogLayer))
        {
            myTilePos = hit.transform.gameObject;

            result.x = myTilePos.GetComponent<TerrainData>().x;
            result.y = myTilePos.GetComponent<TerrainData>().y;
        }

        return result;
    }


    // 결과 이동력이 유닛의 이동력보다 낮으면 이번 턴에 이동할 수 있는 타일
    // -> 해당하는 타일 print로 출력 -> OnDrawGizmo


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < testAbleGoList.Count; ++i)
            Gizmos.DrawCube(testAbleGoList[i].worldPosition, Vector3.one * .5f);
    }



}
