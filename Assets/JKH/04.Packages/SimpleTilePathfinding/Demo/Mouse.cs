using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple example of path creation.
// Makes a path from the 0,0,0 position to wherever the user clicks on the screen.
// Attach this script to a GameObject after setting up a grid.
public class Mouse : MonoBehaviour
{
    private SimplePF2D.Path path; 

    // Start is called before the first frame update
    void Start(){
        // Find the SimplePathFinding2D component attached to a GameObject with a Grid component.
        SimplePathFinding2D simplePathFinding = GameObject.Find("Grid").GetComponent<SimplePathFinding2D>();
        // Create a new path using this SimplePathFinding2D object.
        path = new SimplePF2D.Path(simplePathFinding);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            // Get the world position of the user's click
            Vector3 mouseworldpos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseworldpos.z = 0.0f;

            // Create a path from the 0,0,0 position to the mouseworldpos.
            path.CreatePath(new Vector3(0.0f, 0.0f, 0.0f), mouseworldpos);
            // To see this path, turn on DebugDrawPaths on the SimplePathFinding2D component on the Grid GameObject.
        }


    }
}
