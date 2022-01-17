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
    Animator anim;

    List<JKH_Node> movableList = new List<JKH_Node>();
    //선제거 위한 List
    List<JKH_Node> ShaderList = new List<JKH_Node>();

    public Unit unitInfo;
    public bool ableToMove = false;
    LayerMask mapLayer;
    LineRenderer lr;

    //mark(선택한 유닛, 목적타일표시, 적군포착 할 때 표시)
    public GameObject unitMark;
    public GameObject moveMark;
    public GameObject enemyMark;

    public bool unitSelecting;

    void Start()
    {
        terrainDataMap = new List<TerrainData>(GetComponentsInChildren<TerrainData>());

        lr = GetComponent<LineRenderer>();
        lr.material.SetTextureScale("_MainTex", new Vector2(10f, 1f));

        mapLayer = mapLayer | LayerMask.GetMask("GrassLand");
        mapLayer = mapLayer | LayerMask.GetMask("Plains");
        mapLayer = mapLayer | LayerMask.GetMask("Desert");
        mapLayer = mapLayer | LayerMask.GetMask("Mountain");
        mapLayer = mapLayer | LayerMask.GetMask("Coast");
        mapLayer = mapLayer | LayerMask.GetMask("Ocean");

        unitSelecting = false;
    }

    void Update()
    {
        getUnitInfo();
        SelectedUnitMove();
        UnitMarks();
    }

    public void UnitMarks()
    {
        //UnitMark
        if (unitMark.activeInHierarchy == true && selectedUnit != null)
        {
            Vector3 pos = selectedUnit.transform.position;
            pos.y += .1f;
            unitMark.transform.position = pos;
            // print("UnitMarks");
        }
        //MoveMark
        //EnemyMark
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

    // TODO 광훈 targetX, targetY만 검사하는 코드로 수정
    public void UnitCheck(int targetX, int targetY)
    {

        for (int j = 0; j < terrainDataMap.Count; j++)
        {
            int mapX = terrainDataMap[j].x;
            int mapY = terrainDataMap[j].y;
            TerrainData data = terrainDataMap[(mapY * mapWidth) + mapX];

            Ray ray = new Ray(terrainDataMap[j].transform.position, transform.up);
            RaycastHit hitInfo;
            LayerMask layer = LayerMask.GetMask("Unit");

            // 목표지점에 유닛이 있는 경우
            if (data == terrainDataMap[(targetY * mapWidth) + targetX])
            {
                //print("유닛있음");
                continue;
            }
            if (Physics.Raycast(ray, out hitInfo, float.MaxValue, layer))
            {
                unitInfo = hitInfo.transform.gameObject.GetComponent<Unit>();

                nodeMap[terrainDataMap[j].x + mapWidth * terrainDataMap[j].y].walkable = false;
            }
        }
    }


    //유닛에대한 정보를 가져오는 함수
    public void getUnitInfo()
    {
        if (Camera.main == null)
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        LayerMask layer = LayerMask.GetMask("Unit");
        //==LayerMask layer = 1 << LayerMask.NameToLayer("Unit");

        //마우스 클릭한다
        if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject())
        {
            
            if (Physics.Raycast(ray, out hitInfo, 1000, layer))
            {
                
                lr.positionCount = 0;
                //unitMove
                selectedUnit = hitInfo.transform.GetComponent<Unit>();
                anim = hitInfo.transform.GetComponent<Animator>();
                ableToMove = false;

                // 유닛 정보 출력(확인용)
                print(selectedUnit.gameObject.name);
                print("이동력: " + selectedUnit.movePower);
                print("체력: " + selectedUnit.hp);
                print("UnitID: " + selectedUnit.playerId);

                unitSelecting = true;

                //생성
                unitMark.SetActive(true);
                Vector3 markPos = selectedUnit.transform.position;
                markPos.y += 0.01f;
                unitMark.transform.position = markPos;

                //selectedUnit.transform.GetChild(1).gameObject.SetActive(true);


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

        List<Collider> cols = new List<Collider>(Physics.OverlapSphere(selectedUnit.transform.position, selectedUnit.movePower * .75f, layerMask));

        Vector2Int startPos = CheckMyPos(); //클릭한유닛의 좌표.
        movableList.Clear();


        StartCoroutine(unitMoveStep(cols, startPos));
        //for (int i = 0; i < cols.Count; i++)
        //{
        //    TerrainData terrainData = cols[i].GetComponent<TerrainData>();
        //    Vector2Int endPos = new Vector2Int(terrainData.x, terrainData.y);
        //    if (startPos == endPos)
        //        continue;
        //    print(startPos);
        //    print(endPos);
        //    List<JKH_Node> path = FindPath(startPos.x, startPos.y, endPos.x, endPos.y);
        //    if (path == null)
        //        continue;
        //    movePower = 0;
        //    //path값 Null...
        //    string pathStr = string.Format("({0}, {1})", path[0].gridX, path[0].gridY);
        //    for (int j = 1; j < path.Count; ++j)
        //    {
        //        movePower += path[j].requiredMovePower;
        //        pathStr += string.Format("-> ({0}, {1})", path[j].gridX, path[j].gridY);
        //    }
        //    pathStr += "(이동력 :" + movePower + ")";
        //    print(pathStr);
        //    if (selectedUnit.movePower >= movePower)
        //    {
        //        //그려주기해야함
        //        movableList.Add(path[0]);
        //    }

        //}
    }

    List<GameObject> oldCubes = new List<GameObject>();
    IEnumerator unitMoveStep(List<Collider> cols, Vector2Int startPos)
    {

        //oldCube = GameObject.Find("Cube");
        DeleteCube();


        List<JKH_Node> path = new List<JKH_Node>();

        for (int i = 0; i < cols.Count; i++)
        {
            LayerMask unitLayer = LayerMask.GetMask("Unit");
            Collider[] tileOnUnit = Physics.OverlapSphere(cols[i].transform.position, .3f, unitLayer);
            //print(tileOnUnit.Length);
            if (tileOnUnit.Length >= 1 && tileOnUnit[0].GetComponent<Unit>().playerId == selectedUnit.playerId) //상대방유닛은 안거르게됨.
            {
                tileOnUnit.Initialize();
                continue;
            }
            //tileOnUnit.Initialize();
            TerrainData terrainData = cols[i].GetComponent<TerrainData>();
            Vector2Int endPos = new Vector2Int(terrainData.x, terrainData.y);
            if (startPos == endPos)
                continue;
            //print(startPos);
            //print(endPos);
            path = FindPath(startPos.x, startPos.y, endPos.x, endPos.y);
            yield return null;
            if (path == null)
                continue;

            //LayerMask unitLayer = LayerMask.GetMask("Unit");
            //Collider[] tileOnUnit = Physics.OverlapSphere(cols[i].transform.position, .3f, unitLayer);
            //print(tileOnUnit.Length);


            movePower = 0;
            //path값 Null...
            string pathStr = string.Format("({0}, {1})", path[0].gridX, path[0].gridY);
            for (int j = 1; j < path.Count; ++j)
            {
                movePower += path[j].requiredMovePower;
                pathStr += string.Format("-> ({0}, {1})", path[j].gridX, path[j].gridY);
            }
            pathStr += "(이동력 :" + movePower + ")";
            //print(pathStr);
            //print(pathStr);
            if (selectedUnit.movePower >= movePower)
            {
                //그려주기해야함 corout
                movableList.Add(path[0]);
            }
        }


        //Create Cube/ outline
        //저장소 구하기..
        
        for (int i = 0; i < movableList.Count; i++)
        {
            while (movableList[i].parent != null)
            {
                movableList[i] = movableList[i].parent;
            }
            int x = movableList[i].gridX;
            int y = movableList[i].gridY;
            print("list" + x + ", " + y);

            
            
            terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/OutlineShader");
            //====
            //terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<Outline>().OutlineWidth = 10;
            //cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/OutlineShader");

            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Destroy(cube.GetComponent<BoxCollider>());
            //cube.GetComponent<Renderer>().material.color = new Color(0, .5f, 0, .3f);
            //cube.transform.localScale = Vector3.one * .3f;
            //JKH_Node node = movableList[i];
            //while (node.parent != null)
            //    node = node.parent;
            //Vector3 pos = node.worldPosition;

            //pos.y = -.5f;
            //cube.transform.position = pos;

            //oldCubes.Add(cube);
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

    //bool visualizeOneTime = false;
    public GameObject rayWay;
    Vector3 targetPos;
    public void SelectedUnitMove()
    {
        if (ableToMove && selectedUnit.movePower > 0)
        {



            print("Get Selected Function");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            //이동가능한좌표 표시하기
            for (int i = 0; i < movableList.Count; i++)
            {
                float cost = 0;
                JKH_Node dest = movableList[i];
                while (dest.parent != null)
                {
                    dest = dest.parent;
                    cost += dest.requiredMovePower;
                }

                //진짜 이동가능한 좌표.
                if (cost > selectedUnit.movePower)
                {
                    print("ableToMove" + i + "번째: " + "x: " + dest.gridX + ", y: " + dest.gridY + ", cost : " + cost);
                }

                //이미 dest.parent=null
                //print(new Vector2(dest.gridX, dest.gridX)); //가능한좌표 표시한다

                //MoveMark표시
                if (Physics.Raycast(ray, out hitInfo, 1000, mapLayer))
                {
                    if (hitInfo.transform.gameObject.tag == "Map")
                    {
                        LayerMask unitLayer = LayerMask.GetMask("Unit");
                        Collider[] tileOnUnit = Physics.OverlapSphere(hitInfo.transform.position, .3f, unitLayer);
                        if (tileOnUnit.Length > 0 && tileOnUnit[0].GetComponent<Unit>().playerId != selectedUnit.playerId)
                        {
                            enemyMark.SetActive(true);
                            Vector3 enemyMarkPos = hitInfo.transform.position;
                            enemyMarkPos.y += .2f;
                            enemyMark.transform.position = enemyMarkPos;
                        }
                        else
                            enemyMark.SetActive(false);

                        //moveMark Pos 표시..
                        moveMark.SetActive(true);
                        Vector3 moveMarkPos = hitInfo.transform.position;
                        moveMarkPos.y += .2f;
                        moveMark.transform.position = moveMarkPos;
                    }
                }

                //마우스 가르키고 있는 타일까지 경로 표시
                if (Physics.Raycast(ray, out hitInfo, 1000, mapLayer))
                {

                    if (hitInfo.transform.gameObject.tag == "Map" && hitInfo.transform.position == dest.worldPosition) //유닛있는데는 표시하면 안됨!
                    {
                        //print("XXX");
                        dest = movableList[i]; //시작점.
                        int lrCount = 0; //lineRenderer 갯수
                        //int destLen = -1;
                        lr.positionCount = 0;
                        while (dest != null)
                        {
                            lr.positionCount++;
                            Vector3 destPos = dest.worldPosition;
                            destPos.y = -.9f;
                            lr.SetPosition(lrCount, destPos);
                            lrCount++;
                            dest = dest.parent;


                        }
                        #region 보기싫은
                        //dest = movableList[i]; //또?
                        //print(dest);
                        //print(dest.parent);
                        //print(destLen);
                        //while (dest != null)
                        //while(destLen!=0)
                        //{
                        //print(dest.gridX + ", " + dest.gridY);
                        //dest-dest.parent 선긋기

                        //if (dest.parent == null)
                        //{
                        //    targetPos = hitInfo.transform.position;
                        //}
                        //else
                        //{
                        //    targetPos = dest.parent.worldPosition;

                        //}
                        //dest.worldPosition.y = -.7f;
                        //dest.parent.worldPosition.y = -.7f;
                        //lr.SetPosition(0, dest.worldPosition);
                        //lr.SetPosition(1, targetPos);

                        //dest.worldPosition.y = -.7f;
                        ///----
                        //lr.SetPosition(0, dest.worldPosition);
                        //if (dest.parent == null)
                        //{
                        //    targetPos = selectedUnit.transform.position;
                        //}
                        //else if (dest.parent != null)
                        //{
                        //    targetPos = dest.worldPosition;
                        //}
                        //targetPos.y = -.7f;
                        //lr.SetPosition(1, targetPos);
                        //dest = dest.parent;
                        //destLen--;
                        //아무튼이거이상함
                        //}
                        #endregion
                    }
                }
                else
                    lr.positionCount = 0;

                //이동 가능한 타일 목록(시각화) ==> 너무많이 생성된다? 계속update가 된다.
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.GetComponent<Renderer>().material.color = new Color(0, .5f, 0, .3f);
                //cube.transform.localScale = Vector3.one * .3f;
                //Vector3 pos = dest.worldPosition;
                //pos.y = -.5f;
                //cube.transform.position = pos;
            }

            //print(movableList.Count);
            //for (int k = 0; k < movableList.Count; k++)
            //{
            //    JKH_Node dest = movableList[k];
            //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //    cube.GetComponent<Renderer>().material.color = new Color(0, .5f, 0, .3f);
            //    cube.transform.localScale = Vector3.one * .3f;
            //    Vector3 pos = dest.worldPosition;
            //    pos.y = -.5f;
            //    cube.transform.position = pos;

            //}

            if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject() && selectedUnit.movePower > 0)
                if (Physics.Raycast(ray, out hitInfo, 1000, mapLayer))
                {
                    if (hitInfo.transform.gameObject.tag == "Map")
                    {
                        //lr.positionCount = 0;
                        //만약 이동하려는 좌표를 gameObj를 대신해 누른다면?
                        //target= 내가 찍은 맵 좌표
                        int targetX = hitInfo.transform.gameObject.GetComponent<TerrainData>().x;
                        int targetY = hitInfo.transform.gameObject.GetComponent<TerrainData>().y;
                        JKH_Node target = nodeMap[targetX + mapWidth * targetY];



                        for (int i = 0; i < movableList.Count; i++)
                        {
                            //누르면 큐브 사라지게한다.
                            DeleteCube();
                            LayerMask unitLayer = LayerMask.GetMask("Unit");
                            Collider[] tileOnUnit = Physics.OverlapSphere(hitInfo.transform.position, .3f, unitLayer);

                            //조건추가 (내 playerID와 목표지점 playerID가 같으면 못간다. 즉, 다르면 적군이고 클릭 할 수 있다.)
                            //&&hitInfo.transform.GetComponent<Unit>().playerId==selectedUnit.playerId
                            //if (tileOnUnit.Length > 0 && hitInfo.transform.GetComponent<Unit>().playerId != selectedUnit.playerId)
                            //{
                            //    print("다른유닛");
                            //    return;
                            //}

                            //if (tileOnUnit.Length>0)
                            //{
                            //    print(tileOnUnit[0].gameObject.name);
                            //}
                            //print(tileOnUnit[0].name);
                            if (tileOnUnit.Length > 0 && tileOnUnit[0].GetComponent<Unit>().playerId == selectedUnit.playerId)
                            {
                                print("can not go");
                                lr.positionCount = 0;
                                return;
                            }


                            JKH_Node dest = movableList[i];
                            float movePower = 0;
                            while (dest.parent != null)
                            {
                                dest = dest.parent;
                                movePower += dest.requiredMovePower;
                            }

                            // 목표지점에  상대방 유닛이있을때 그전까지이동하게하기
                            if (target.GetPosition() == dest.GetPosition() && (movePower <= selectedUnit.movePower) && tileOnUnit.Length > 0
                                    && tileOnUnit[0].GetComponent<Unit>().playerId != selectedUnit.playerId)
                            {
                                //목표지점에 상대유닛있다
                                print("상대유닛발견");

                                // 플레이어 오브젝트 위치 이동 칸대로 이동하기
                                JKH_Node path = movableList[i];
                                DeleteCube();

                                //DeleteCube() 하고나면 path 다시 넣어줘야함.
                                StartCoroutine(MoveUnitCoroutine(selectedUnit, path, true));
                                selectedUnit.movePower -= movePower;

                                //=0
                                selectedUnit.movePower = 0;
                                if (selectedUnit.movePower <= 0)
                                {
                                    print("유닛 선택 해제");
                                    //selectedUnit = null;
                                }
                                print("Success");
                                //bool off
                                ableToMove = false;
                                break;
                            }

                            //hitinfo 타일에 gameObj있나 없나 검사하기.
                            if ((target.GetPosition() == dest.GetPosition())
                                && (movePower <= selectedUnit.movePower))
                            {
                                // 플레이어 오브젝트 위치 이동 칸대로 이동하기
                                JKH_Node path = movableList[i];
                                DeleteCube();
                                path = movableList[i];
                                StartCoroutine(MoveUnitCoroutine(selectedUnit, path, false));
                                // 좌표 이동, 이동력 감소
                                selectedUnit.movePower -= movePower;
                                if (selectedUnit.movePower <= 0)
                                {
                                    print("유닛 선택 해제");
                                    //selectedUnit = null;
                                }
                                print("Success");
                                //bool off
                                ableToMove = false;
                                break;
                            }


                        }

                        // 이동할 수 없는 타일을 선택했을 경우
                        if (ableToMove == true)
                        {
                            print("Failed");
                        }


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
        //int mapLayer = LayerMask.GetMask("");

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


    public int damageDealt;
    public int damageReceived;

    //enemy부분 combatUnit일떄랑 그렇지않을떄랑 경우의수 나누어서 본다.
    public void UnitCombat(CombatUnit unit, Unit enemy)
    {
        print("이함수 실행");

        //아군 유닛 변수
        //float unitHp = unit.hp;
        float unitMeleeDmg = unit.meleeAttack;
        float unitRangeDmg = unit.rangeAttack;
        float enemyMeleeDmg = 0;
        float enemyRangeDmg = 0;
        //상대 유닛 변수
        //float enemyHp = enemy.hp;
        if (enemy.GetComponent<CombatUnit>() != null)
        {
            enemyMeleeDmg = enemy.GetComponent<CombatUnit>().meleeAttack;
        }
        else
            print("이거왜그럼?");
        //enemyMeleeDmg = enemy.meleeAttack;
        //enemyRangeDmg = enemy.rangeAttack;

        battleFormula(unitMeleeDmg, enemyMeleeDmg);
        //여기다 전투 enim..



        unit.hp = unit.hp - damageReceived;
        enemy.hp = enemy.hp - damageDealt;
        if (unit.hp > 0 && enemy.hp > 0)
        {
            print("both survived");
            unit.movePower = 0;
            return;
        }

        else if (unit.hp > 0 && enemy.hp <= 0)
        {
            Destroy(enemy.gameObject);
            unit.movePower = 0;
            print("won");
            lastPos.y = -.7f;
            StartCoroutine(MoveUnitCoroutine(selectedUnit, finalMove, false));
        }

        else if (enemy.hp <= 0)
        {
            unit.movePower = 0;
            Destroy(enemy.gameObject);
            print("enemy has been slained");
        }

        if (unit.hp <= 0)
        {
            Destroy(unit.gameObject);
            print("ally has been slained");
        }

    }


    //수정해야함
    public void calculateWeight()
    {
        int playerCode = 0;
        int enemyCode = 0;
        float weight = 1;
        //상성 가중치 (playerCode, enemyCode) //근접(1)>대기병(2)>기병(3)>근접(1)

        if (playerCode == enemyCode)
        {
            weight = 1;
        }

        //player가 이기는 상성
        else if ((playerCode == 1 && enemyCode == 2)
            || (playerCode == 2) && (enemyCode == 3)
            || (playerCode == 3) && (enemyCode == 1))
        {
            weight = 1.2f;
        }

        //지는상성
        else if ((playerCode == 2 && enemyCode == 1)
            || (playerCode == 3) && (enemyCode == 2)
            || (playerCode == 1) && (enemyCode == 3))
        {
            weight = 1.0f / 1.2f;
        }

        //그외 있어선 안되는 일
        else
        {
            print("이건 말도 안되는 일");
            weight = 1;
        }

        print("playerCode: " + playerCode);
        print("enemyCode" + enemyCode);
        print("weight=" + weight);
    }

    //내가 주는 데미지 계산. (나-> 상대방) (상대방-> 나)

    public void battleFormula(float myDmg, float opponentDmg)
    {
        print(myDmg);
        print(opponentDmg);


        float rand = Random.Range(0.75f, 1.25f);
        print(30 * (Mathf.Exp(0.04f * (myDmg - opponentDmg)) * rand));
        damageDealt = Mathf.RoundToInt(30 * (Mathf.Exp(0.04f * (myDmg - opponentDmg)) * rand));

        //damageDealt = Mathf.Round(damageDealt);
        damageReceived = Mathf.RoundToInt(30 * (Mathf.Exp(0.04f * (opponentDmg - myDmg)) * rand));

        //상대유닛이 nonCombatUnit일때 max데미지로 때린다
        if (myDmg > 0 && opponentDmg == 0)
        {
            damageDealt = 10000;
            damageReceived = 0;
        }

        print("damageDealt=" + damageDealt);
        print("damageReceived= " + damageReceived);
        //damageReceived = Mathf.Round(damageReceived);

    }

    Vector3 dir;
    Vector3 lastPos;
    JKH_Node finalMove;
    IEnumerator MoveUnitCoroutine(Unit unit, JKH_Node path, bool onEnemy = false)
    {
        // lr 하나씩 지운다?
        int lrCount = 0;

        unitSelecting = false;
        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_INFANTRY_WALK);

        //어떤경로로 이동하는지 표시
        while (path.parent != null)
        {

            //print("116");
            yield return null;

            anim.SetBool("isMove", true);

            // 이동방향 : 현재 타일 -> 다음 타일
            dir = path.parent.worldPosition - path.worldPosition;
            dir.Normalize();
            Vector3 dest = path.parent.worldPosition;
            dest.y = unit.transform.position.y;
            // 유닛 바라보는 방향 이동방향으로 변경
            unit.transform.forward = dir;
            unit.transform.position += dir * Time.deltaTime;
            // 보정값 안에 들어오면 도착한것으로 판단
            if ((dest - unit.transform.position).sqrMagnitude < 0.005f)
            {
                ////2
                lrCount++;

                print("lrCount: " + lrCount);
                // 다음 타일로 변경
                path = path.parent;
            }
            dir = Vector3.zero;
            if (onEnemy == true)
            {
                if (path.parent.parent == null)
                {
                    finalMove = path;
                    path = path.parent;
                }
                lastPos = path.worldPosition;

            }
            //lr 제거..




        }
        if (onEnemy == true)
        {
            LayerMask unitLayer = LayerMask.GetMask("Unit");
            // overlapsphere path 타일에 있는 상대 유닛 찾아옴
            Collider[] tileOnUnit = Physics.OverlapSphere(path.worldPosition, .3f, unitLayer);
            print(path.gridX + ", " + path.gridY);

            UnitCombat(selectedUnit.GetComponent<CombatUnit>(), tileOnUnit[0].GetComponent<Unit>());

            //Todo위치시켜주기 else도 마찬가지로 시도본다.?
        }

        //경로표시 다끝나면 선 지운다.
        lr.positionCount = 0;
        anim.SetBool("isMove", false);

        unit.transform.forward = Vector3.back;

        //유닛 위치 타일에 다시 저장
        unit.CheckMyPos();

    }

    //함수이름 바꾸기,
    public void DeleteCube()
    {
        if (oldCubes != null)
        {
            for (int j = 0; j < oldCubes.Count; j++)
            {
                Destroy(oldCubes[j]);
            }
            
        }


        // 선 제거. 여기서 path.parent null시킴
        for (int i = 0; i < movableList.Count; i++)
        {
            //while (movableList[i].parent != null)
            //{
            //    movableList[i] = movableList[i].parent;
            //}
            //int x = movableList[i].gridX;
            //int y = movableList[i].gridY;
            //Material material = terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material;
            //material.shader = Shader.Find("Standard");
            //terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material = material;
            //이러면 최근데이터 불러와져서 안지워짐, 근데 다른 타일 누르면 지워짐
            //그래서 데이터따로 저장하고 불러와서 지운다.
            JKH_Node node = movableList[i];
            while (node.parent != null)
            {
                node = node.parent;
            }

            
            terrainDataMap[node.gridX + node.gridY * mapWidth].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
            
            //terrainDataMap[node.gridX + node.gridY * mapWidth].gameObject.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor", new Color(0, 0, 0, 0));
        }

        
    }

    //막 사라지지 않게 경우의수 추가
    public void MarkDisabled()
    {
        unitMark.SetActive(false);
        moveMark.SetActive(false);
        enemyMark.SetActive(false);
    }
}
