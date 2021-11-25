using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_RayMove : MonoBehaviour
{
    //\
    
    //Ray 6ways

    void Start()
    {
        rayShot();
    }

    void Update()
    {
        DrawRay();
        //rayShot();
    }
    public void rayShot()
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
            pos.y = .05f;
            Ray ray = new Ray(pos, rotation * transform.forward);

            int layer = 1 << LayerMask.NameToLayer("TestCube");
            //오버렙스피어
            if (Physics.Raycast(ray, out hitInfo, 1000))
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

    //Draw the Ray from thea mid
    void DrawRay()
    {
        Quaternion originRotation = transform.rotation;
        for (int i = 0; i < 6; i++)
        {
            //Vector3 tmp = new Vector3(2 * Mathf.Sin((60 * i) * (Mathf.PI / 180)),
            //    0, 2 * Mathf.Cos((60 * i) * (Mathf.PI / 180)));

            Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
            var pos=transform.position;
            pos.y += .05f;
            //방향에 각도를 조절해준다(각도, 방향)
            Ray ray = new Ray(pos, rotation * transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 15, Color.red);

            //Vector3 dir = transform.position - tmp;
            //dir.y = 0;
            //Debug.DrawRay(transform.position, dir * 15, Color.red);
            //Ray ray = new Ray(transform.position, dir);
        }
    }

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
