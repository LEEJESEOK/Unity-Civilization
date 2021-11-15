using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//�Ǽ�
public enum Facility
{
    FARM, MINE, NONE
}

//Ư������
public enum District
{
    CAMPUS, COMMERCAILHUB, INDUSTRIALZONE , NONE
}

[Serializable]
public class DistrictInPut
{
    public int productivity;
    public int gold;
    public DistrictInPut(int productivity,int gold)
    {
        this.productivity = productivity;
        this.gold = gold;
    }
}

    public class FacilityData : MonoBehaviour
{
    public TerrainData terrainData;
    //�����ǹ�
    public string[] facilityOn;
    //���� Ư������
    public string[] districtOn;
    public DistrictInPut districtInput;
    public District district;
    //�α�
    public int populatuon;

    public void WhatDistric()
    {
        switch (district)
        {
            case District.CAMPUS:
                districtInput = new DistrictInPut(54, 1);
                terrainData.output.science = populatuon * 2;
                break;
            case District.COMMERCAILHUB:
                districtInput = new DistrictInPut(54, 1);
                terrainData.output.gold = populatuon * 4;
                break;
            case District.INDUSTRIALZONE:
                districtInput = new DistrictInPut(54, 1);
                terrainData.output.productivity = populatuon * 2;
                break;
            case District.NONE:
                districtInput = new DistrictInPut(0,0);
                break;
            default:
                break;
        }
    }

    public void Constr_Condition()
    {
        districtOn = new string[(populatuon * 3) - 2];

        if(districtOn != null)
        {
            terrainData.output = new OutPut(0, 0, 0, 0);
        }
    }
}
