using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class DistrictUnderway
{
    public InGameObjectId id;
    TypeIdBase objectType;
    public Transform pos;
    public int remain;

    public DistrictUnderway(InGameObjectId id, TypeIdBase objectType, Transform pos, int remain)
    {
        this.id = id;
        this.objectType = objectType;
        this.pos = pos;
        this.remain = remain;
    }

    public DistrictUnderway(ProductObject productObject, Transform pos)
    {
        this.id = productObject.id;
        this.objectType = productObject.type;
        this.remain = productObject.remainCost;
        this.pos = pos;
    }
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
    public string territoryName;
    public TotalOutPut totalOutput;

    public List<TerrainData> data;

    public GameObject cityCenter;

    public int population = 1;

    //소유자(player id)
    public int tt_playerId;

    //보유 특수지구
    public List<InGameObjectId> districtOn = new List<InGameObjectId>();
    public DistrictUnderway districtUnderway;
    public DistrictInPut districtInput = new DistrictInPut(54, 1);
    public bool distric_limit = true;

    //특수지구 건설
    int maintenanceCost;
    int carryRemain;

    private void Awake()
    {
        totalOutput = new TotalOutPut();
        districtUnderway = new DistrictUnderway(InGameObjectId.NONE, TypeIdBase.NONE, null, -1);

        tt_playerId = GameManager.instance.currentPlayerId;
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
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, ~layerMask);

        for (int i = 0; i < cols.Length; i++)
        {
            TerrainData td = cols[i].GetComponent<TerrainData>();

            if (td != null)
            {
                #region test
                td.gameObject.name = "" + td.x + ", " + td.y;
                td.transform.SetAsFirstSibling();
                #endregion

                data.Add(td);
                totalOutput.Totalfood += td.output.food;
                totalOutput.TotalProductivity += td.output.productivity;
                totalOutput.TotalGold += td.output.gold;
                totalOutput.TotalScience += td.output.science;

                td.output.callback = MyCallback;
                td.myCenter = cityCenter;
            }
        }

        for(int j = 0; j < data.Count; j++)
        {
            Material material = data[j].gameObject.GetComponent<MeshRenderer>().material;
            material.shader = Shader.Find("Custom/OutlineShader");
            material.SetColor("_OutlineColor", ColorManager.instance.playerColor[tt_playerId]);
            //material.color = ColorManager.instance.playerColor[tt_playerId];
            data[j].gameObject.GetComponent<MeshRenderer>().material = material;
        }
    }

    public void AddDistrict(InGameObjectId add)
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
        if (carryRemain > 0)
        {
            districtUnderway.remain -= carryRemain;
            carryRemain = 0;
        }

        //건설 완료
        else if (districtUnderway.remain <= 0)
        {
            HYO_ConstructManager.instance.CreateDistrict(districtUnderway.id, districtUnderway.pos);
            carryRemain = -districtUnderway.remain;
        }

        //특수지구 유지비
        if (districtOn == null) return;
        for (int i = 0; i < districtOn.Count; i++)
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

    // Food = 15 + 8 * (n − 1) + (n − 1) ^1.5;
    public void RequestedFood()
    {
        //인구 증가 요구 식량
        float pow = Mathf.Pow(population - 1, 1.5f);

        if (totalOutput.Totalfood == 8 * population + 7 + (int)Mathf.Floor(pow))
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
    private void LateUpdate()
    {
        if (MapManager.instance.unitSelecting == false)
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].gameObject.GetComponent<MeshRenderer>().material.shader != Shader.Find("Custom/OutlineShader"))
                {
                    Material material = data[i].gameObject.GetComponent<MeshRenderer>().material;
                    material.shader = Shader.Find("Custom/OutlineShader");
                    material.SetColor("_OutlineColor", ColorManager.instance.playerColor[tt_playerId]);
                    data[i].gameObject.GetComponent<MeshRenderer>().material = material;
                }
            }
        }
    }
}
