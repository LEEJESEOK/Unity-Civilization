using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_MousePath : MonoBehaviour
{
    private SimplePF2D.Path path;

    private void Start()
    {
        SimplePathFinding2D pf = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        path = new SimplePF2D.Path(pf);
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0.0f;

            path.CreatePath(new Vector3(0f, 0f, 0f), mouseWorldPos);

        }
    }
}
