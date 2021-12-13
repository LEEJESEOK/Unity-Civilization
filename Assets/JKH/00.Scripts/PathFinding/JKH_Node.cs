using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[Serializable]
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
    public float requiredMovePower;

    //경로저장용 == 발자취 (linked list)
    public JKH_Node parent;

    public JKH_Node()
    {
        parent = null;
    }

    public JKH_Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, float _requiredMovePower)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        requiredMovePower = _requiredMovePower;

    }

    public float fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public override string ToString()
    {
        string result = JsonUtility.ToJson(this, true);

        return result;
    }
}

