using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


//�ü�
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
    //�����ü�
    public Facility facility;
    public string[] facilityOn;
    //���� Ư������
    public string[] districtOn;
    public DistrictInPut districtInput;
    public District district;
    //�α�
    public int populatuon;
    //�̹���
    public Sprite[] icons;
    public GameObject iconPos;
    public int iconNum;

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
                districtInput = new DistrictInPut(0,0);
                break;
            default:
                break;
        }
    }

    public void Constr_Btn()
    {
        GameObject empty = Instantiate(iconPos, this.transform.position, Quaternion.identity);
        empty.transform.parent = this.transform;
        empty.GetComponent<SpriteRenderer>().sprite = icons[iconNum];
        
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
