using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
    public static HYO_ConstructManager instance;
    private void Awake()
    {
        instance = this;
    }
    public Sprite[] icons;
    public GameObject emptyPre;
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

    public Transform tileTemp;
    public GameObject centerCheck;

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

        //Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitInfo;

        //if (Physics.Raycast(rayPoint, out hitInfo))
        //{
        //    tileTemp = hitInfo.transform;
        //    if (tileTemp.GetComponent<TerrainData>() != null)
        //    {
        //        StartCoroutine(tileInfo_Cour());
        //    }
        //    else if(hitInfo.transform != tileTemp)
        //    {
        //        tileInfo.SetActive(false);
        //    }

        //}

    }
    IEnumerator tileInfo_Cour()
    {
        yield return new WaitForSeconds(2);
        tileTemp.GetComponent<TerrainData>().ShowTileInfo();
        tileInfo.SetActive(true);

    }
    //Create buttons
    public void OnClickFarmBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().facility == Facility.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.FARM);
            CreateFacility(3);

        }
        else return;
    }
    public void OnClickMineBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().facility == Facility.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.MINE);
            CreateFacility(4);
        }
        else return;
    }
    public void OnClickCampusBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().district == District.NONE && tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.CAMPUS);
            CreateDistrict(0);
        }
        else return;
    }
    public void OnClickCommercialHubBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().district == District.NONE && tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.COMMERCAILHUB);
            CreateDistrict(1);
        }
        else return;
    }
    public void OnclickIndustrialZoneBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().district == District.NONE && tileTemp.GetComponent<TerrainData>().myCenter.GetComponent<Territory>().distric_limit)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.INDUSTRIALZONE);
            CreateDistrict(2);
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
        empty.GetComponent<SpriteRenderer>().sprite = HYO_ConstructManager.instance.icons[chooseIndex/*fd.iconNum*/];
    }

}
