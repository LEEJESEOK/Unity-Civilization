using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


//시설
public enum Facility
{
    FARM, MINE, NONE
}

//특수지구
public enum District
{
    CAMPUS, COMMERCAILHUB, INDUSTRIALZONE, NONE
}

public class FacilityData : MonoBehaviour
{
    public TerrainData terrainData;
    //보유시설
    public InGameObjectId facility;

    public InGameObjectId district;

    public bool canCreate;
    //이미지
    public int iconNum;

    private void Start()
    {
        district = InGameObjectId.NONE;
        //canCreate = true;
        terrainData = gameObject.GetComponent<TerrainData>();

        facility = InGameObjectId.NONE;
        district = InGameObjectId.NONE;
    }

    public void SetDistrict(InGameObjectId next)
    {
        district = next;
        WhatDistric();
    }
    public void WhatDistric()
    {
        switch (district)
        {
            case InGameObjectId.CAMPUS:
                iconNum = 0;
                terrainData.output.science = terrainData.myCenter.GetComponent<Territory>().population * 2;
                break;
            case InGameObjectId.COMMERCIAL_HUB:
                iconNum = 1;
                terrainData.output.gold = terrainData.myCenter.GetComponent<Territory>().population * 4;
                break;
            case InGameObjectId.INDUSTRIAL_ZONE:
                iconNum = 2;
                terrainData.output.productivity = terrainData.myCenter.GetComponent<Territory>().population * 2;
                break;
            case InGameObjectId.NONE:
                break;
        }
    }

    public void SetFacility(InGameObjectId next)
    {
        facility = next;
        whatFacility();
    }


    public void whatFacility()
    {
        switch (facility)
        {
            case InGameObjectId.FARM:
                terrainData.output.food += 1;
                break;
            case InGameObjectId.MINE:
                terrainData.output.productivity += 1;
                break;
            case InGameObjectId.NONE:
                break;
        }
    }

}
