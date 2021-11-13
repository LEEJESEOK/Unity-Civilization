using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

//지형
public enum TerrainType
{
    GrassLand,
    Plains,
    Desert,
    Mountain,
    Coast,
    Ocean,
}
//지형특성
public enum Feature
{
    ICE, WOOD, MARSH, RAINFOREST, NONE
}

//건설
public enum Facility
{
    FARM,MINE,NONE
}


[Serializable]
public class OutPut
{
    //산출량
    public int food;
    public int productivity;
    public int gold;
    public int science;
    //이동력(0이면 이동불가)
    public int movePower;

    public OutPut(int food, int productivity, int gold, int science, int movePower)
    {
        this.food = food;
        this.productivity = productivity;
        this.gold = gold;
        this.science = science;
        this.movePower = movePower;
    }
}

public class TerrainData : MonoBehaviour
{
    public TerrainType terrainType;
    public OutPut output;
    public Facility facility;
    public LayerMask mask;
    // 언덕유무
    public bool isHills;
    

    //map index
    public int width = 50;
    public int length = 50;

    public int x, y;

    public void SetIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    //layer(6~12)
    public void CheckTerrainType()
    {
        switch (terrainType)
        {
            case TerrainType.GrassLand:
                gameObject.layer = 6;
                output = new OutPut(2, 0, 0, 0, 1);
                CheckHillsNFacility();
                break;
            case TerrainType.Plains:
                gameObject.layer = 7;
                output = new OutPut(1, 1, 0, 0, 1);
                CheckHillsNFacility();
                break;
            case TerrainType.Desert:
                gameObject.layer = 8;
                output = new OutPut(0, 0, 0, 0, 1);
                CheckHillsNFacility();
                break;
            case TerrainType.Mountain:
                gameObject.layer = 9;
                output = new OutPut(0, 0, 0, 0, 0);
                CheckHillsNFacility();
                break;
            case TerrainType.Coast:
                gameObject.layer = 10;
                output = new OutPut(1, 0, 1, 0, 1);
                CheckHillsNFacility();
                break;
            case TerrainType.Ocean:
                gameObject.layer = 11;
                output = new OutPut(1, 0, 0, 0, 1);
                CheckHillsNFacility();
                break;
            default:
                break;
        }

    }

    public void CheckHillsNFacility()
    {
        if (isHills)
        {
            output.productivity += 1;
            output.movePower = 2;
            //전투력 +3
        }
    }

    private void Update()
    {
        CheckTerrainType();
    }
}
