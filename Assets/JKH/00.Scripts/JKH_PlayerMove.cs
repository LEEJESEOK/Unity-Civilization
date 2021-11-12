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

        if (Input.GetButtonDown("Fire1"))
        {
            if(Physics.Raycast(ray,out hitinfo, 1000))
            {
                JKH_PlayerMove pm = hitinfo.transform.GetComponent<JKH_PlayerMove>();
                pm.isPlayerClick = true;
            }
        }

        if (isPlayerClick)
        {
            if(Physics.Raycast(ray, out hitinfo, 1000))
            {
                //Vector3
            }
        }
    }
}
