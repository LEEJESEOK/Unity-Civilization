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

    public Vector2 GetPosition()
    {
        return new Vector2(gridX, gridY);
    }

    public override bool Equals(object obj)
    {
        return obj is JKH_Node node &&
               walkable == node.walkable &&
               worldPosition.Equals(node.worldPosition) &&
               gridX == node.gridX &&
               gridY == node.gridY &&
               requiredMovePower == node.requiredMovePower;
    }

    public override int GetHashCode()
    {
        int hashCode = 1865475001;
        hashCode = hashCode * -1521134295 + walkable.GetHashCode();
        hashCode = hashCode * -1521134295 + worldPosition.GetHashCode();
        hashCode = hashCode * -1521134295 + gridX.GetHashCode();
        hashCode = hashCode * -1521134295 + gridY.GetHashCode();
        hashCode = hashCode * -1521134295 + requiredMovePower.GetHashCode();
        return hashCode;
    }

    public override string ToString()
    {
        string result = JsonUtility.ToJson(this, true);

        return result;
    }

    public static bool operator ==(JKH_Node a, JKH_Node b)
    {
        if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
            return true;
        if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            return false;

        if (a.walkable != b.walkable) return false;
        if (a.worldPosition != b.worldPosition) return false;
        if (a.gridX != b.gridX) return false;
        if (a.gridY != b.gridY) return false;
        if (a.requiredMovePower != b.requiredMovePower) return false;

        return true;
    }
    public static bool operator !=(JKH_Node a, JKH_Node b)
    {
        if (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null))
            return false;
        if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            return true;

        if (a.walkable != b.walkable) return true;
        if (a.worldPosition != b.worldPosition) return true;
        if (a.gridX != b.gridX) return true;
        if (a.gridY != b.gridY) return true;
        if (a.requiredMovePower != b.requiredMovePower) return true;

        return false;
    }
}

