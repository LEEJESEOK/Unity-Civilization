using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_RayMove : MonoBehaviour
{
    
    //레이 6방향으로 쏜다.

    void Start()
    {
        rayShot();
    }

    // Update is called once per frame
    void Update()
    {
        //DrawRay();
        //rayShot();
    }
    public void rayShot()
    {
        RaycastHit hitInfo;

        for (int i = 0; i < 6; i++)
        {
            Vector3 tmp = new Vector3(2 * Mathf.Sin((60 * i) * (Mathf.PI / 180)),
                0, 2 * Mathf.Cos((60 * i) * (Mathf.PI / 180)));

            Vector3 dir = transform.localPosition - tmp;
            print(dir);
            Ray ray = new Ray(transform.position, dir);
            int layer = 1 << LayerMask.NameToLayer("TestCube");
            //오버렙스피어
            if (Physics.Raycast(ray, out hitInfo, 1000,layer))
            {
                if (hitInfo.transform.gameObject)
                {
                    print("ㅎㅇ");
                    print(i);
                    print(hitInfo.transform.gameObject.name);
                }
            }
        }
    }

    //public Vector3 tmp;
    //void DrawRay()
    //{
    //    for (int i = 0; i < 6; i++)
    //    {
    //        Debug.DrawRay(transform.position, dir * 15, Color.red);
    //        Ray ray = new Ray(transform.position, dir);
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i < 6; i++)
    //    {


    //        Quaternion qDir = Quaternion.Euler(0,0,1);
    //        Quaternion diff = Quaternion.Euler(0, i * 60, 0);
    //        Vector3 dir = (qDir * diff).eulerAngles;

    //        Gizmos.color = Color.red;

    //        Gizmos.DrawLine(transform.position, transform.position + dir);
    //    }
    //}
}
