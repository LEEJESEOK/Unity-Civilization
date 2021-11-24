using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_Agent : MonoBehaviour
{
    private SimplePF2D.Path path;
    private Rigidbody rb;
    private Vector3 nextPoint;
    private float speed = 5.0f;
    private bool isStationary = true;

    private void Start()
    {
        path = new SimplePF2D.Path(GameObject.Find("Grid").GetComponent<SimplePathFinding2D>());
        rb = GetComponent<Rigidbody>();
        nextPoint = Vector3.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //mouseWorldPos.z = .0f;
            path.CreatePath(transform.position, mouseWorldPos);
        }


        if(path.IsGenerated())
        {
            if(isStationary)
            {
                if(path.GetNextPoint(ref nextPoint))
                {
                    rb.velocity = nextPoint - transform.position;
                    rb.velocity = rb.velocity.normalized;
                    rb.velocity *= speed;
                    isStationary = false;
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    isStationary = true;
                }
            }
            else
            {
                Vector3 delta = nextPoint - transform.position;
                if (delta.magnitude <= .2f)
                {
                    rb.velocity = Vector3.zero;
                    isStationary = true;
                }
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
            isStationary = true;
        }
    }
}
