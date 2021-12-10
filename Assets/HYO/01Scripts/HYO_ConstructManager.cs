using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
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

    // 지형레이어
    //필요없음
    int layerGrassLand;  // 6
    int layerPlains;     // 7
    int layerDesert;     // 8
    int layerMountain;   // 9
    int fogLayer;
    //유닛 레이어
    int layerUnit;

    void Start()
    {
        isFirst = true;
        farmBTN.SetActive(false);
        mineBTN.SetActive(false);
        settleBTN.SetActive(false);

        layerGrassLand = LayerMask.GetMask("GrassLand");
        layerPlains = LayerMask.GetMask("Plains");
        layerDesert = LayerMask.GetMask("Desert");
        layerMountain = LayerMask.GetMask("Mountain");
        fogLayer = LayerMask.GetMask("HexFog");
        layerUnit = LayerMask.GetMask("Unit");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectUnit();

            if (isUnitSelected)
            {
                tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;

                if(tileTemp.GetComponent<TerrainData>().myCenter == null)
                {
                    tileTemp = null;

                    farmBTN.SetActive(false);
                    mineBTN.SetActive(false);
                    campusBTN.SetActive(false);
                    commercialHubBTN.SetActive(false);
                    industrialZoneBTN.SetActive(false);
                }

            }
        }

        //if (Input.GetMouseButtonDown(0) && isUnitSelected)
        //{

        //    tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;
        //    //SelectTile(); 
        //}
        //else if (Input.GetMouseButtonDown(0) && isUnitSelected == false)
        //{
        //    SelectUnit();
        //}

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
    public void SelectTile()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //int layerMask = 1 << LayerMask.NameToLayer("HexFog");


        int layerMask = (layerGrassLand | layerPlains | layerDesert | layerMountain) & ~fogLayer & ~layerUnit;

        if (Physics.Raycast(ray, out hit, float.MaxValue, layerMask))
        {

            tileTemp = hit.transform;
            print(tileTemp.name);
            //settleBTN.SetActive(true);

            int layerNum = hit.transform.gameObject.layer;
            layerNum = LayerMask.GetMask(LayerMask.LayerToName(layerNum));

            if (layerNum == layerGrassLand || layerNum == layerPlains)
            {

                bool isHillis = hit.transform.gameObject.GetComponent<TerrainData>().isHills;
                if (isHillis)
                {
                    farmBTN.SetActive(false);
                }
                else
                {
                    mineBTN.SetActive(false);
                }
            }
        }
    }
    //tileInfo popup
    public void TileInfoPopUp()
    {
        //레이쏴서
        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // 1.1 만약 마우스가 움직이지 않는다면
        //      2. 만약 2초가 흘렀다면
        //      3. 마우스의 위치에 타일이 있는지 검사하고싶다.
        //      4. 만약 타일이 있다면 팝업을 띄우고싶다.
        // 1.2 그렇지않고 팝업을 보여주는 중이라면 팝업을 끄고싶다.


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
        if (tileTemp.GetComponent<FacilityData>().facility == Facility.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.FARM);
            CreateFacility(3);
            tileTemp = null;
            isUnitSelected = false;

        }
        else return;
    }
    public void OnClickMineBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().facility == Facility.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.MINE);
            CreateFacility(4);
            tileTemp = null;

            mineBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }
    public void OnClickCampusBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().district == District.NONE && tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.CAMPUS);
            CreateDistrict(0);
            tileTemp = null;

            campusBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }
    public void OnClickCommercialHubBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().district == District.NONE && tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.COMMERCAILHUB);
            CreateDistrict(1);
            tileTemp = null;

            commercialHubBTN.SetActive(false);

            isUnitSelected = false;
        }
        else return;
    }
    public void OnclickIndustrialZoneBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().district == District.NONE && tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.INDUSTRIALZONE);
            CreateDistrict(2);
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
        tileTemp.gameObject.AddComponent<Territory>();
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

    }
    public void CreateDistrict(int chooseIndex)
    {
        tileTemp = unitInfo.GetComponent<NonCombatUnit>().myTilePos.transform;

        GameObject empty = Instantiate(icons[chooseIndex]);
        unitInfo.GetComponent<NonCombatUnit>().buildCount += 1;
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
        tt.AddDistrict(empty);

        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = new Vector3(0, 0.109f, 0);//constrMng.iconPos[chooseIndex];
        //empty.transform.localEulerAngles = new Vector3(90, 0, 0);
        //empty.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
    }

}
