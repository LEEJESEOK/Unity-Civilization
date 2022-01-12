using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCombatUnit : Unit
{
    public int buildCount;

    protected override void Start()
    {
        base.Start();
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