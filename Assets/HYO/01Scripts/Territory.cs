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

        // Physics.SphereCast (�������� �߻��� ��ġ, ���� �ݰ�, �߻� ����, �浹 ���, �ִ� �Ÿ�)
        //bool isHit = Physics.SphereCast(transform.position, 1, transform.up, out hit, maxDistance);
        getTerritory = Physics.OverlapSphere(transform.position, radius);
       
    }

    void Update()
    {
        
    }
}
