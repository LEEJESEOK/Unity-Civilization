using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Non_CombatUnitType
{
    Settler,
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
            }
        }
    }
    
    //create button
    public void OnClickFarmBtn()
    {
        tileTemp.GetComponent<FacilityData>().SetFacility(Facility.FARM);
        Create(3); 
    }
    public void OnClickMineBtn()
    {
        tileTemp.GetComponent<FacilityData>().SetFacility(Facility.MINE);
        Create(4);
    }
    
    public void Create(int chooseIndex)
    {
        GameObject empty = Instantiate(constrMng.emptyPre);
        for (int i = 0; i < constrMng.districtOn_.Length; i++)
        {
            if (constrMng.districtOn_[i] = null)
            {
                constrMng.districtOn_[i] = empty;
                break;
            }
        }
        if (chooseIndex == -1)
        {
            return;
        }
        empty.transform.parent = tileTemp;
        empty.transform.position = tileTemp.position;
        empty.transform.localPosition = new Vector3(0, 0.109f, 0);
        empty.transform.localEulerAngles = new Vector3(90, 0, 0);
        empty.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        FacilityData fd = tileTemp.GetComponent<FacilityData>();
        empty.GetComponent<SpriteRenderer>().sprite = constrMng.icons[chooseIndex];
    }
   
  
}
