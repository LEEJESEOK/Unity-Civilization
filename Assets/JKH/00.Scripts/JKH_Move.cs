using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JKH_Move : MonoBehaviour
{
    Camera cam;
    public LayerMask layer;
    public LayerMask ground;

    public GameObject moveBtn;

    //유닛 클릭한다
    public bool isUnitClick;
    public bool canMove;

    //Unit's Stat
    public int move;
    public int meleeAttack;
    public int Hp;
    public int RangeAttack;
    public int Range;

   


    void Start()
    {
        canMove = false;
        cam = Camera.main;
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //마우스 클릭한다
        if (Input.GetButton("Fire1"))
        {
            if(Physics.Raycast(ray,out hitInfo, 1000, layer))
            {
                //unitMove
                JKH_Move um = hitInfo.transform.GetComponent<JKH_Move>();
                um.isUnitClick = true;

                // 유닛 정보 출력(확인용)
                print("이동력: " + move);
                print("공격력: " + meleeAttack);
                print("체력: " + Hp);
                print("원거리공격력: " + RangeAttack);
                print("사거리: " + Range);
            }
        }

        if (isUnitClick == true)
        {
            //버튼이 나온다
            moveBtn.SetActive(true);
        }

        UnitMove();
    }
    
    //버튼을 누르면
    public void onClickMoveBtn()
    {
        canMove = true;
        print("눌림");
    }

    public void UnitMove()
    {
        if (canMove)
        {
            print("canMove");

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            //마우스 클릭한다
            if (Input.GetButton("Fire1"))
            {
                print("클릭확인");
                //레이 추가? 해야하겠주?
                if (Physics.Raycast(ray, out hitInfo, 1000))
                {
                    print("눌린거확인");
                    transform.position = hitInfo.transform.position;
                    //가라앉는거 수정한다.
                    canMove = false;

                }
            }
        }
        
    }
}
