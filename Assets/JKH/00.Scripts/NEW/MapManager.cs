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
    #region test
    List<JKH_Node> movableList = new List<JKH_Node>();
    #endregion

    JKH_Node temp;

    public Unit unitInfo;

    public bool ableToMove = false;
    LayerMask mapLayer;

    LineRenderer lr;

    void Start()
    {
        terrainDataMap = new List<TerrainData>(GetComponentsInChildren<TerrainData>());
        lr = GetComponent<LineRenderer>();
        float scaleX = Mathf.Cos(Time.time) * 0.5f + 1;
        float scaleY = Mathf.Sin(Time.time) * 0.5f + 1;
        lr.material.SetTextureScale("_MainTex", new Vector2(scaleX, scaleY));

        mapLayer = mapLayer | LayerMask.GetMask("GrassLand");
        mapLayer = mapLayer | LayerMask.GetMask("Plains");
        mapLayer = mapLayer | LayerMask.GetMask("Desert");
        mapLayer = mapLayer | LayerMask.GetMask("Mountain");
        mapLayer = mapLayer | LayerMask.GetMask("Coast");
        mapLayer = mapLayer | LayerMask.GetMask("Ocean");

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
                print("유닛있음");
                continue;
            }
            if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layer))
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
                //anim = hitInfo.transform.GetComponent<Animator>();
                ableToMove = false;

                // 유닛 정보 출력(확인용) 
                print(selectedUnit.gameObject.name);
                print("이동력: " + selectedUnit.movePower);
                print("체력: " + selectedUnit.hp);
                print("UnitID: " + selectedUnit.playerId);

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
            print(tileOnUnit.Length);
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

        for (int i = 0; i < movableList.Count; i++)
        {
            //큐브위치에 유닛이 있으면 안된다
            //print("cubeCount?==" + movableList.Count);
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(cube.GetComponent<BoxCollider>());
            cube.GetComponent<Renderer>().material.color = new Color(0, .5f, 0, .3f);
            cube.transform.localScale = Vector3.one * .3f;
            JKH_Node node = movableList[i];
            while (node.parent != null)
                node = node.parent;
            Vector3 pos = node.worldPosition;

            pos.y = -.5f;
            cube.transform.position = pos;

            oldCubes.Add(cube);
        }
        // 경로 탐색 완료
        // ToDo 큐브  박기 , 다른거 누르면 바꾼다. @@
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

            //print("Get Selected Function");
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

                //마우스 가르키고 있는 타일까지 경로 표시
                if (Physics.Raycast(ray, out hitInfo, 1000, mapLayer))
                {

                    if (hitInfo.transform.gameObject.tag == "Map" && hitInfo.transform.position == dest.worldPosition) //유닛있는데는 표시하면 안됨!
                    {
                        dest = movableList[i]; //시작점.
                        int lrCount = 0; //lineRenderer 갯수
                        //int destLen = -1;
                        lr.positionCount = 0;
                        while (dest != null)
                        {
                            lr.positionCount++;
                            Vector3 destPos = dest.worldPosition;
                            destPos.y = -.3f;
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
                            //DeleteCube();
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

                            //밑에 식 움직이게? 하기위한 if문 제일 앞에거만 되고 그 뒤에거는 안댐 t
                            //나중에 이거 walkable true해줘야함
                            //if ((movePower <= selectedUnit.movePower) && tileOnUnit.Length > 0 && tileOnUnit[0].GetComponent<Unit>().playerId != selectedUnit.playerId)
                            //{
                            //    print("===============");
                            //    print(dest);
                            //    //target.walkable = true;
                            //    print(target); //↓↓↓↓이게 false라서 안움직임 ↓↓↓↓
                            //    print("===============");
                            //}

                            // 목표지점에  상대방 유닛이있을때 그전까지이동하게하기
                            if (target.GetPosition() == dest.GetPosition() && (movePower <= selectedUnit.movePower) && tileOnUnit.Length > 0
                                    && tileOnUnit[0].GetComponent<Unit>().playerId != selectedUnit.playerId)
                            {
                                //목표지점에 상대유닛있다
                                print("상대유닛발견");

                                // 플레이어 오브젝트 위치 이동 칸대로 이동하기   
                                JKH_Node path = movableList[i];
                                DeleteCube();
                                StartCoroutine(MoveUnitCoroutine(selectedUnit, path, true));
                                selectedUnit.movePower -= movePower;
                                if (selectedUnit.movePower <= 0)
                                {
                                    print("유닛 선택 해제");
                                    selectedUnit = null;
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
                                //print("상대유닛발견실패");
                                // 플레이어 오브젝트 위치 이동 칸대로 이동하기   
                                JKH_Node path = movableList[i];
                                DeleteCube();
                                
                                StartCoroutine(MoveUnitCoroutine(selectedUnit, path,false));
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

    //Todo 전투
    Unit selectedUnitId;
    Unit targetUnitId;
    public void UnitCombat()
    {
        print("이함수 실행");
        #region Todo      
        //function = Combat()
        //[필요한 변수]
        //각 유닛 playerID
        //최종목적지의 좌표(전투에 이겼을때.)
        //전투에필요한 변수

        //[과정]
        //목표지점 누른다
        //만약 그것이(상대`)유닛이라면
        //+ 유닛있는 타일 잠궜는데 예외추가**
        //eg > 유닛의ID가 나와 다르다면 이동가능하다(변수 많이생길듯)


        //만약 그것의 playerID가 다르다면
        //그 전 방향까지 이동한다
        //싸우는 함수실행

        //[경우의수]
        //1_만약이긴다: 상대파괴 후 목적지 이동
        //2_비긴다: 그자리에 있는다
        //3_진다: 내 유닛 파괴
        #endregion
        float rand = Random.Range(8.0f, 1.2f);
        damageDealt = 30 * Mathf.Exp(0.04f * unitDmg - enemyDmg) * rand;
        damageDealt = Mathf.Round(damageDealt);
    }

    Vector3 dir;
    IEnumerator MoveUnitCoroutine(Unit unit, JKH_Node path, bool onEnemy = false)
    {
        // TODO 전투 
        //int count = 0;
        //// 노드의 갯수 저장
        //if onEnemy
        //    while
        //        {
        //        if path != null;
        //        ++count;
        //    }

        //어떤경로로 이동하는지 표시 
        while (path.parent != null)
        {
            //좌표틀어짐>> 정중앙에 위치시키도록한다
            yield return null;

            //anim test
            //anim.SetBool("isMove", true);

            //Vector3 pos = path.worldPosition; //1
            //pos.y = -.7f; //2
            //unit.transform.position = pos; //3
            //---
            dir = path.parent.worldPosition - path.worldPosition;
            dir.Normalize();

            unit.transform.position += dir * Time.deltaTime;
            //---
            if ((path.parent.worldPosition - unit.transform.position).sqrMagnitude < (.1f))
            {
                path = path.parent; //
            }
            dir = Vector3.zero;
            if (onEnemy == true)
            {
                if (path.parent.parent == null)
                    path = path.parent;
                
            }

            //// 움직이는 동작 처리하는 코루틴
            //yield return StartCoroutine(FinishedUnitCoroutine(null, null));
            //// path 다음으로

        }
        if (onEnemy == true)
        {
            UnitCombat();
            //Todo위치시켜주기 else도 마찬가지로 시도본다.?
        }
        //Teleport
        //Vector3 pos = path.worldPosition;
        //pos.y = -.5f;
        //unit.transform.position = pos;
        //경로표시 다끝나면 선 지운다.
        lr.positionCount = 0;
        //anim.SetBool("isMove", false);
    }
    IEnumerator FinishedUnitCoroutine(Unit unit, JKH_Node path) //움직임처리하는
    {                                                                                             
        yield return new WaitForSeconds(1); //???
    }

    #region 코루틴2
    IEnumerator MoveUnitCoroutine_2(Unit unit, JKH_Node path)
    {

        //어떤경로로 이동하는지 표시 
        while (path.parent.parent != null)
        {
            //좌표틀어짐>> 정중앙에 위치시키도록한다
            yield return null;

            //anim test
            //anim.SetBool("isMove", true);

            //Vector3 pos = path.worldPosition; //1
            //pos.y = -.7f; //2
            //unit.transform.position = pos; //3
            //---
            dir = path.parent.worldPosition - path.worldPosition;
            dir.Normalize();

            unit.transform.position += dir * Time.deltaTime;
            //---
            if ((path.parent.worldPosition - unit.transform.position).sqrMagnitude < (.01f))
            {
                path = path.parent; //
            }
            dir = Vector3.zero;

            //// 움직이는 동작 처리하는 코루틴
            //yield return StartCoroutine(FinishedUnitCoroutine(null, null));
            //// path 다음으로

        }
        //경로표시 다끝나면 선 지운다.
        lr.positionCount = 0;
        //anim.SetBool("isMove", false);

        UnitCombat();
    }
    #endregion
    public void DeleteCube()
    {
        if (oldCubes != null)
        {
            for (int j = 0; j < oldCubes.Count; j++)
            {
                Destroy(oldCubes[j]);
            }
        }
    }
}
