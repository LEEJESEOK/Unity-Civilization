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
    public Facility facility;

    public District district;

    public bool canCreate;
    //이미지
    public int iconNum;

    private void Start()
    {
        district = District.NONE;
        //canCreate = true;
        terrainData = gameObject.GetComponent<TerrainData>();

        facility = Facility.NONE;
        district = District.NONE;
    }

    public void SetDistrict(District next)
    {
        district = next;
        WhatDistric();
    }
    public void WhatDistric()
    {
        switch (district)
        {
            case District.CAMPUS:
                iconNum = 0;
                terrainData.output.science = terrainData.myCenter.GetComponent<Territory>().population * 2;
                break;
            case District.COMMERCAILHUB:
                iconNum = 1;
                terrainData.output.gold = terrainData.myCenter.GetComponent<Territory>().population * 4;
                break;
            case District.INDUSTRIALZONE:
                iconNum = 2;
                terrainData.output.productivity = terrainData.myCenter.GetComponent<Territory>().population * 2;
                break;
            case District.NONE:
                break;
        }
    }

    public void SetFacility(Facility next)
    {
        facility = next;
        whatFacility();
    }


    public void whatFacility()
    {
        switch (facility)
        {
            case Facility.FARM:
                terrainData.output.food += 1;
                break;
            case Facility.MINE:
                terrainData.output.productivity += 1;
                break;
            case Facility.NONE:
                break;
        }
    }

}
