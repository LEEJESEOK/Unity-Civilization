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

    //TileInfo UI
    public GameObject tileInfo;
    public Vector3 mousePos;
    public float popupTime = 3;
    public float currentTime;

    public Transform tileTemp;
    public GameObject centerCheck;

    //Unit Info
    public bool isUnitSelected = false;
    public GameObject unitInfo;
    public int unitLimit;
    Non_CombatUnitType unitType;

    //first city center
    public bool isFirst;
    bool isOpenPopup;
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
    int layerMask;

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
            else if (SelectCity())
            {
                if (cityTemp = null) return;

                if (SelectTile())
                {
                    td = tileTemp.GetComponent<TerrainData>();
                    fd = tileTemp.GetComponent<FacilityData>();

                    for (int i = 0; i < cityTemp.data.Count; i++)
                    {
                        cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Standard");
                    }

                    cityTemp = null;

                    if (tileTemp.GetComponent<TerrainData>().myCenter)
                    {
                        if (fd.district != District.NONE || td.myCenter.GetComponent<Territory>().distric_limit == false)
                        {
                            tileTemp = null;
                            print("!:특수지구 건설 불가");
                        }
                    }
                    else
                    {
                        tileTemp = null;
                        print("!:영토 아님");
                    }
                }               
            }
        }

        TileInfoPopUp();
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

    public bool SelectCity()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layer = layerCity & ~layerUI;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layer))
        {
            print(hit.transform.GetComponentInParent<Territory>());

            cityTemp = hit.transform.GetComponentInParent<Territory>();

            for(int i=0; i < cityTemp.data.Count; i++)
            {
                cityTemp.data[i].gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Custom/OutlineShader");
            }

            return true;
        }
        else
            return false;
    }
    public bool SelectTile()
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
                    }
                }
            }

            int layerNum = hit.transform.gameObject.layer;
            layerNum = LayerMask.GetMask(LayerMask.LayerToName(layerNum));

            if (layerNum == layerGrassLand || layerNum == layerPlains)
            {
                bool isHillis = hit.transform.gameObject.GetComponent<TerrainData>().isHills;
                if (isHillis)
                {
                    //버튼끄기
                }
                else
                {
                    //버튼끄기
                }
            }
            return true;
        }
        else
            return false;
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
        pos.GetComponent<FacilityData>().SetDistrict(id);
        pos.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().AddDistrict(id);
        empty.transform.parent = pos;
        empty.transform.position = pos.position;
        empty.transform.localPosition = new Vector3(0, 0.179f, 0);
        empty.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
 
    }

    public void TileInfoPopUp()
    {
        if (Camera.main == null)
        {
            return;
        }

        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        /* 
           1.1 만약 마우스가 움직이지 않는다면
              2. 만약 2초가 흘렀다면
              3. 마우스의 위치에 타일이 있는지 검사하고싶다.
              4. 만약 타일이 있다면 팝업을 띄우고싶다.
          1.2 그렇지않고 팝업을 보여주는 중이라면 팝업을 끄고싶다.
        */

        if (isOpenPopup == false)
            mousePos = Input.mousePosition;


        if (Physics.Raycast(rayPoint, out hitInfo, 1000, layerMask))
        {
            if (mousePos == Input.mousePosition)
            {
                currentTime += Time.deltaTime;
                if (currentTime > popupTime)
                {
                    isOpenPopup = true;

                    Transform tileTemp = hitInfo.transform;
                    if (tileTemp.GetComponent<TerrainData>() != null && !UIManager.IsPointerOverUIObject())
                    {
                        tileTemp.GetComponent<TerrainData>().ShowTileInfo();
                        tileInfo.transform.position = new Vector3(mousePos.x, mousePos.y);
                        tileInfo.SetActive(true);
                    }
                    currentTime = 0;

                }
            }
            else if (tileInfo.activeSelf == true)
            {
                isOpenPopup = false;

                tileInfo.SetActive(false);
            }

        }
    }
}
