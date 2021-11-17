using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
    public Collider[] getTerritory;
    void Start()
    {
        float maxDistance = 0f;
        int radius = 1;
        RaycastHit hit;

        // Physics.SphereCast (레이저를 발사할 위치, 구의 반경, 발사 방향, 충돌 결과, 최대 거리)
        //bool isHit = Physics.SphereCast(transform.position, 1, transform.up, out hit, maxDistance);
        getTerritory = Physics.OverlapSphere(transform.position, radius);
       
    }

    void Update()
    {
        
    }
}
