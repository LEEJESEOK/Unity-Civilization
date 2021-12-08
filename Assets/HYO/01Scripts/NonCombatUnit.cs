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
    //public Facility facility;

    public int buildCount;

    //unit tilePos
    public GameObject myTilePos;
    public int posX, posY;



    public void NonCambatUnitCase()
    {
        switch (non_CombatUnitType)
        {
            case Non_CombatUnitType.Settler:             
                break;
            case Non_CombatUnitType.Builder:                
                break;
            default:
                break;
        }
    }

    private void Update()
    {
         if(non_CombatUnitType == Non_CombatUnitType.Builder)
        {
            if(buildCount == 3)
            {
                Destroy(gameObject);
            }
        }
    }
    //이동할때 불러
    public void CheckMyPos()
    {
        int fogLayer = LayerMask.GetMask("HexFog");

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up * -1);
        Debug.DrawRay(transform.position, transform.up * -1, Color.red);
        if (Physics.Raycast(ray, out hit, 1,~fogLayer))
        {
            myTilePos = hit.transform.gameObject;
            posX = myTilePos.GetComponent<TerrainData>().x;
            posY = myTilePos.GetComponent<TerrainData>().y;
        }
    }

}