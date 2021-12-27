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
    public GameObject[] icons;
    public GameObject emptyPre;
    public GameObject cityGate;
    public GameObject fovPre;
    public Vector3[] iconPos;

    public Transform tileTemp;
    public GameObject centerCheck;

    //Unit Info
    public bool isUnitSelected = false;
    public GameObject unitInfo;
    public int unitLimit;
    Non_CombatUnitType unitType;

    //first city center
    public bool isFirst;
    public bool selectTile;
    public Territory cityTemp;

    TerrainData td;
    FacilityData fd;

    //지형 레이어
    int layerGrassLand;  // 6
    int layerPlains;     // 7
    int layerDesert;     // 8
    int layerMountain;   // 9
    int fogLayer;
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

        if (!UIManager.IsPointerOverUIObject() && Input.GetMouseButtonDown(0))
        {
            if (SelectUnit())
            {
                tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;
                fd = tileTemp.GetComponent<FacilityData>();

            }
            else
                SelectCity();
        }


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
            unitInfo.GetComponent<NonCombatUnit>().CheckMyPos();

            unitType = unitInfo.GetComponent<NonCombatUnit>().non_CombatUnitType;
            isUnitSelected = true;

            UIPanelManager.instance.OpenPanel("UNIT_PANEL");
            if (unitType == Non_CombatUnitType.Settler)
            {
                //settleBTN.SetActive(true);
            }
            else if (unitType == Non_CombatUnitType.Builder)
            {
                //건설 버튼
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
                cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/OutlineShader");
            }

            UIPanelManager.instance.OpenPanel("CITY_PRODUCT_PANEL");

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

                            for (int j = 0; j < cityTemp.data.Count; j++)
                            {
                                cityTemp.data[j].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
                            }

                            if (fd.district != District.NONE || td.myCenter.GetComponent<Territory>().distric_limit == false)
                            {
                                tileTemp = null;
                                print("!:특수지구 건설 불가");
                            }
                        }
                        //else
                        //{
                        //    tileTemp = null;
                        //    print("!:영토 아님");
                        //}
                    }

                    this.cityTemp = null;
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
        GameManager.instance.currentPlayer.info.cities.Add(tileTemp.transform.gameObject);


        worldOutPut.WorldFood += tt.totalOutput.Totalfood;
        worldOutPut.WorldProductivity += tt.totalOutput.TotalProductivity;
        worldOutPut.WorldGold += tt.totalOutput.TotalGold;
        worldOutPut.WorldScience += tt.totalOutput.TotalScience;

        tt.totalOutput.worldCallback = WorldCallback;


        GameObject city = Instantiate(cityGate);

        //SET COLOR
        MeshRenderer mesh = city.GetComponentInChildren<MeshRenderer>();
        Material mat = ColorManager.instance.buildingMat[GameManager.instance.currentPlayerId];
        mesh.material = mat;

        city.transform.parent = tileTemp;
        city.transform.position = tileTemp.position;
        city.transform.localPosition = new Vector3(0, 0.1f, 0);
        city.transform.localEulerAngles = new Vector3(-90, 0, 90);
        city.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        GameObject fov = Instantiate(fovPre);

        fov.transform.parent = city.transform;
        fov.transform.position = city.transform.position;

        tileTemp = null;

        //선택된 유닛 제거 후 초기화
        Destroy(unitInfo);
        isUnitSelected = false;
        unitInfo = null;
    }
    public void CreateFacility(Facility id)
    {
        if (fd.facility == Facility.NONE)
        {
            fd.SetFacility(id);

            tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;
            GameObject empty = Instantiate(icons[(int)id + 3]);


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

            unitInfo.GetComponent<NonCombatUnit>().buildCount += 1;
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

            tileTemp = null;
            isUnitSelected = false;
        }
        else return;

    }

    // TODO
    // parameter : 타일 x, y 좌표, 선택한 건물
    // 선택한 타일에 건물 모델 생성, 타일의 산출량 변경
    public void SetDistrictInfo(District id)
    {
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>();

        tt.districtUnderway.remain = TEST_REMAIN_PRODUCT;
        tt.districtUnderway.pos = tileTemp.transform;
        tt.districtUnderway.id = id;

        tileTemp = null;
    }
    public void CreateDistrict(District id, Transform pos)
    {
        GameObject empty = Instantiate(icons[(int)id]);

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

    }

}
