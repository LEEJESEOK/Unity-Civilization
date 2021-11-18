using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class TotalOutPut
{
    public int totalfood;
    public int totalProductivity;
    public int totalGold;
    public int totalScience;

    public TotalOutPut(int totalfood, int totalProductivity, int totalGold, int totalScience)
    {
        this.totalfood = totalfood;
        this.totalProductivity = totalProductivity;
        this.totalGold = totalGold;
        this.totalScience = totalScience;
    }
}

public class Territory : MonoBehaviour
{
    public Collider[] getTerritory;
    public TotalOutPut totalOutput;
    public int outputOrigin;
    public int outputTemp;
    void Start()
    {
        float maxDistance = 0f;
        int radius = 1;
        RaycastHit hit;

        // Physics.SphereCast (레이저를 발사할 위치, 구의 반경, 발사 방향, 충돌 결과, 최대 거리)
        //bool isHit = Physics.SphereCast(transform.position, 1, transform.up, out hit, maxDistance);
        getTerritory = Physics.OverlapSphere(transform.position, radius);
       
    }

    void Update()
    {
        outputOrigin = outputTemp;
        if (outputOrigin != outputTemp || outputOrigin == 0)
        {
            if (getTerritory != null)
            {  
                for (int i = 0; i < getTerritory.Length; i++)
                {
                    totalOutput.totalfood += getTerritory[i].gameObject.GetComponent<TerrainData>().output.food;
                    totalOutput.totalProductivity += getTerritory[i].gameObject.GetComponent<TerrainData>().output.productivity;
                    totalOutput.totalGold += getTerritory[i].gameObject.GetComponent<TerrainData>().output.gold;
                    totalOutput.totalScience += getTerritory[i].gameObject.GetComponent<TerrainData>().output.science;
                }
                outputTemp = totalOutput.totalfood + totalOutput.totalProductivity + totalOutput.totalGold + totalOutput.totalScience;
            }
        }
       
    }
}
