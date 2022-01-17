using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : Unit
{
    public int meleeAttack;
    public int rangeAttack;
    public int range;
    public int combatCode;

    protected override void Start()
    {
        base.Start();
        
        GameObjectInfo gameObjectType = GetComponent<GameObjectInfo>();
        gameObjectType.type = ObjectType.COMBAT_UNIT;
    }
}
