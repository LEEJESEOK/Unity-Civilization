using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_Node
{
    public bool walkable;
    public Vector3 worldPosition;

    //데이터용 좌표
    public int gridX;
    public int gridY;

    public float gCost;
    public float hCost;

    //movePower 이동력 추가
    public float movePower;
    
    //경로저장용 == 발자취 (linked list)
    public JKH_Node parent;

    public JKH_Node()
    {
        parent = null;
    }

    public JKH_Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        //movePower = _movePower;
        
    }

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}

