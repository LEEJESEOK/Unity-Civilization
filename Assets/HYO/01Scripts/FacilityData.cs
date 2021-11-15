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
    //�����ü�
    public Facility facility;
    public GameObject[] facilityOn;
    //���� Ư������
    public GameObject[] districtOn = new GameObject[3];
    public DistrictInPut districtInput;
    public District district;
    //�α�
    public int populatuon;
    //�̹���
    public int iconNum;

    private void Start()
    {
        for (int i = 0; i < districtOn.Length; i++)
        {
            districtOn[i].SetActive(false);
        }
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

    public void whatFacility()
    {
        switch (facility)
        {
            case Facility.FARM:
                iconNum = 3;
                break;
            case Facility.MINE:
                iconNum = 4;
                break;
            case Facility.NONE:
                break;
        }
    }


    public void Constr_Condition()
    {
        districtOn = new GameObject[(populatuon * 3) - 2];

        if (districtOn != null)
        {
            terrainData.output = new OutPut(0, 0, 0, 0);
        }
    }
}
