using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCombatUnit : Unit
{
    public int buildCount;

    protected override void Start()
    {
        base.Start();
        
        GameObjectType gameObjectType = GetComponent<GameObjectType>();
        gameObjectType.type = ObjectType.NON_COMBAT_UNIT;
    }

    protected override void Update()
    {
        base.Update();

        switch (unitType)
        {
            case InGameObjectId.SETTLER:
                break;
            case InGameObjectId.BUILDER:
                if (buildCount == 3)
                    Destroy(gameObject);
                break;
        }
    }

    public void NonCambatUnitCase()
    {
        switch (unitType)
        {
            case InGameObjectId.SETTLER:
                break;
            case InGameObjectId.BUILDER:
                break;
            default:
                break;
        }
    }
}