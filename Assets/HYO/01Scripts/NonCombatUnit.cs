using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Non_CombatUnitType
{
    Settler=TypeIdBase.UNIT,
    Builder
}
public class NonCombatUnit : MonoBehaviour
{
    public Non_CombatUnitType non_CombatUnitType;
    HYO_ConstructManager constrMng;
    public Facility facility;
    //button UI
    public GameObject farmBTN;
    public GameObject mineBTN;
    public GameObject settleBTN;
    public GameObject campusBTN;
    public GameObject commercialHubBTN;
    public GameObject industrialZoneBTN;
    public Transform tileTemp;

    public void NonCambatUnitCase()
    {
        switch (non_CombatUnitType)
        {
            case Non_CombatUnitType.Settler:
                break;
            case Non_CombatUnitType.Builder:
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        constrMng = HYO_ConstructManager.instance;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && non_CombatUnitType==Non_CombatUnitType.Builder)
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
                else if(layerNum ==8 || layerNum == 9)
                {
                    tileTemp = hit.transform;
                }
            }
        }
        else if(Input.GetMouseButtonDown(0) && non_CombatUnitType == Non_CombatUnitType.Settler)
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
        tileTemp.GetComponent<FacilityData>().SetFacility(Facility.FARM);
        CreateFacility(3);
    }
    public void OnClickMineBtn()
    {
        tileTemp.GetComponent<FacilityData>().SetFacility(Facility.MINE);
        CreateFacility(4);
    }
    public void OnClickCampusBtn()
    {
        tileTemp.GetComponent<FacilityData>().SetDistrict(District.CAMPUS);
        CreateDistrict(0);
    }
    public void OnClickCommercialHubBtn()
    {
        tileTemp.GetComponent<FacilityData>().SetDistrict(District.COMMERCAILHUB);
        CreateDistrict(1);
    }
    public void OnclickIndustrialZoneBtn()
    {
        tileTemp.GetComponent<FacilityData>().SetDistrict(District.INDUSTRIALZONE);
        CreateDistrict(2);
    }
    public void CreateFacilityTerritoryBtn()
    {
        tileTemp.gameObject.AddComponent<Territory>();   
    }


    public void CreateFacility(int chooseIndex)
    {
        GameObject empty = Instantiate(constrMng.emptyPre);
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
        empty.GetComponent<SpriteRenderer>().sprite = constrMng.icons[chooseIndex];

    }
    public void CreateDistrict(int chooseIndex)
    {
        GameObject empty = Instantiate(constrMng.emptyPre);
        FacilityData fd = tileTemp.GetComponent<FacilityData>();
        fd.AddDistrict(empty);

        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = constrMng.iconPos[chooseIndex];
        empty.transform.localEulerAngles = new Vector3(90, 0, 0);
        empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        empty.GetComponent<SpriteRenderer>().sprite = HYO_ConstructManager.instance.icons[fd.iconNum];
    }

}