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

[Serializable]
public class DistrictInPut
{
    public int productivity;
    public int gold;
    public DistrictInPut(int productivity, int gold)
    {
        this.productivity = productivity;
        this.gold = gold;
    }
}

public class FacilityData : MonoBehaviour
{
    public TerrainData terrainData;
    //보유시설
    public Facility facility;
    //보유 특수지구
    public GameObject[] districtOn;
    public DistrictInPut districtInput;
    public District district;
    public bool canCreate;
    //인구
    public int populatuon;
    //이미지
    public int iconNum;

    private void Start()
    {
        canCreate = true;
        districtOn = new GameObject[3];
        terrainData = gameObject.GetComponent<TerrainData>();
        facility = Facility.NONE;
        district = District.NONE;

    }
    private void Update()
    {
        //Constr_Condition();
        if (transform.childCount >= 3)
        {
            canCreate = false;
        }
    }
    public void AddDistrict(GameObject add)
    {
        for (int i = 0; i < districtOn.Length; i++)
        {
            if (districtOn[i] == null)
            {
                districtOn[i] = add;
                break;
            }
        }
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
                districtInput = new DistrictInPut(54, 1);
                terrainData.output.science = populatuon * 2;
                break;
            case District.COMMERCAILHUB:
                iconNum = 1;
                districtInput = new DistrictInPut(54, 1);
                terrainData.output.gold = populatuon * 4;
                break;
            case District.INDUSTRIALZONE:
                iconNum = 2;
                districtInput = new DistrictInPut(54, 1);
                terrainData.output.productivity = populatuon * 2;
                break;
            case District.NONE:
                districtInput = new DistrictInPut(0, 0);
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


    public void Constr_Condition()
    {
        float pow = Mathf.Pow(populatuon - 1, 1.5f);
        terrainData.output.food = 8 * populatuon + 7 + (int)Mathf.Floor(pow);
        int resize = (populatuon * 3) - 2;
        Array.Resize(ref districtOn, resize);
        //districtOn = new GameObject[(populatuon * 3) - 2];

        //if (districtOn != null)
        //{
        //    terrainData.output = new OutPut(0, 0, 0, 0);
        //}
    }
}
