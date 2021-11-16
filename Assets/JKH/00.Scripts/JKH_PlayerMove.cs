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

        //���콺 ���� Ŭ���Ѵ�
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
                //��ġ�� ��� ���ؼ� Y��ǥ fix�Ѵ�
                Vector3 vec = new Vector3(hitinfo.point.x,
                    -0.74f , hitinfo.point.z);
                transform.position = vec;
            }
        }

        //���콺 Ŭ�� ���� ����� �ڸ��� �д�
        if (Input.GetButtonUp("Fire1") && isPlayerClick)
        {
            isPlayerClick = false;
        }
    }
}
