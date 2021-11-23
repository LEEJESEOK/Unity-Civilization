using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
    public Sprite[] icons;
    public GameObject emptyPre;
    public GameObject cityGate;
    public Vector3[] iconPos;
    public Transform evironment;

    public Facility facility;
    //button UI
    public GameObject farmBTN;
    public GameObject mineBTN;
    public GameObject settleBTN;
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

    bool isOpenPopup;

    void Start()
    {

    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                int layerNum = hit.transform.gameObject.layer;
                if (layerNum == 6 || layerNum == 7)
                {
                    tileTemp = hit.transform;
                    if (hit.transform.gameObject.GetComponent<TerrainData>().isHills)
                    {
                        mineBTN.SetActive(true);
                    }
                    else
                    {
                        farmBTN.SetActive(true);
                    }
                }
                else if (layerNum == 8 || layerNum == 9)
                {
                    tileTemp = hit.transform;
                }
                else if (layerNum == 6 || layerNum == 7 || layerNum == 8 || layerNum == 9)
                {
                    tileTemp = hit.transform;
                    settleBTN.SetActive(true);

                }
            }
        }

        //레이쏴서
        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // 1.1 만약 마우스가 움직이지 않는다면
        //      2. 만약 2초가 흘렀다면
        //      3. 마우스의 위치에 타일이 있는지 검사하고싶다.
        //      4. 만약 타일이 있다면 팝업을 띄우고싶다.
        // 1.2 그렇지않고 팝업을 보여주는 중이라면 팝업을 끄고싶다.


        if(isOpenPopup == false)
            mousePos = Input.mousePosition;


        if (Physics.Raycast(rayPoint, out hitInfo))
        {
            if(mousePos == Input.mousePosition)
            {
                currentTime += Time.deltaTime;
                if(currentTime > popupTime)
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
                if(tileInfo.activeSelf == true)
                {
                    isOpenPopup = false;

                    tileInfo.SetActive(false);
                }
            }



            //// 팝업으로 보려는 시도 중
            //if (isTryShowTileInfoPopup)
            //{
            //    // 창을 띄우고싶다.
            //    if (false == isTryShowTileInfoPopup)
            //    {
            //        StartCoroutine(tileInfo_Cour());
            //        isTryShowTileInfoPopup = true;
            //    }
            //}
            //else
            //{
            //    // 타일 위에 마우스 포인트가 올라왔다면
            //    //타일이 맞으면
            //    tileTemp = hitInfo.transform;
            //    if (tileTemp.GetComponent<TerrainData>() != null)
            //    {
            //        isTryShowTileInfoPopup = true;
            //        currentTime = 0;
            //        mousePos = Input.mousePosition;
            //    }
            
            //}

        }
    }

    //bool isTryShowTileInfoPopup;
    //IEnumerator tileInfo_Cour()
    //{

    //    //창띄우기
    //    tileTemp.GetComponent<TerrainData>().ShowTileInfo();
    //    tileInfo.SetActive(true);
    //    while (true)
    //    {
    //        //마우스가 움직이면
    //        if (mousePos != Input.mousePosition)
    //        {
    //            //끈다
    //            tileInfo.SetActive(false);
    //        }
    //        yield return 0;
    //    }

    //}
    //Create buttons
    public void OnClickFarmBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().facility == Facility.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.FARM);
            CreateFacility(3);
            tileTemp = null;

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
        }
        else return;
    }
    public void CreateTerritoryBtn()
    {
        
        Collider[] centers = Physics.OverlapSphere(tileTemp.position, 1);

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
        city.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        tileTemp = null;
    }


    public void CreateFacility(int chooseIndex)
    {
        GameObject empty = Instantiate(emptyPre);
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
        //FacilityData fd = tileTemp.GetComponent<FacilityData>();

        if (chooseIndex == -1)
        {
            return;
        }
        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = new Vector3(0, 0.109f, 0);
        empty.transform.localEulerAngles = new Vector3(90, 0, 0);
        empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        empty.GetComponent<SpriteRenderer>().sprite = icons[chooseIndex];

    }
    public void CreateDistrict(int chooseIndex)
    {
        GameObject empty = Instantiate(emptyPre);
        Territory tt = tileTemp.GetComponent<TerrainData>().myCenter.gameObject.GetComponent<Territory>();
        tt.AddDistrict(empty);

        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = new Vector3(0, 0.109f, 0);//constrMng.iconPos[chooseIndex];
        empty.transform.localEulerAngles = new Vector3(90, 0, 0);
        empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        empty.GetComponent<SpriteRenderer>().sprite = instance.icons[chooseIndex/*fd.iconNum*/];
    }

}
