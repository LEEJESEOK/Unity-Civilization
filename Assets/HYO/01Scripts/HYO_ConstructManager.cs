using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYO_ConstructManager : Singleton<HYO_ConstructManager>
{
    public Sprite[] icons;
    public GameObject emptyPre;
    public GameObject[] districtOn_;
    public Vector3[] iconPos;
    public int iconPosTemp;
    public Transform evironment;

    void Start()
    {
        districtOn_ = evironment.GetComponentInChildren<FacilityData>().districtOn;
    }

}
