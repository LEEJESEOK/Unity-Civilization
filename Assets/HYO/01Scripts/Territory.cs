using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class DistrictUnderway
{
    public District id;
    public Transform pos;
    public int remain;
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
public enum TotalOutPutType
{
    TOTALFOOD, TOTALPRODUCTIVITY, TOTALGOLD, TOTALSCIENCE
}

[Serializable]
public class TotalOutPut
{
    public int _totalfood;
    public int _totalProductivity;
    public int _totalGold;
    public int _totalScience;

    public Action<TotalOutPutType, int> worldCallback;

    public int Totalfood
    {
        get { return _totalfood; }
        set
        {
            if (worldCallback != null) { worldCallback(TotalOutPutType.TOTALFOOD, value - _totalfood); }
            _totalfood = value;
        }
    }
    public int TotalProductivity
    {
        get { return _totalProductivity; }
        set
        {
            if (worldCallback != null) { worldCallback(TotalOutPutType.TOTALPRODUCTIVITY, value - _totalProductivity); }
            _totalProductivity = value;
        }
    }
    public int TotalGold
    {
        get { return _totalGold; }
        set
        {
            if (worldCallback != null) { worldCallback(TotalOutPutType.TOTALGOLD, value - _totalGold); }
            _totalGold = value;
        }
    }
    public int TotalScience
    {
        get { return _totalScience; }
        set
        {
            if (worldCallback != null) { worldCallback(TotalOutPutType.TOTALSCIENCE, value - _totalScience); }
            _totalScience = value;
        }
    }

}

public class Territory : MonoBehaviour
{
    public TotalOutPut totalOutput;

    public List<TerrainData> data;

    public GameObject cityCenter;

    public int population =1;


    //보유 특수지구
    public List<District> districtOn = new List<District>();
    public DistrictUnderway districtUnderway = new DistrictUnderway();
    public DistrictInPut districtInput = new DistrictInPut(54, 1);
    public bool distric_limit = true;

    //특수지구 건설
    int maintenanceCost;
    int carryRemain;

    private void Awake()
    {
        totalOutput = new TotalOutPut();
        districtUnderway.id = District.NONE;
    }

    void Start()
    {
        //first city
        if (HYO_ConstructManager.instance.isFirst)
        {
            gameObject.tag = "FirstCity";
            HYO_ConstructManager.instance.isFirst = false;
        }
        cityCenter = gameObject;
        
        data = new List<TerrainData>();

       
        int radius = 1;
        int layerMask = 1 << LayerMask.NameToLayer("HexFog");

        Collider[] cols = Physics.OverlapSphere(transform.position, radius,~layerMask);
        
        for (int i = 0; i < cols.Length; i++)
        {
            TerrainData td = cols[i].GetComponent<TerrainData>();
            if (td != null)
            {
                data.Add(td);
                totalOutput.Totalfood += td.output.food;
                totalOutput.TotalProductivity += td.output.productivity;
                totalOutput.TotalGold += td.output.gold;
                totalOutput.TotalScience += td.output.science;

                td.output.callback = MyCallback;
                td.myCenter = cityCenter;
            }
        }
    }

    public void AddDistrict(District add)
    {
        districtOn.Add(add);
        if (districtOn != null)
        {
            if (districtOn.Count > (population * 3) - 2) distric_limit = false;
        }
            
    }
    public void DistrictProcess()
    {
        districtUnderway.remain -= totalOutput._totalProductivity;
        if(carryRemain > 0)
        {
            districtUnderway.remain -= carryRemain;
            carryRemain = 0;
        }

        //건설 완료
        else if(districtUnderway.remain <= 0)
        {
            HYO_ConstructManager.instance.CreateDistrict(districtUnderway.id, districtUnderway.pos);
            carryRemain = -districtUnderway.remain;
        }

        //특수지구 유지비
        if (districtOn == null) return;
        for(int i =0; i < districtOn.Count; i++)
        {
            maintenanceCost += districtInput.gold;
        }
        totalOutput.TotalGold -= maintenanceCost;

    }

    void MyCallback(OutPutType otype, int amount)
    {
        switch (otype)
        {
            case OutPutType.FOOD: totalOutput.Totalfood += amount; break;
            case OutPutType.PRODUCTIVITY: totalOutput.TotalProductivity += amount; break;
            case OutPutType.GOLD: totalOutput.TotalGold += amount; break;
            case OutPutType.SCIENCE: totalOutput.TotalScience += amount; break;
        }
    }

    //Food=15+8*(n−1)+(n−1)^1.5
    public void RequestedFood()
    {
        //인구 증가 요구 식량
        float pow = Mathf.Pow(population - 1, 1.5f);

        if(totalOutput.Totalfood == 8 * population + 7 + (int)Mathf.Floor(pow))
        {
            population += 1;
        }
    }
    //public void Constr_Condition()
    //{
    //    int resize = (population * 3) - 2;
    //    Array.Resize(ref districtOn, resize);
    //}

    void Update()
    {
        RequestedFood();
    }
}
