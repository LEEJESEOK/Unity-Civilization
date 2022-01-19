using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_UnitMove : MonoBehaviour
{
    Camera cam;
    public GameObject moveBtn;
    public LayerMask layer;
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

    //유닛 클릭한다
    public bool isUnitClick;
    public bool canMove;

    //유닛이 갈수 있는가 없는가 체크한다.
    //public bool ableMove;


    void Start()
    {
        cam = Camera.main;

    }

    void Update()
    {
        getUnitInfo();
        UnitMove();
    }


    //JKH_Move Scripts
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
                JKH_UnitMove um = hitInfo.transform.GetComponent<JKH_UnitMove>();
                um.isUnitClick = true;

                // 유닛 정보 출력(확인용)
                print("이동력: " + movePower);
                print("공격력: " + meleeAttack);
                print("체력: " + Hp);
                print("원거리공격력: " + RangeAttack);
                print("사거리: " + Range);

                //유닛이있는 타일의 정보를 가져온다.


            }
        }

        if (isUnitClick == true)
        {
            //버튼이 나온다
            moveBtn.SetActive(true);
        }
    }

    public void GetTileInfo()
    {
        float radius = 0.05f;
        Collider[] maps = Physics.OverlapSphere(transform.position, radius, ~gameObject.layer);
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

    public void onClickMoveBtn()
    {
        canMove = true;
        print("눌림");

        //startPos가 된다.

    }


    //바닥클릭하면 이동력을 소모하고 이동한다
    public void UnitMove()
    {
        if (isUnitClick)
        {
            print("canMove");
            //JKH_Move.instance.FindPath();
            checkAbleToGo();

            //JKH_Move.instance.FindPath();
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




    private void JustForNote()
    {
        //start X,Y= 유닛의좌표 
        //hitInfo.transform.GetComponent<TerrainData>().x => startX
        //hitInfo.transform.GetComponent<TerrainData>().y => startY
    }

    //갈수있는 타일 받기 
    public void checkAbleToGo()
    {
        LayerMask layerMask = LayerMask.GetMask("GrassLand") | LayerMask.GetMask("Plains") | LayerMask.GetMask("Desert");

        Collider[] cols = Physics.OverlapSphere(transform.position, movePower, layerMask);
        //float[] x = new float[cols.Length];

        KeyValuePair<int, int> mypos = CheckMyPos();
        JKH_Node start = new JKH_Node(JKH_Move.instance.grid.grid[mypos.Key, mypos.Value].walkable,
            transform.position, mypos.Key, mypos.Value, JKH_Move.instance.grid.grid[mypos.Key, mypos.Value].requiredMovePower);

        for (int i = 0; i < cols.Length; i++)
        {

            TerrainData terrainData = cols[i].GetComponent<TerrainData>();
            //print(CheckMyPos());
            int x = terrainData.x;
            int y = terrainData.y;

            JKH_Node end = new JKH_Node(JKH_Move.instance.grid.grid[x, y].walkable,
                JKH_Move.instance.grid.grid[x, y].worldPosition, x, y, JKH_Move.instance.grid.grid[x, y].requiredMovePower);

            JKH_Node path = JKH_Move.instance.FindPath(start, end);
            float movePower = 0;
            
            print(string.Format("-> x : {0}, y : {1}", path.gridX, path.gridY));

            //while (path.parent != null)
            //    movePower += path.requiredMovePower;

            print(movePower);
            if (movePower > this.movePower)
                print(string.Format("-> x : {0}, y : {1}", x, y));
        }

             
    }

    public KeyValuePair<int, int> CheckMyPos()
    {
        Vector2 pos = new Vector2();
        int fogLayer = LayerMask.GetMask("HexFog");

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);
        if (Physics.Raycast(ray, out hit, 1, ~fogLayer))
        {
            GameObject myTilePos = hit.transform.gameObject;
            pos.x = myTilePos.GetComponent<TerrainData>().x;
            pos.y = myTilePos.GetComponent<TerrainData>().y;
        }

        return new KeyValuePair<int, int>((int)pos.x, (int)pos.y);

    }
}
