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

public class DistrictInfo
{
    public District id;
    public Transform pos;
    public DistrictInPut input;
}


public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
    //전체 도시
    public List<GameObject> worldCity = new List<GameObject>();

    public WorldOutPut worldOutPut;

    //prefab & icon
    public GameObject[] icons;
    public GameObject emptyPre;
    public GameObject cityGate;
    public GameObject fovPre;
    public Vector3[] iconPos;
    public Transform evironment;

    public Facility facility;

    //button UI
    public GameObject settleBTN;

    public GameObject farmBTN;
    public GameObject mineBTN;
    public GameObject campusBTN;
    public GameObject commercialHubBTN;
    public GameObject industrialZoneBTN;

    //TileInfo UI
    public GameObject tileInfo;
    public Vector3 mousePos;
    public float popupTime = 2;
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

    TerrainData td;
    FacilityData fd;

    DistrictInfo districtInfo;

    // 지형레이어
    //필요없음
    //int layerGrassLand;  // 6
    //int layerPlains;     // 7
    //int layerDesert;     // 8
    //int layerMountain;   // 9
    int fogLayer;
    //유닛 레이어
    int layerUnit;

    void Start()
    {
        isFirst = true;
        farmBTN.SetActive(false);
        mineBTN.SetActive(false);
        settleBTN.SetActive(false);

        //layerGrassLand = LayerMask.GetMask("GrassLand");
        //layerPlains = LayerMask.GetMask("Plains");
        //layerDesert = LayerMask.GetMask("Desert");
        //layerMountain = LayerMask.GetMask("Mountain");
        fogLayer = LayerMask.GetMask("HexFog");
        layerUnit = LayerMask.GetMask("Unit");
       
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
        if (!UIManager.IsPointerOverUIObject()&& Input.GetMouseButtonDown(0))
        {
            SelectUnit();

            if (isUnitSelected)
            {
                tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;
                td = tileTemp.GetComponent<TerrainData>();
                fd = tileTemp.GetComponent<FacilityData>();

            }
        }

        TileInfoPopUp();
    }


    //select
    public void SelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = layerUnit & ~fogLayer;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {
            unitInfo = hit.transform.gameObject;

            //이동할때로 옮겨 나중에
            unitInfo.GetComponent<NonCombatUnit>().CheckMyPos();

            unitType = unitInfo.GetComponent<NonCombatUnit>().non_CombatUnitType;
            isUnitSelected = true;

            UIPanelManager.instance.OpenPanel("UNIT_PANEL");
            if (unitType == Non_CombatUnitType.Settler)
            {
                settleBTN.SetActive(true);
            }
            else if (unitType == Non_CombatUnitType.Builder)
            {
                unitLimit = unitInfo.GetComponent<NonCombatUnit>().buildCount;

                farmBTN.SetActive(true);
                mineBTN.SetActive(true);
                campusBTN.SetActive(true);
                commercialHubBTN.SetActive(true);
                industrialZoneBTN.SetActive(true);
            }
        }
    }
    //public void SelectTile()
    //{

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    //int layerMask = 1 << LayerMask.NameToLayer("HexFog");


    //    int layerMask = (layerGrassLand | layerPlains | layerDesert | layerMountain) & ~fogLayer & ~layerUnit;

    //    if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
    //    {

    //        tileTemp = hit.transform;
    //        print(tileTemp.name);
    //        //settleBTN.SetActive(true);

    //        int layerNum = hit.transform.gameObject.layer;
    //        layerNum = LayerMask.GetMask(LayerMask.LayerToName(layerNum));

    //        if (layerNum == layerGrassLand || layerNum == layerPlains)
    //        {

    //            bool isHillis = hit.transform.gameObject.GetComponent<TerrainData>().isHills;
    //            if (isHillis)
    //            {
    //                farmBTN.SetActive(false);
    //            }
    //            else
    //            {
    //                mineBTN.SetActive(false);
    //            }
    //        }
    //    }
    //}
    //tileInfo popup
    public void TileInfoPopUp()
    {
        if(Camera.main == null)
        {
            return;
        }

        //레이쏴서
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


        if (Physics.Raycast(rayPoint, out hitInfo, 1000, ~fogLayer))
        {
            if (mousePos == Input.mousePosition)
            {
                currentTime += Time.deltaTime;
                if (currentTime > popupTime)
                {
                    isOpenPopup = true;

                    tileTemp = hitInfo.transform;
                    if (tileTemp.GetComponent<TerrainData>() != null)
                    {
                        tileTemp.GetComponent<TerrainData>().ShowTileInfo();
                        tileInfo.SetActive(true);
                    }
                    currentTime = 0;
                }
            }
            else
            {
                if (tileInfo.activeSelf == true)
                {
                    isOpenPopup = false;

                    tileInfo.SetActive(false);
                }
            }


        }
    }

    //Create buttons
    public void OnClickFarmBtn()
    {
        if (fd.facility == Facility.NONE)
        {
            fd.SetFacility(Facility.FARM);
            CreateFacility(3);
            tileTemp = null;
            isUnitSelected = false;

        }
        else return;
    }
    public void OnClickMineBtn()
    {
        if (fd.facility == Facility.NONE)
        {
            fd.SetFacility(Facility.MINE);
            CreateFacility(4);
            tileTemp = null;

            mineBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }

    public void OnClickCampusBtn()
    {
        if (fd.district == District.NONE && td.myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.CAMPUS);
            //CreateDistrict(0);
            CreateDistrict(District.CAMPUS);
            tileTemp = null;

            campusBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }
    public void OnClickCommercialHubBtn()
    {
        if (fd.district == District.NONE && td.myCenter.GetComponent<Territory>().distric_limit)
        {
            fd.SetDistrict(District.COMMERCAILHUB);
            //CreateDistrict(1);
            CreateDistrict(District.COMMERCAILHUB);
            tileTemp = null;

            commercialHubBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }
    public void OnclickIndustrialZoneBtn()
    {
        if (fd.district == District.NONE && td.myCenter.GetComponent<Territory>().distric_limit)
        {
            fd.SetDistrict(District.INDUSTRIALZONE);
            //CreateDistrict(2);
            CreateDistrict(District.INDUSTRIALZONE);
            tileTemp = null;

            industrialZoneBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }
    public void CreateTerritoryBtn()
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
        //worldCity.Add(tileTemp.transform.gameObject);
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

        settleBTN.SetActive(false);

        //선택된 유닛 제거 후 초기화
        isUnitSelected = false;

        Destroy(unitInfo);
        unitInfo = null;
    }


    public void CreateFacility(int chooseIndex)
    {
        tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;
        print(tileTemp);
        GameObject empty = Instantiate(icons[chooseIndex]);
        unitInfo.GetComponent<NonCombatUnit>().buildCount += 1;
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
        //FacilityData fd = tileTemp.GetComponent<FacilityData>();

        if (chooseIndex == -1)
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

    // TODO
    // parameter : 타일 x, y 좌표, 선택한 건물
    // 선택한 타일에 건물 모델 생성, 타일의 산출량 변경
    public void CreateDistrict(District id)
    {
        tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;

        districtInfo.pos = tileTemp;
        districtInfo.id = id;
        districtInfo.input = fd.districtInput;

        GameObject empty = Instantiate(icons[(int)id]);
        unitInfo.GetComponent<NonCombatUnit>().buildCount += 1;
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
        tt.AddDistrict(empty);

        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = new Vector3(0, 0.109f, 0);
        empty.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
    }

    //public void CreateDistrict(int chooseIndex)
    //{
    //    tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;

    //    GameObject empty = Instantiate(icons[chooseIndex]);
    //    unitInfo.GetComponent<NonCombatUnit>().buildCount += 1;
    //    Territory td.myCenter.GetComponent<Territory>() = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
    //    td.myCenter.GetComponent<Territory>().AddDistrict(empty);

    //    empty.transform.parent = tileTemp;
    //    empty.transform.position = tileTemp.position;
    //    empty.transform.localPosition = new Vector3(0, 0.109f, 0);//constrMng.iconPos[chooseIndex];
    //    //empty.transform.localEulerAngles = new Vector3(90, 0, 0);
    //    empty.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
    //}

}
