using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

public enum OutPutType
{
    FOOD, PRODUCTIVITY, GOLD, SCIENCE
}

[Serializable]
public class OutPut
{
    //산출량

    int _food;
    int _productivity;
    int _gold;
    int _science;

    // delegate : 변수인데 함수를 담아놓고 사용
    //delegate void MyCallback(OutPutType otype, int amount);
    //MyCallback callback;
    public Action<OutPutType, int> callback;


    // Property : 함수(get,set)인데 변수처럼 사용
    public int food
    {
        get { return _food; }
        set
        {
            // Food의 값이 변화가 되는 순간
            if (callback != null) { callback(OutPutType.FOOD, value - _food); }
            _food = value;
        }
    }
    public int productivity
    {
        get { return _productivity; }
        set
        {
            if (callback != null) { callback(OutPutType.PRODUCTIVITY, value - _productivity); }
            _productivity = value;
        }
    }
    public int gold
    {
        get { return _gold; }
        set
        {
            if (callback != null) { callback(OutPutType.GOLD, value - _gold); }
            _gold = value;
        }
    }
    public int science
    {
        get { return _science; }
        set
        {
            if (callback != null) { callback(OutPutType.SCIENCE, value - _science); }
            _science = value;
        }
    }

    //이동력(0이면 이동불가)
    public int movePower;

    public OutPut(int food, int productivity, int gold, int science)
    {
        this.food = food;
        this.productivity = productivity;
        this.gold = gold;
        this.science = science;
    }
}

public class TerrainData : MonoBehaviour
{
    public TerrainType terrainType;
    public Feature feature;
    public FacilityData facilityData;
    public OutPut output;
    public LayerMask mask;
    public GameObject[] territory = new GameObject[7];
    public GameObject myCenter;

    public List<GameObject> objectOn = new List<GameObject>();
    // 언덕유무
    public bool isHills;

    //유닛 확인
    public bool isUnitOn;

    //map index
    public int width = 50;
    public int length = 50;

    public int x, y;

    public Material[] tileMats;

    private void Awake()
    {
        InitTerrainType();
        InitTerrainFeature();
    }

    private void Start()
    {
        facilityData = gameObject.GetComponent<FacilityData>();
    }

    public void SetIndex(int x, int y)
    {
        this.x = x;
        this.y = y;

    }

    //layer(6~12)
    public void InitTerrainType()
    {
        switch (terrainType)
        {
            case TerrainType.GrassLand:
                gameObject.layer = 6;
                output = new OutPut(2, 0, 0, 0);
                output.movePower = 1;
                CheckHills();
                break;
            case TerrainType.Plains:
                gameObject.layer = 7;
                output = new OutPut(1, 1, 0, 0);
                output.movePower = 1;
                CheckHills();
                break;
            case TerrainType.Desert:
                gameObject.layer = 8;
                output = new OutPut(0, 0, 0, 0);
                output.movePower = 1;
                CheckHills();
                break;
            case TerrainType.Mountain:
                gameObject.layer = 9;
                output = new OutPut(0, 0, 0, 0);
                output.movePower = 0;
                CheckHills();
                break;
            case TerrainType.Coast:
                gameObject.layer = 10;
                output = new OutPut(1, 0, 1, 0);
                output.movePower = 1;
                CheckHills();
                break;
            case TerrainType.Ocean:
                gameObject.layer = 11;
                output = new OutPut(1, 0, 0, 0);
                output.movePower = 0;
                CheckHills();
                break;
            default:
                break;
        }

    }

    public void InitTerrainFeature()
    {
        switch (feature)
        {
            case Feature.ICE:
                output.movePower += 0;
                break;
            case Feature.WOOD:
                output.productivity += 1;
                output.movePower += 1;
                break;
            case Feature.MARSH:
                isHills = false;
                output.food += 1;
                output.movePower += 1;
                break;
            case Feature.RAINFOREST:
                output.food += 1;
                output.movePower += 1;
                break;
            case Feature.NONE:
                break;
            default:
                break;
        }
    }

    public void CheckHills()
    {
        if (isHills)
        {
            output.productivity += 1;
            output.movePower = 2;
            //전투력 +3
        }
    }

    public void SetTileInfo()
    {
        UIManager.instance.GetTileInfo(terrainType, myCenter, output.movePower, output.food, output.productivity);
    }


    //한 번만 하게
    bool one;
    //타일에 위치한 오브젝트 저장
    public void AddObjectOn(GameObject obj, int playerId)
    {
        objectOn.Add(obj);

        if (one == false && playerId != GameManager.instance.currentPlayerId)
        {
            obj.GetComponent<Unit>().body.SetActive(false);
            one = true;
        }
    }
}
