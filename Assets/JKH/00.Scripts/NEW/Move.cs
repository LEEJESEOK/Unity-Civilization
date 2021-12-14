using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    Camera cam;
    public LayerMask layer;

    public GameObject moveBtn;

    //유닛 클릭한다
    public bool isUnitClick;
    public bool canMove;

    //Unit's Stat
    public int movePower;

    //UI
    public Text moveUI;

    public static Move instance;

    //jkh_Grid..
    public JKH_Grid grid;
    public int startX, startY, endX, endY;
    JKH_Node start;
    JKH_Node end;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
