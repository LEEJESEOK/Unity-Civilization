using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    // 선택한 유닛
    public Unit selectedUnit;
    public int mapWidth, mapHeight;
    public List<TerrainData> terrainDataMap;
    List<JKH_Node> nodeMap;
    Animator anim;

    List<JKH_Node> movableList = new List<JKH_Node>();
    //선제거 위한 List
    List<JKH_Node> ShaderList = new List<JKH_Node>();

    public Unit unitInfo;
    public bool ableToMove = false;
    LayerMask mapLayer;
    LayerMask cityLayer;
    LineRenderer lr;

    //mark(선택한 유닛, 목적타일표시, 적군포착 할 때 표시)
    public GameObject unitMark;
    public GameObject moveMark;
    public GameObject enemyMark;

    public bool unitSelecting;

    LayerMask unitLayer;

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


        unitLayer = LayerMask.GetMask("Unit");

        cityLayer = LayerMask.GetMask("City");

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
            layer |= LayerMask.GetMask("City");

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
            return;
        // 선택한 오브젝트가 유닛
        if (GameManager.instance.IsCurrentUnit() == false)
            return;
        // 현재 선택된 유닛과 새로 선택한 유닛이 다를 때
        if (selectedUnit == GameManager.instance.GetCurrentUnit())
            return;

        selectedUnit = GameManager.instance.GetCurrentUnit();
        anim = selectedUnit.animator;

        ableToMove = false;

        unitSelecting = true;

        MarkEnabled();

        CheckAbleToGo();


        // //마우스 클릭한다
        // if (Input.GetButtonDown("Fire1") && !UIManager.IsPointerOverUIObject())
        // {
        //     MarkDisabled();
        //     InitMoveArea();
        //     if (Physics.Raycast(ray, out hitInfo, 1000, layer))
        //     {

        //         lr.positionCount = 0;
        //         //unitMove
        //         selectedUnit = hitInfo.transform.GetComponent<Unit>();
        //         anim = hitInfo.transform.GetComponent<Animator>();
        //         ableToMove = false;

        //         // 유닛 정보 출력(확인용)
        //         print(selectedUnit.gameObject.name);
        //         print("이동력: " + selectedUnit.movePower);
        //         print("체력: " + selectedUnit.hp);
        //         print("UnitID: " + selectedUnit.playerId);

        //         unitSelecting = true;

        //         //생성
        //         unitMark.SetActive(true);
        //         Vector3 markPos = selectedUnit.transform.position;
        //         markPos.y += 0.01f;
        //         unitMark.transform.position = markPos;

        //         //selectedUnit.transform.GetChild(1).gameObject.SetActive(true);


        //         //유닛이있는 타일의 정보를 가져온다.

        //         CheckAbleToGo();
        //     }
        // }
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
    public void CheckAbleToGo()
    {
        LayerMask layerMask = LayerMask.GetMask("GrassLand") | LayerMask.GetMask("Plains") | LayerMask.GetMask("Desert");

        List<Collider> cols = new List<Collider>(Physics.OverlapSphere(selectedUnit.transform.position, selectedUnit.movePower * .75f, layerMask));

        Vector2Int startPos = CheckMyPos(); //클릭한유닛의 좌표.
        movableList.Clear();


        StartCoroutine(unitMoveStep(cols, startPos));
    }

    public JKH_Node shaderStorage;

    IEnumerator unitMoveStep(List<Collider> tiles, Vector2Int startPos)
    {
        InitMoveArea();

        List<JKH_Node> path = new List<JKH_Node>();

        for (int i = 0; i < tiles.Count; i++)
        {
            //---
            //List<Collider> tileOnUnit = new List<Collider>(Physics.OverlapSphere(cols[i].transform.position, .3f, unitLayer | cityLayer));
            //if (tileOnUnit.Count >= 1 && (unit.playerId == selectedUnit.playerId)) //상대방유닛은 안거르게됨.
            //{
            //    //if (tileOnUnit[0].gameObject.name.Contains("Castle"))
            //    continue;
            //}
            //--
            List<GameObject> objectOn = tiles[i].GetComponent<TerrainData>().objectOn;
            List<Unit> units = new List<Unit>();

            for (int j = 0; j < objectOn.Count; ++j)
            {
                units.Add(objectOn[j].GetComponentInChildren<Unit>(true));
                
            }

            if (units.Count > 0 && units[0].playerId == selectedUnit.playerId)
            {
                print(units[0].gameObject.name);
                continue;
            }

            //if (tileOnUnit.Length >= 1 && tileOnUnit[0].GetComponent<TerrainData>().objectOn == "City")
            //{
            //    //if (tileOnUnit.Length >= 1 && tileOnUnit[0].GetComponent<Building>().tag == "City")
            //    {
            //        print("city잡힘");
            //    }
            //}

            //tileOnUnit.Initialize();
            TerrainData terrainData = tiles[i].GetComponent<TerrainData>();
            Vector2Int endPos = new Vector2Int(terrainData.x, terrainData.y);
            print(endPos);
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

            float movePower = 0;
            //path값 Null...
            string pathStr = string.Format("({0}, {1})", path[0].gridX, path[0].gridY);
            for (int j = 1; j < path.Count; ++j)
            {
                movePower += path[j].requiredMovePower;
            }
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

            shaderStorage = movableList[i];
            while (shaderStorage.parent != null)
            {
                shaderStorage = shaderStorage.parent;
            }
            int x = shaderStorage.gridX;
            int y = shaderStorage.gridY;

            Material material = terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material;
            material.shader = Shader.Find("Custom/OutlineShader");
            material.SetColor("_OutlineColor", Color.white);
            terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material = material;
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

                // 이미 dest.parent=null
                // print(new Vector2(dest.gridX, dest.gridX)); //가능한좌표 표시한다
                // ||hitInfo.transform.gameObject.layer==LayerMask.NameToLayer("City")

                //MoveMark표시
                if (Physics.Raycast(ray, out hitInfo, 1000, mapLayer))
                {
                    if (hitInfo.transform.gameObject.tag == "Map")
                    {

                        Collider[] tileOnUnit = Physics.OverlapSphere(hitInfo.transform.position, .3f, unitLayer|cityLayer);
                        if (tileOnUnit.Length > 0 && (tileOnUnit[0].GetComponent<Unit>().playerId != selectedUnit.playerId)) //|| tileOnUnit[0].GetComponent<Building>().tag==("City")
                        {
                            enemyMark.SetActive(true);
                            Vector3 enemyMarkPos = hitInfo.transform.position;
                            enemyMarkPos.y += .2f;
                            enemyMark.transform.position = enemyMarkPos;
                        }
                        else
                            enemyMark.SetActive(false);

                        //moveMark Pos 표시...
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
                    }
                }

                else
                    lr.positionCount = 0;
            }
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
                            InitMoveArea();
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
                                InitMoveArea();

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
                                InitMoveArea();
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

            if (Physics.Raycast(ray, out hitInfo))
            {
                TerrainData data = hitInfo.transform.gameObject.GetComponent<TerrainData>();
                if (nodeMap[data.x + (data.y * mapWidth)].walkable)
                {
                    int idx = terrainDataMap.IndexOf(data);
                    neighbours.Add(nodeMap[idx]);
                }
                //해당 x,y좌표 저장
            }
        }

        // int originX = node.gridX;
        // int originY = node.gridY;

        // neighbours.Add(nodeMap[(originX - 1) + (originY - 1) * mapWidth]);
        // neighbours.Add(nodeMap[(originX) + (originY - 1) * mapWidth]);
        // neighbours.Add(nodeMap[(originX + 1) + (originY - 1) * mapWidth]);

        // neighbours.Add(nodeMap[(originX - 1) + (originY) * mapWidth]);
        // neighbours.Add(nodeMap[(originX + 1) + (originY) * mapWidth]);

        // neighbours.Add(nodeMap[(originX) + (originY + 1) * mapWidth]);

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
    void UnitCombat(CombatUnit unit, Unit enemy)
    {
        if (unit == null)
            return;
        print("이함수 실행");

        //아군 유닛 변수
        //float unitHp = unit.hp;
        float unitMeleeDmg = unit.meleeAttack;
        float unitRangeDmg = unit.rangeAttack;
        float enemyMeleeDmg = 0;
        //float enemyRangeDmg = 0;
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
            HexFogManager.instance.findTargetList[enemy.gameObject.GetComponent<Unit>().playerId].Remove(enemy.gameObject.GetComponent<Hideable>());
            Destroy(enemy.gameObject);
            print("enemy cut");
        }

        if (unit.hp <= 0)
        {
            Destroy(unit.gameObject);
            print("ally cut");
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

        anim.SetBool("isMove", true);
        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_INFANTRY_WALK);

        //어떤경로로 이동하는지 표시
        while (path.parent != null)
        {

            yield return null;

            Vector3 unitPos = unit.transform.position;
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
                // 다음 타일로 변경
                path = path.parent;
            }

            //Line Renderer의 시작점= UnitPos
            if (lrCount > 1)
                for (int i = 0; i < lrCount; ++i)
                    lr.SetPosition(i, unitPos);


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

        }
        //경로표시 다끝나면 선 지운다.
        lr.positionCount = 0;
        anim.SetBool("isMove", false);

        if (onEnemy == true)
        {
            // overlapsphere path 타일에 있는 상대 유닛 찾아옴
            //Collider[] tileOnUnit = Physics.OverlapSphere(path.worldPosition, .3f, unitLayer);
            List<GameObject> tileOnUnit = terrainDataMap[path.gridX + (path.gridY * mapWidth)].objectOn;
            //print(path.gridX + ", " + path.gridY); 

            // TODO 전투 애니메이션 시작
            anim.SetBool("isMove", false);
            anim.SetBool("onCombat", true);
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
            {
                yield return null;
            }
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                print("anim");
                yield return null;
            }
            UnitCombat(selectedUnit.GetComponent<CombatUnit>(), tileOnUnit[0].GetComponent<Unit>());

            // TODO 전투 애니메이션 종료
            anim.SetBool("onCombat", false);

            //Todo위치시켜주기 else도 마찬가지로 시도본다.?
        }

        // TODO 애니메이션 중간에 뒤돔
        unit.transform.forward = Vector3.back;

        //유닛 위치 타일에 다시 저장
        unit.CheckMyPos();

    }

    //함수이름 바꾸기,
    public void InitMoveArea()
    {
        // 선 제거. 여기서 path.parent null시킴
        for (int i = 0; i < movableList.Count; i++)
        {
            //이러면 최근데이터 불러와져서 안지워짐, 근데 다른 타일 누르면 지워짐
            //그래서 데이터따로 저장하고 불러와서 지운다.
            JKH_Node deleteShader = movableList[i];
            while (deleteShader.parent != null)
            {
                deleteShader = deleteShader.parent;
            }
            int x = deleteShader.gridX;
            int y = deleteShader.gridY;
            Material material = terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material;
            material.shader = Shader.Find("Standard");
            terrainDataMap[(y * mapWidth) + x].gameObject.GetComponent<MeshRenderer>().material = material;
        }
    }

    public void MarkEnabled()
    {
        Vector3 markPos = selectedUnit.transform.position;
        markPos.y += 0.1f;
        unitMark.transform.position = markPos;

        unitMark.SetActive(true);
    }

    //막 사라지지 않게 경우의수 추가
    public void MarkDisabled()
    {
        unitMark.SetActive(false);
        moveMark.SetActive(false);
        enemyMark.SetActive(false);
    }
}
