using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimplePF2D;

// Follows a set path backward and forward that can be shared
// between multiple objects of this type
public class Ant : MonoBehaviour
{
    public bool UsesDynamicPathFlag = false;
    private static Path antPath;
    private static Vector3 nestTarget;
    private static Vector3 foodTarget;
    private static bool init = false;
    private Vector3 nextPoint;
    private Rigidbody2D rb;
    private int currentindex;
    private float speed;
    private bool isStationaryFlag;
    private bool isGoingForward;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isStationaryFlag = true;
        isGoingForward = true;
        speed = 9.0f;
        currentindex = 0;
        // !!!!! Create these two GameObjects in the editor. These will act as the two positions the path will be generated between !!!!!
        nestTarget = GameObject.Find("NestTarget").transform.position;
        foodTarget = GameObject.Find("FoodTarget").transform.position;

        // This path is a static variable. Thus is shared by all instances of the Ant class.
        antPath = new Path(GameObject.Find("Grid").GetComponent<SimplePathFinding2D>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (init && antPath.IsGenerated())
        {
            if (isStationaryFlag)
            {
                bool endPath = false;
                if (isGoingForward)
                {
                    // Gets the next point in the path based on a custom index (currentindex)
                    endPath = antPath.GetNextPointIndex(ref nextPoint, ref currentindex);
                }
                else
                {
                    // Gets the previous point in the path based on a custom index (currentindex)
                    endPath = antPath.GetPreviousPointIndex(ref nextPoint, ref currentindex);
                }

                if (endPath)
                {
                    rb.velocity = nextPoint - transform.position;
                    rb.velocity = rb.velocity.normalized;
                    rb.velocity *= speed;
                    isStationaryFlag = false;
                }
                else // The path failed or we reached the end.
                {
                    isGoingForward = !isGoingForward;
                    rb.velocity = Vector3.zero;
                }
            }
            else
            {
                Vector3 delta = nextPoint - transform.position;
                if (delta.magnitude <= 0.25f)
                {
                    rb.velocity = Vector3.zero;
                    isStationaryFlag = true;
                }
            }
        }
        else if (!init)
        {
            init = true;
            // This is here to ensure this path is only created once, so that one path can
            // be shared by all the Ants.
            antPath.CreatePath(nestTarget, foodTarget, true, UsesDynamicPathFlag);
        }
    }
}
