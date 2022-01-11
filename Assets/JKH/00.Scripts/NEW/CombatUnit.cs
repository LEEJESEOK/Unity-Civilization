using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : Unit
{
    public int meleeAttack;
    public int rangeAttack;
    public int range;

    protected override void Start()
    {
        base.Start();
        
        GameObjectType gameObjectType = GetComponent<GameObjectType>();
        gameObjectType.type = ObjectType.COMBAT_UNIT;
    }
}
