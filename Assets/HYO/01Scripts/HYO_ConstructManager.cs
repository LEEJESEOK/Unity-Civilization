using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class WorldOutPut
{
    public int WorldFood;
    public int WorldProductivity;
    public int WorldGold;
    public int WorldScience;
}


public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
    // test
    public int TEST_REMAIN_PRODUCT = 54;

    public WorldOutPut worldOutPut;

    //prefab & icon
    public GameObject[] disctirctIcons;
    public GameObject[] facilityIcons;
    //public GameObject emptyPre;
    public GameObject cityGate;
    public Vector3[] iconPos;

    public Material matRoad;

    //test
    public GameObject roadBTN_test;

    public Transform tileTemp;
    public GameObject centerCheck;

    //Unit Info
    public bool isUnitSelected = false;
    public GameObject unitInfo;
    public int unitLimit;
    InGameObjectId unitType;

    //first city center
    public bool isFirst;
    public bool selectTile;
    public Territory cityTemp;

    TerrainData td;
    FacilityData fd;

    //road
    //지형 레이어
    int layerGrassLand;  // 6
    int layerPlains;     // 7
    int layerDesert;     // 8
    int layerMountain;   // 9
    public int fogLayer;
    //유닛 레이어
    int layerUnit;
    //UI 레이어
    int layerUI;

    int layerCity;
    public int layerMask;


    void Start()
    {
        isFirst = true;
        selectTile = false;

        layerUI = LayerMask.GetMask("UI");
        layerGrassLand = LayerMask.GetMask("GrassLand");
        layerPlains = LayerMask.GetMask("Plains");
        layerDesert = LayerMask.GetMask("Desert");
        layerMountain = LayerMask.GetMask("Mountain");
        fogLayer = LayerMask.GetMask("HexFog");
        layerUnit = LayerMask.GetMask("Unit");
        layerCity = LayerMask.GetMask("City");

        layerMask = (layerGrassLand | layerPlains | layerDesert | layerMountain) & ~fogLayer & ~layerUnit & ~layerUI;

    }

    void WorldCallback(TotalOutPutType toType, int totalAmount)
    {
        switch (toType)
        {
            case TotalOutPutType.TOTALFOOD: worldOutPut.WorldFood += totalAmount; break;
            case TotalOutPutType.TOTALPRODUCTIVITY: worldOutPut.WorldProductivity += totalAmount; break;
            case TotalOutPutType.TOTALGOLD: worldOutPut.WorldGold += totalAmount; break;
            case TotalOutPutType.TOTALSCIENCE: worldOutPut.WorldScience += totalAmount; break;
        }
    }

    private void Update()
    {
        if (GameManager.instance.IsCurrentUnit())
        {
            tileTemp = GameManager.instance.GetCurrentUnit().myTilePos.transform;
            fd = tileTemp.GetComponent<FacilityData>();
        }
        if(GameManager.instance.IsCurrentCity())
        {
            
        }

        // if (!UIManager.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
        // {
        //     if (SelectUnit())
        //     {
        //         tileTemp = unitInfo.GetComponent<Unit>().myTilePos.transform;

        //     }
        //     else
        //         SelectCity();
        // }


        if (cityTemp != null)
        {
            SelectTile(cityTemp);
        }

    }

    //select
    public bool SelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerUnit))
        {
            unitInfo = hit.transform.gameObject;

            //이동할때로 옮겨 나중에
            unitInfo.GetComponent<Unit>().CheckMyPos();

            unitType = unitInfo.GetComponent<Unit>().unitType;
            isUnitSelected = true;

            switch (unitType)
            {
                case InGameObjectId.SETTLER:
                    break;
                case InGameObjectId.BUILDER:
                    break;
            }
            return true;
        }
        else
            return false;

    }
    public void SelectCity()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layer = layerCity & ~layerUI;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layer))
        {
            print(hit.transform.GetComponentInParent<Territory>());

            cityTemp = hit.transform.GetComponentInParent<Territory>();

            for (int i = 0; i < cityTemp.data.Count; i++)
            {
                // 영역 내 외곽선 그리기
                // cityTemp.data[i].gameObject.GetComponent<Outline>().OutlineWidth = 10;
                // cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/OutlineShader");

                Material material = cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material;
                //material.shader = Shader.Find("Custom/OutlineShader");
                material.SetColor("_OutlineColor", Color.white);
                cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material = material;
            }

            // 선택한 도시로 UI 갱신
            // UIPanelManager.instance.OpenPanel("CITY_PANEL");
            UIManager.instance.UpdateCityPanelData(cityTemp);
            // UIPanelManager.instance.OpenPanel("CITY_PRODUCT_PANEL");

            SelectTile(cityTemp);
        }

    }

    public void SelectTile(Territory cityTemp)
    {
        if (!UIManager.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
            {
                if (hit.transform.GetComponent<Territory>() == null)
                {
                    for (int i = 0; i < cityTemp.data.Count; i++)
                    {
                        if (cityTemp.data[i].gameObject == hit.transform.gameObject)
                        {
                            tileTemp = hit.transform;

                            fd = tileTemp.GetComponent<FacilityData>();
                            td = tileTemp.GetComponent<TerrainData>();

                            if (fd.district != InGameObjectId.NONE || td.myCenter.GetComponent<Territory>().distric_limit == false)
                            {
                                tileTemp = null;
                                print("!:특수지구 건설 불가");
                            }
                        }
                    }

                    for (int i = 0; i < cityTemp.data.Count; i++)
                    {
                        // 그린 외곽선 해제
                        // cityTemp.data[i].gameObject.GetComponent<Outline>().OutlineWidth = 0;
                        Material material = cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material;
                        //material.shader = Shader.Find("Standard");
                        material.SetColor("_OutlineColor", ColorManager.instance.playerColor[cityTemp.tt_playerId]);
                        cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material = material;
                    }

                    this.cityTemp = null;
                }

            }
            else if (Input.GetMouseButtonDown(0))
            {
                for (int i = 0; i < cityTemp.data.Count; i++)
                {
                    // 그린 외곽선 해제
                    // cityTemp.data[i].gameObject.GetComponent<Outline>().OutlineWidth = 0;
                    Material material = cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material;
                    //material.shader = Shader.Find("Standard");
                    material.SetColor("_OutlineColor", ColorManager.instance.playerColor[cityTemp.tt_playerId]);
                    cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material = material;
                }
            }
        }
    }

    //create
    public void CreateTerritory()
    {

        Collider[] centers = Physics.OverlapSphere(tileTemp.position, 1, ~fogLayer & ~layerUnit);

        for (int i = 0; i < centers.Length; i++)
        {

            if (centers[i].GetComponent<TerrainData>().myCenter != null)
            {
                print("도시건설 불가:도시 인접지역");
                return;
            }
        }
        Territory tt = tileTemp.gameObject.AddComponent<Territory>();

        //전체 도시 리스트에 저장
        GameManager.instance.currentPlayer.BuildCity(tt);


        worldOutPut.WorldFood += tt.totalOutput.Totalfood;
        worldOutPut.WorldProductivity += tt.totalOutput.TotalProductivity;
        worldOutPut.WorldGold += tt.totalOutput.TotalGold;
        worldOutPut.WorldScience += tt.totalOutput.TotalScience;

        tt.totalOutput.worldCallback = WorldCallback;


        GameObject city = Instantiate(cityGate);

        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_BUILD);

        //SET COLOR
        MeshRenderer mesh = city.GetComponentInChildren<MeshRenderer>();
        Material mat = ColorManager.instance.buildingMat[GameManager.instance.currentPlayerId];
        mesh.material = mat;

        city.transform.parent = tileTemp;
        city.transform.position = tileTemp.position;
        city.transform.localPosition = new Vector3(0, 0.1f, 0);
        city.transform.localEulerAngles = new Vector3(-90, 0, 90);
        city.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);


        HexFogManager.instance.buildings[HexFogManager.instance.currentPlayerId].Add(city);

        tileTemp.GetComponent<TerrainData>().AddObjectOn(city, GameManager.instance.currentPlayerId);

        tileTemp.GetComponent<TerrainData>().objectOn.Remove(unitInfo);
        HexFogManager.instance.units[unitInfo.GetComponent<Unit>().playerId].Remove(unitInfo.GetComponent<Unit>());
        tileTemp = null;

        //선택된 유닛 제거 후 초기화
        Destroy(unitInfo);
        isUnitSelected = false;
        unitInfo = null;
    }

    public void CreateFacility(InGameObjectId id)
    {
        if (fd.facility == InGameObjectId.NONE)
        {
            fd.SetFacility(id);

            NonCombatUnit unit = GameManager.instance.currentSelect.GetComponent<NonCombatUnit>();
            tileTemp = unit.myTilePos.transform;
            GameObject empty = Instantiate(facilityIcons[(int)(id - InGameObjectId.FARM)]);

            SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_BUILD);

            //SET COLOR
            MeshRenderer mesh = empty.GetComponentInChildren<MeshRenderer>();
            Material mat = ColorManager.instance.buildingMat[GameManager.instance.currentPlayerId];
            mesh.material = mat;

            List<SkinnedMeshRenderer> skinnedMesh = new List<SkinnedMeshRenderer>(empty.GetComponentsInChildren<SkinnedMeshRenderer>());
            for (int i = 0; i < skinnedMesh.Count; ++i)
            {
                Material skinnedmMat = ColorManager.instance.buildingMat[GameManager.instance.currentPlayerId];
                skinnedMesh[i].material = skinnedmMat;
            }

            unitInfo.GetComponent<NonCombatUnit>().buildCount -= 1;
            Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
            //FacilityData fd = tileTemp.GetComponent<FacilityData>();

            if ((int)id + 3 == -1)
            {
                return;
            }
            empty.transform.parent = tileTemp;
            empty.transform.position = tileTemp.position;
            empty.transform.localPosition = new Vector3(0, 0.109f, 0);
            //empty.transform.localEulerAngles = new Vector3(0, -90, 0);
            empty.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            HexFogManager.instance.buildings[HexFogManager.instance.currentPlayerId].Add(empty);
            tileTemp.GetComponent<TerrainData>().AddObjectOn(empty, GameManager.instance.currentPlayerId);
            tileTemp = null;
            isUnitSelected = false;
        }
        else return;

    }

    //test
    public void OnClickCreateRoad()
    {
        tileTemp.gameObject.GetComponent<MeshRenderer>().material = matRoad;

        TerrainData td = tileTemp.gameObject.GetComponent<TerrainData>();
        td.output.movePower = 1;

        roadBTN_test.SetActive(false);
    }

    // TODO
    // parameter : 타일 x, y 좌표, 선택한 건물
    // 선택한 타일에 건물 모델 생성, 타일의 산출량 변경
    public void SetDistrictInfo(InGameObjectId objectId)
    {
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>();

        ProductObject productObject = ProductObjectDataManager.instance.productObjects.Find(x => x.id == objectId);

        tt.districtUnderway = new DistrictUnderway(productObject, tileTemp.transform);

        // tt.districtUnderway.id = productObject.id;
        // tt.districtUnderway.remain = productObject.remainCost;
        // tt.districtUnderway.pos = tileTemp.transform;;

        tileTemp = null;
    }

    public void CreateDistrict(InGameObjectId id, Transform pos)
    {
        GameObject empty = Instantiate(disctirctIcons[(int)(id - InGameObjectId.CAMPUS)]);

        SoundManager.instance.PlayEFT(SoundManager.EFT_TYPE.EFT_BUILD);

        //SET COLOR
        MeshRenderer mesh = empty.GetComponentInChildren<MeshRenderer>();
        Material mat = ColorManager.instance.buildingMat[GameManager.instance.currentPlayerId];
        mesh.material = mat;


        pos.GetComponent<FacilityData>().SetDistrict(id);
        pos.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().AddDistrict(id);
        empty.transform.parent = pos;
        empty.transform.position = pos.position;
        empty.transform.localPosition = new Vector3(0, 0.179f, 0);
        empty.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);

        HexFogManager.instance.buildings[HexFogManager.instance.currentPlayerId].Add(empty);

        //특수지구에서 행동력 무조건 1
        pos.gameObject.GetComponent<TerrainData>().output.movePower = 1;
        //object on 에 건물 추가
        pos.gameObject.GetComponent<TerrainData>().AddObjectOn(empty, GameManager.instance.currentPlayerId);

    }
}
