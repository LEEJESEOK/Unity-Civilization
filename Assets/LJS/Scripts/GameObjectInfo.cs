using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectInfo : MonoBehaviour
{
    public int playerId = -1;
    public ObjectType type;

    private void Start()
    {
        Unit unit = GetComponent<Unit>();
        if (unit != null)
            playerId = unit.playerId;
    }
}
