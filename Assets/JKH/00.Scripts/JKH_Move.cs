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
    }

    void Update()
    {
        moveUI.text = "남은이동력: " + movePower;


        getUnitInfo();
        UnitMove();
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

                //gameobj위치 기준으로 레이 조금 쏜다 
                //만약 충돌한게 tag로 확인해서 map이 있나본다
                //있다면, 맵의 terraindata에xy가져온다 idx를
                float radius = 0.05f;
                Collider[] maps = Physics.OverlapSphere(transform.position, radius, ~layer);
                if (maps.Length == 1)
                {
                    if (maps[0].gameObject.tag == "Map")
                    {
                        print(maps[0].GetComponent<TerrainData>().x + ", " + maps[0].GetComponent<TerrainData>().y);
                        startPosX= maps[0].GetComponent<TerrainData>().x ;
                        startPosY= maps[0].GetComponent<TerrainData>().y ;
                    }
                }
            }
        }

        if (isUnitClick == true)
        {
            //버튼이 나온다
            moveBtn.SetActive(true);
        }
    }

    //버튼을 누르면
    public void onClickMoveBtn()
    {
        canMove = true;
        print("눌림");        
        
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
                print("쏘고있나");
                if (hitInfo.transform.gameObject.tag == "Map")
                {
                    //print("마우스에 위치한 좌표" + hitInfo.transform.GetComponent<TerrainData>().x + ", " + hitInfo.transform.GetComponent<TerrainData>().y);
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



    //@@@@@@@@@@@@@@@@@@@@@@@@@@@
    public void AMove()
    {
        //
    }
}
