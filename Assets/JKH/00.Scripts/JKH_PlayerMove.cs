using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_PlayerMove : MonoBehaviour
{
    Camera cam;

    public LayerMask layer;

    public bool isPlayerClick;
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitinfo;

        //마우스 왼쪽 클릭한다
        if (Input.GetButtonDown("Fire1"))
        {
            if(Physics.Raycast(ray,out hitinfo, 1000, layer))
            {
                JKH_PlayerMove pm = hitinfo.transform.GetComponent<JKH_PlayerMove>();
                pm.isPlayerClick = true;
            }
        }

        if (isPlayerClick)
        {
            if(Physics.Raycast(ray, out hitinfo, 1000, ~layer))
            {
                //위치가 계속 변해서 Y좌표 fix한다
                Vector3 vec = new Vector3(hitinfo.point.x,
                    -0.74f , hitinfo.point.z);
                transform.position = vec;
            }
        }

        //마우스 클릭 떼면 변경된 자리로 둔다
        if (Input.GetButtonUp("Fire1") && isPlayerClick)
        {
            isPlayerClick = false;
        }
    }
}
