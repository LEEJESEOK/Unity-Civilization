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
    Hills
}
//지형특성
public enum feature
{
    ICE, WOOD, MARSH, RAINFOREST, NONE
};


[Serializable]
public class OutPut
{
    //산출량
    public int food;
    public int productivity;
    public int gold;
    public int science;
    //이동력
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
    public LayerMask mask;

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
                break;
            case TerrainType.Plains:
                gameObject.layer = 7;
                break;
            case TerrainType.Desert:
                gameObject.layer = 8;
                break;
            case TerrainType.Mountain:
                gameObject.layer = 9;
                break;
            case TerrainType.Coast:
                gameObject.layer = 10;
                break;
            case TerrainType.Ocean:
                gameObject.layer = 11;
                break;
            case TerrainType.Hills:
                gameObject.layer = 12;
                break;
            default:
                break;
        }

    }

    
}
