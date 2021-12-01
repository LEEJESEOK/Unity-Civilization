using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Non_CombatUnitType
{
    Settler=TypeIdBase.UNIT,
    Builder
}
public class NonCombatUnit : MonoBehaviour
{
    public Non_CombatUnitType non_CombatUnitType;
    public Facility facility;
    public int settleCount;
    public int buildCount;

    public void NonCambatUnitCase()
    {
        switch (non_CombatUnitType)
        {
            case Non_CombatUnitType.Settler:
                if(settleCount == 1)
                {
                    Destroy(gameObject);
                }
                break;
            case Non_CombatUnitType.Builder:
                if(buildCount == 3)
                {
                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }
    }

}