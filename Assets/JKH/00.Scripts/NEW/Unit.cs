using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //SelectUnit: [Move] Script에서 레이케스트로 유닛정보 받아올 함수? 를만든다

    Camera cam;
    public LayerMask layer;
    public bool isUnitClick;
    public GameObject moveBtn;


    //Unit's stat
    public int movePower;
    public int meleeAttack;
    public int Hp;
    public int RangeAttack;
    public int Range;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //유닛에대한 정보를 가져오는 함수
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
                Unit um = hitInfo.transform.GetComponent<Unit>();
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
}
