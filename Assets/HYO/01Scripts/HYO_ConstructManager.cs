using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
    public static HYO_ConstructManager instance;
    private void Awake()
    {
        instance = this;
    }
    public Sprite[] icons;
    public GameObject emptyPre;
    public GameObject[] districtOn_;
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
    public Transform tileTemp;

    void Start()
    {
        districtOn_ = evironment.GetComponentInChildren<FacilityData>().districtOn;
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
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                int layerNum = hit.transform.gameObject.layer;
                if (layerNum == 6 || layerNum == 7 || layerNum == 8 || layerNum == 9)
                {
                    tileTemp = hit.transform;
                    settleBTN.SetActive(true);

                }

            }
        }
    }

    //Create buttons
    public void OnClickFarmBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().isfacility == false)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.FARM);
            CreateFacility(3);
            tileTemp.GetComponent<FacilityData>().isfacility = true;

        }
        else return;
    }
    public void OnClickMineBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().isfacility == false)
        {
            tileTemp.GetComponent<FacilityData>().SetFacility(Facility.MINE);
            CreateFacility(4);
            tileTemp.GetComponent<FacilityData>().isfacility = true;
        }
        else return;
    }
    public void OnClickCampusBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().canCreate && tileTemp.GetComponent<FacilityData>().district == District.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.CAMPUS);
            CreateDistrict(0);
        }
        else return;
    }
    public void OnClickCommercialHubBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().canCreate && tileTemp.GetComponent<FacilityData>().district == District.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.COMMERCAILHUB);
            CreateDistrict(1);
        }
        else return;
    }
    public void OnclickIndustrialZoneBtn()
    {
        if (tileTemp.GetComponent<FacilityData>().canCreate && tileTemp.GetComponent<FacilityData>().district == District.NONE)
        {
            tileTemp.GetComponent<FacilityData>().SetDistrict(District.INDUSTRIALZONE);
            CreateDistrict(2);
        }
        else return;
    }
    public void CreateTerritoryBtn()
    {
        tileTemp.gameObject.AddComponent<Territory>();
    }


    public void CreateFacility(int chooseIndex)
    {
        GameObject empty = Instantiate(emptyPre);
        FacilityData fd = tileTemp.GetComponent<FacilityData>();
        fd.AddDistrict(empty);

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
        FacilityData fd = tileTemp.GetComponent<FacilityData>();
        fd.AddDistrict(empty);

        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = new Vector3(0.169f, 0.104999997f, 0.307999998f);//constrMng.iconPos[chooseIndex];
        empty.transform.localEulerAngles = new Vector3(90, 0, 0);
        empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        empty.GetComponent<SpriteRenderer>().sprite = HYO_ConstructManager.instance.icons[chooseIndex/*fd.iconNum*/];
    }

}
