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
}

public class Territory : MonoBehaviour
{
    public List<TerrainData> data;
    public TotalOutPut totalOutput;
    public int population =1;

    void Start()
    {
        totalOutput = new TotalOutPut();
        data = new List<TerrainData>();
       
        int radius = 1;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        
        for (int i = 0; i < cols.Length; i++)
        {
            TerrainData td = cols[i].GetComponent<TerrainData>();
            if (td != null)
            {
                data.Add(td);
                totalOutput.totalfood += td.output.food;
                totalOutput.totalProductivity += td.output.productivity;
                totalOutput.totalGold += td.output.gold;
                totalOutput.totalScience += td.output.science;

                td.output.callback = MyCallback;
            }
        }
    }

    void MyCallback(OutPutType otype, int amount)
    {
        switch (otype)
        {
            case OutPutType.FOOD: totalOutput.totalfood += amount; break;
            case OutPutType.PRODUCTIVITY: totalOutput.totalProductivity += amount; break;
            case OutPutType.GOLD: totalOutput.totalGold += amount; break;
            case OutPutType.SCIENCE: totalOutput.totalScience += amount; break;
        }
    }

    public void RequestedFood()
    {
        //인구 증가 요구 식량
        float pow = Mathf.Pow(population - 1, 1.5f);

        if(totalOutput.totalfood == 8 * population + 7 + (int)Mathf.Floor(pow))
        {
            population += 1;
        }
    }
    void Update()
    {
        RequestedFood();
    }
}
