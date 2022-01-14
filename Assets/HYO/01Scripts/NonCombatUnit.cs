using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCombatUnit : Unit
{
    public int maxBuildCount = 3;
    public int buildCount;

    protected override void Start()
    {
        base.Start();

        GameObjectType gameObjectType = GetComponent<GameObjectType>();
        gameObjectType.type = ObjectType.NON_COMBAT_UNIT;

        buildCount = maxBuildCount;
    }

    protected override void Update()
    {
        base.Update();

        switch (unitType)
        {
            case InGameObjectId.SETTLER:
                break;
            case InGameObjectId.BUILDER:
                if (buildCount == 0)
                {
                    gameObject.GetComponent<Unit>().myTilePos.GetComponent<TerrainData>().objectOn.Remove(gameObject);
                    HexFogManager.instance.units[gameObject.GetComponent<Unit>().playerId].Remove(gameObject.GetComponent<Unit>());

                    Destroy(gameObject);
                }

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