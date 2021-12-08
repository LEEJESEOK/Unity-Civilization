using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JKH_Move : MonoBehaviour
{
    Camera cam;
    public LayerMask layer;
    //public LayerMask ground;

    public GameObject moveBtn;

    //유닛 클릭한다
    public bool isUnitClick;
    public bool canMove;

    //Unit's Stat
    public int movePower;
    public int meleeAttack;
    public int Hp;
    public int RangeAttack;
    public int Range;

    public int startPosX;
    public int startPosY;
    public int endPosX;
    public int endPosY;

    //UI
    public Text moveUI;

    public static JKH_Move instance;

    //From RayMove

    //jkh_Grid..
    JKH_Grid grid;
    public int startX, startY, endX, endY;
    JKH_Node start;
    JKH_Node end;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        canMove = false;
        cam = Camera.main;

        grid = GetComponent<JKH_Grid>();

        //Set Coordinate

        //Just Check 맞게나옴.
        //print("start: "+start.gridX+", "+start.gridY);
        //print("startWorldPos: "+start.worldPosition);
        //print("end: " + end.gridX + ", " + end.gridY);
        //print("endWorldPos: "+end.worldPosition);



        //FindPath

        print(grid.path);
        for (int i = 0; i < grid.path.Count; ++i)
        {
            print(string.Format("x : {0}, y : {1}", grid.path[i].gridX, grid.path[i].gridY));
        }
    }

    void Update()
    {
        moveUI.text = "남은이동력: " + movePower;


        getUnitInfo();
        UnitMove();

        start = new JKH_Node(true, grid.grid[startX, startY].worldPosition, startX, startY);
        end = new JKH_Node(true, grid.grid[endX, endY].worldPosition, endX, endY);

        FindPath(start, end);
    }

    public void getUnitInfo()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //마우스 클릭한다
        if (Input.GetButton("Fire1"))
        {
            if (Physics.Raycast(ray, out hitInfo, 1000, layer))
            {
                //unitMove
                JKH_Move um = hitInfo.transform.GetComponent<JKH_Move>();
                um.isUnitClick = true;

                // 유닛 정보 출력(확인용)
                print("이동력: " + movePower);
                print("공격력: " + meleeAttack);
                print("체력: " + Hp);
                print("원거리공격력: " + RangeAttack);
                print("사거리: " + Range);

                //유닛이있는 타일의 정보를 가져온다.
                GetTileInfo();
            }
        }

        if (isUnitClick == true)
        {
            //버튼이 나온다
            moveBtn.SetActive(true);
        }
    }

    //get Tile's Info
    public void GetTileInfo()
    {
        float radius = 0.05f;
        Collider[] maps = Physics.OverlapSphere(transform.position, radius, ~layer);
        if (maps.Length == 1)
        {
            if (maps[0].gameObject.tag == "Map")
            {
                print(maps[0].GetComponent<TerrainData>().x + ", " + maps[0].GetComponent<TerrainData>().y);
                startPosX = maps[0].GetComponent<TerrainData>().x;
                startPosY = maps[0].GetComponent<TerrainData>().y;
            }
        }
    }

    //버튼을 누르면
    public void onClickMoveBtn()
    {
        canMove = true;
        print("눌림");

        //startPos가 된다.

    }


    //바닥클릭하면 이동력을 소모하고 이동한다
    public void UnitMove()
    {
        if (canMove)
        {
            print("canMove");

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;


            //마우스가 위치한 좌표.
            if (Physics.Raycast(ray, out hitInfo, 1000))
            {

                if (hitInfo.transform.gameObject.tag == "Map")
                {
                    print("맵맞추는듕");
                    //이러면서? 마우스가 맵 위에 올리면 1) 해당 좌표랑 V 2) 이동력 계산하기 X 하면 좋겠ㄴㅔ...
                    //PathFinding 하기, 실시간 이동력 계산 
                    // Node[] path = pathfinding(start, end);
                    //Matrix4x4 
                    // path[0] path[1] ... path[count-1]


                    print("마우스에 위치한 좌표" + hitInfo.transform.GetComponent<TerrainData>().x + ", " + hitInfo.transform.GetComponent<TerrainData>().y);
                    if (Input.GetButton("Fire1"))
                    {
                        endPosX = hitInfo.transform.GetComponent<TerrainData>().x;
                        endPosY = hitInfo.transform.GetComponent<TerrainData>().y;
                        print("EndNewPos" + endPosX + ", " + endPosY);
                        print("StartNewPos" + startPosX + ", " + startPosY);

                        //Start


                        //이동량이 있다. (이동하는것)
                        if (movePower > 0)
                        {
                            transform.position = hitInfo.transform.position;
                            //가라앉는거 수정한다.
                            canMove = false;
                            //목표 타일에 따라 이동력 감소량 다르게 한다.                  
                            movePower--;
                            print("이동완료!");



                        }

                        else if (movePower <= 0)
                        {
                            print("못움직임");
                            canMove = false;
                        }
                    }
                }
            }

            #region Fire2
            //마우스 클릭한다
            if (Input.GetButton("Fire2")) //XXXXXXX
            {
                print("클릭확인");
                //레이 추가? 해야하겠주?

                if (Physics.Raycast(ray, out hitInfo, 1000))
                {
                    print("눌린거확인");

                    print(AutoMapTile.instance.tiles[0].GetComponent<TerrainData>().x + " ," +
                    AutoMapTile.instance.tiles[0].GetComponent<TerrainData>().y);


                    // 목적지 클릭했다. 대신에 무브 버튼을 누른다음에 마우스가 가르키는 타일의 거리 구한다
                    // ==현재위치에서 목적지까지에 필요한 이동력을 계산한다
                    // == 이동력 얼마나 소모하는지 표시한다
                    // ↓↓↓↓↓↓↓↓↓↓
                    // ★ A* 알고리즘 이용 ★
                    // Needs
                    // 각 타일의 좌표 및 정보(eg.이동력 소모값, 이동할수있는 타일인가 아닌가(벽같은개념)) 
                    // -타일의 좌표
                    // 클릭한타일을 지정해서 가져온다.
                    // 시작점(시작점 제외)에서 목적지까지의 최단거리를 구한다(위내용과 같음)
                    // 시작점(시작점 제외)에서 목적지까지의 거쳐간 길들의 이동력을 더한다                    
                    // @@@@@@
                    // 만약 계산된 이동력이 0보다 크다면 이동한다
                    // 계산된 이동력이 0뵤다 작다면 이동하지 않는다
                    //
                    //


                    //이동량이 있다. (이동하는것)
                    if (movePower > 0)
                    {
                        transform.position = hitInfo.transform.position;
                        //가라앉는거 수정한다.
                        canMove = false;
                        //목표 타일에 따라 이동력 감소량 다르게 한다.
                        movePower--;
                        print("이동완료!");


                    }

                    else if (movePower <= 0)
                    {
                        print("못움직임");
                        canMove = false;
                    }

                }
            }
            #endregion

        }

    }



    //Get "DrawRay & ray" Function from RayMove_Script ++++
    public void checkNeighbour()
    {
        RaycastHit hitInfo;
        Quaternion originRotation = transform.rotation;


        for (int i = 0; i < 6; i++)
        {

            //Vector3 tmp = new Vector3(2 * Mathf.Sin((60 * i) * (Mathf.PI / 180)), 
            //    0, 2 * Mathf.Cos((60 * i) * (Mathf.PI / 180)));

            //Vector3 dir = transform.position - tmp;
            //dir.y = 0;
            //print(dir);
            //Ray ray = new Ray(transform.position, dir);
            Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
            var pos = transform.position;
            pos.y = -.95f;
            Ray ray = new Ray(pos, rotation * transform.forward);

            //int layer = 1 << LayerMask.NameToLayer("TestCube");

            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                print("쏜다?");
                if (hitInfo.transform.gameObject)
                {

                    //해당 좌표를 가져온다.
                    //Script xxx =GameObject.Find("xxx").GetComponent<Script>();
                    if (hitInfo.transform.gameObject.tag == "Map")
                    {
                        print(i + "번째에서, " + hitInfo.transform.GetComponent<TerrainData>().x +
                            ", " + hitInfo.transform.GetComponent<TerrainData>().y);

                        //이웃의 이동력도 불러온다!!!
                        print("이동력: " + hitInfo.transform.GetComponent<TerrainData>().output.movePower);
                        //print("이웃타일의 이동력: "+ hitInfo.transform.GetComponent<TerrainData>().)

                        #region ToDo
                        //needs
                        //시작지점의 좌표>> Move 스크립트에서, 버튼 눌렀을때 가져온다
                        //목적지의 좌표>> 클릭된좌표가 목적지의 좌표가 된다. 
                        //본인의 위치의 g(n)과 이웃의 g(n) (자신의 G + 이웃의 이동력  ) 계산
                        //갈수있는 타일인가? BOOL (이미 왔던데, 벽 은 못움직이니까 제외)
                        //----
                        //g(n)= 시작지점부터 현위치까지 이동한거리
                        //h(n)= 현위치에서 목적지까지의 거리
                        //f(n)= g(n)+h(n)

                        //이웃의 f(n) 점수를 합산해서 제일 낮은 위치로 이동
                        //목적지에 도달할떄 까지 반복한다.
                        //-----------
                        //다시 되돌아가서 계산하는거 해야하는데 이거 어케함? 나중에하자
                        #endregion

                    }

                }
            }
        }
    }

    public void DrawRay()
    {
        Quaternion originRotation = transform.rotation;
        for (int i = 0; i < 6; i++)
        {
            Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
            var pos = transform.position;
            pos.y = -.95f; //잘 조절 해야함...
            Ray ray = new Ray(pos, rotation * transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 15, Color.red);
        }
    }


    // 경로 계산
    // 인덱스 배열을 반환
    public void FindPath(JKH_Node start, JKH_Node end)
    {
        //내가추가-------
        List<JKH_Node> openSet = new List<JKH_Node>();
        HashSet<JKH_Node> closeSet = new HashSet<JKH_Node>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            //currentNode=currentNode.
            JKH_Node currentNode = openSet[0];

            //시작지점 openSet=0
            for (int i = 1; i < openSet.Count; i++)
            {
                //만족한 값으로 이동.
                if ((openSet[i].fCost < currentNode.fCost)
                    || (openSet[i].fCost == currentNode.fCost)
                    && (openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            //if (currentNode == end)
            if (currentNode.gridX == end.gridX && currentNode.gridY == end.gridY)
            {
                grid.path = RetracePath(currentNode);

                return;
            }

            //이웃노드 검사한다.
            foreach (JKH_Node neighbour in grid.GetNeighboursAdd(currentNode))
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

        //------
        return;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (grid != null)
            for (int i = 0; i < grid.path.Count; ++i)
            {
                Gizmos.DrawCube(grid.path[i].worldPosition, Vector3.one * 0.5f);
            }
    }
}