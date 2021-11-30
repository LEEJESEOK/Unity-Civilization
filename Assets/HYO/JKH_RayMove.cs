using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_RayMove : MonoBehaviour
{
    void Start()
    {
        //rayShot();
    }

    void Update()
    {
        RayShot();
    }
    public void RayShot()
    {
        RaycastHit hitInfo;
        Quaternion originRotation = transform.rotation;
       

        for (int i = 0; i < 6; i++)
        {

            Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
            var pos = transform.position;
            pos.y = -0.95f;
            Ray ray = new Ray(pos, rotation * transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 15, Color.red);

            if (Physics.Raycast(ray, out hitInfo, 1000))
            {
                if (hitInfo.transform.gameObject)
                {
                    //해당 좌표를 가져온다.
                    //Script xxx =GameObject.Find("xxx").GetComponent<Script>();
                    //if (hitInfo.transform.gameObject.tag == "Map")
                    {
                        print(i +"번째에서, "+hitInfo.transform.GetComponent<TerrainData>().x + 
                            ", " + hitInfo.transform.GetComponent<TerrainData>().y);

                        //이웃의 이동력도 불러온다!!!
                        print("이동력: "+hitInfo.transform.GetComponent<TerrainData>().output.movePower);
                        //print("이웃타일의 이동력: "+ hitInfo.transform.GetComponent<TerrainData>().)


                        //needs
                        //시작지점의 좌표 
                        //목적지의 좌표
                        //본인의 위치의 g(n)과 이웃의 g(n) (자신의 G + 이웃의 이동력  ) 계산
                        //갈수있는 타일인가? BOOL (이미 왔던데, 벽 은 못움직이니까 제외)
                        //----
                        //g(n)= 시작지점부터 현위치까지 이동한거리
                        //h(n)= 현위치에서 목적지까지의 거리
                        //f(n)= g(n)+h(n)
                        
                        //이웃의 f(n) 점수를 합산해서 제일 낮은 위치로 이동
                        //목적지에 도달할떄 까지 반복한다.





                    }

                }
            }
        }
    }

    //Draw the Ray from thea mid
    //void DrawRay()
    //{
    //    Quaternion originRotation = transform.rotation;
    //    for (int i = 0; i < 6; i++)
    //    {
    //        //Vector3 tmp = new Vector3(2 * Mathf.Sin((60 * i) * (Mathf.PI / 180)),
    //        //    0, 2 * Mathf.Cos((60 * i) * (Mathf.PI / 180)));

    //        Quaternion rotation = originRotation * Quaternion.Euler(0, i * 60, 0);
    //        var pos=transform.position;
    //        pos.y += .05f;
    //        //방향에 각도를 조절해준다(각도, 방향)
    //        Ray ray = new Ray(pos, rotation * transform.forward);
    //        Debug.DrawRay(ray.origin, ray.direction * 15, Color.red);

    //        //Vector3 dir = transform.position - tmp;
    //        //dir.y = 0;
    //        //Debug.DrawRay(transform.position, dir * 15, Color.red);
    //        //Ray ray = new Ray(transform.position, dir);
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
