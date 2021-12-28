using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    // 선택한 유닛
    public Unit selectedUnit;
    public int mapWidth, mapHeight;
    List<TerrainData> terrainDataMap;
    List<JKH_Node> nodeMap;

    //FindPath()'s variable
    float moveResult = 0;

    TerrainData[] terrainDatas;

    #region test
    List<JKH_Node> testAbleGoList = new List<JKH_Node>();
    #endregion

    JKH_Node temp;

    public Unit unitInfo;

    public bool ableToMove = false;

    void Start()
    {
        terrainDataMap = new List<TerrainData>(GetComponentsInChildren<TerrainData>());        
    }

    void Update()
    {
        getUnitInfo();
        SelectedUnitMove();
    }


    void InitNodeMap(int targetX, int targetY)
    {

        if (nodeMap != null)
            nodeMap.Clear();
        else
            nodeMap = new List<JKH_Node>();

        for (int i = 0; i < terrainDataMap.Count; ++i)
        {
            TerrainData data = terrainDataMap[i];

            TerrainType terrainType = data.terrainType;
            bool walkable = false;
            #region todo
            //유닛아래에있는 타일들은 못지나감 + 임의로 EnemyUnit설정
            //1. 유닛 밑에있는 타일의 정보들을 가져온다.
            //2. 가져온 타일의 정보를 담아둔다(List)
            //3. 그 정보들은 walkable= false이다
            //표시해주는 단계에서 추가할내용
            //4. 검사범위 안에있는(overlapSphere) 하나씩 true해주면서 그 위치를 갈 수 있는지 검사 
            //5. 갈수있다면 표시, 그렇지 않다면 표시안함.
            #endregion
            LayerMask unitLayer = LayerMask.GetMask("EnemyUnit");


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

        UnitCheck(targetX, targetY);
    }

    public void UnitCheck(int targetX, int targetY)
    {
          
        RaycastHit hitInfo;
        for (int j = 0; j < terrainDataMap.Count; j++)
        {           
            bool walkable;
            int mapX = terrainDataMap[j].x;
            int mapY = terrainDataMap[j].y;
            TerrainData data = terrainDataMap[(mapY * mapWidth) + mapX];

            TerrainType terrainType = data.terrainType;

            Ray ray = new Ray(terrainDataMap[j].transform.position, transform.up);
            LayerMask layer = LayerMask.GetMask("Unit");
            
            // 목표지점에 유닛이 있는 경우
            if (data == terrainDataMap[(targetY * mapWidth) + targetX])
            {
                print("유닛있음");
                continue;
            }
            if (Physics.Raycast(ray, out hitInfo, 5, layer))
            {
                unitInfo = hitInfo.transform.gameObject.GetComponent<Unit>();
                //결과 출력 
                print(unitInfo.name);
                walkable = false;
                print(terrainDataMap[j].x + ", " + terrainDataMap[j].y);

                
                JKH_Node node = new JKH_Node(walkable, data.transform.position, data.x, data.y, data.output.movePower);
                nodeMap[terrainDataMap[j].x + mapWidth * terrainDataMap[j].y].walkable = false;
                nodeMap.Add(node);                
            }
        }
    }


    //유닛에대한 정보를 가져오는 함수
    public void getUnitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        LayerMask layer = LayerMask.GetMask("Unit");
        //==LayerMask layer = 1 << LayerMask.NameToLayer("Unit");

        //마우스 클릭한다
        if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject())
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, layer))
            {

                //unitMove
                selectedUnit = hitInfo.transform.GetComponent<Unit>();
                ableToMove = false;

                // 유닛 정보 출력(확인용) 
                print(selectedUnit.gameObject.name);
                print("이동력: " + selectedUnit.movePower);
                print("체력: " + selectedUnit.hp);

                //유닛이있는 타일의 정보를 가져온다.

                CheckAbleToGo();
            }
        }
    }

    // FindPath
    public List<JKH_Node> FindPath(int startX, int startY, int endX, int endY)
    {
        InitNodeMap(endX, endY);

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
                float newCostToNeighbour = currentNode.gCost + neighbour.requiredMovePower;
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
            if (startPos == endPos)
                continue;
            print(startPos);
            print(endPos);
            List<JKH_Node> path = FindPath(startPos.x, startPos.y, endPos.x, endPos.y);

            movePower = 0;
            //path값 Null...
            string pathStr = string.Format("({0}, {1})", path[0].gridX, path[0].gridY);
            for (int j = 1; j < path.Count; ++j)
            {
                movePower += path[j].requiredMovePower;
                pathStr += string.Format("-> ({0}, {1})", path[j].gridX, path[j].gridY);
            }
            pathStr += "(이동력 :" + movePower + ")";
            print(pathStr);
            if (selectedUnit.movePower >= movePower)
            {
                //그려주기해야함
                testAbleGoList.Add(path[0]);
            }  

        }
    }

   
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


        if (ableToMove && selectedUnit.movePower > 0) //movePower여기맞나?
        {

            print("Get Selected Function");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;            

            //이동가능한좌표 표시하기
            for (int j = 0; j < testAbleGoList.Count; j++)
            {
                float cost = 0;
                JKH_Node dest = testAbleGoList[j];
                while (dest.parent != null)
                {
                    dest = dest.parent;
                    cost += dest.requiredMovePower;
                }

                print("ableToMove" + j + "번째: " + "x: " + dest.gridX + ", y: " + dest.gridY + ", cost : " + cost);

            }

            if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject() && selectedUnit.movePower > 0)
                if (Physics.Raycast(ray, out hitInfo, 1000))
                {
                    if (hitInfo.transform.gameObject.tag == "Map") 
                    {
                        //만약 이동하려는 좌표를 gameObj를 대신해 누른다면? 
                        //target= 내가 찍은 맵 좌표
                        int targetX = hitInfo.transform.gameObject.GetComponent<TerrainData>().x;
                        int targetY = hitInfo.transform.gameObject.GetComponent<TerrainData>().y;
                        JKH_Node target = nodeMap[targetX + mapWidth * targetY];

                        for (int i = 0; i < testAbleGoList.Count; i++)
                        {

                            LayerMask unitLayer = LayerMask.GetMask("Unit");
                            Collider[] tileOnObj = Physics.OverlapSphere(hitInfo.transform.position, .3f, unitLayer);

                            if (tileOnObj.Length > 0)
                            {
                                print("can not go");
                                return;
                            }
                            JKH_Node dest = testAbleGoList[i];
                            float movePower = 0;
                            while (dest.parent != null)
                            {
                                dest = dest.parent;
                                movePower += dest.requiredMovePower;
                            }


                            // 이동력 검사 ++ 해당타일에 다른 gameObj가 없다면?
                            //hitinfo 타일에 gameObj있나 없나 검사하기. 
                            if ((target == dest) && (movePower <= selectedUnit.movePower))
                            {
                                // 플레이어 오브젝트 위치 이동 칸대로 이동하기   
                                JKH_Node path = testAbleGoList[i];
                                StartCoroutine(MoveUnitCoroutine(selectedUnit, path));
                                // 좌표 이동, 이동력 감소
                                selectedUnit.movePower -= movePower;
                                if (selectedUnit.movePower <= 0)
                                {
                                    print("유닛 선택 해제");
                                    selectedUnit = null;
                                }
                                print("Success");
                                //bool off
                                ableToMove = false;
                            }

                        }

                        // 이동할 수 없는 타일을 선택했을 경우
                        if (ableToMove == true)
                            print("Failed");
                    }

                    else
                        print("Click Another One");

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

    //임의 변수(나중에 지워야함!!)
    public int unitDmg = 26;
    public int enemyDmg = 25;
    float damageDealt;

    public void UnitCombat()
    {

        //Todo
        //상성계산, 
        float rand = Random.Range(8.0f, 1.2f);
        damageDealt = 30 * Mathf.Exp(0.04f * unitDmg - enemyDmg) * rand;
        damageDealt = Mathf.Round(damageDealt);
    }

    //List<NonCombatUnit> unitPos = new List<NonCombatUnit>();
    public void CheckUnit_test()
    {
        GameObject[] unitPos = GameObject.FindGameObjectsWithTag("Unit");

        for (int i = 0; i < unitPos.Length; i++)
        {
            print("x: " + unitPos[i].GetComponent<NonCombatUnit>().posX
                + " y: " + unitPos[i].GetComponent<NonCombatUnit>().posY);
        }
    }

    IEnumerator MoveUnitCoroutine(Unit unit, JKH_Node path)
    {
        while (path!=null)
        {
            yield return new WaitForSeconds(1);
            Vector3 pos = path.worldPosition;
            pos.y = -.7f;
            unit.transform.position = pos;
            path = path.parent;
        }
        
    }
}
