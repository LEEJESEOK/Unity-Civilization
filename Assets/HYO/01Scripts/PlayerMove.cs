using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5;
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dirAD = transform.right * h;
        Vector3 dirWS = transform.forward * v;
        Vector3 dir = dirAD + dirWS;
        dir.Normalize();

        transform.position += dir * speed * Time.deltaTime;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1,Color.red);
        if (Physics.Raycast(ray, out hit, 1))
        {
            //Debug.Log(hit.transform.gameObject.layer);
        }
        

    }
}
