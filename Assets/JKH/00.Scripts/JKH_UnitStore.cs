using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//무지성
[System.Serializable]
public class UnitCost
{
    
    //유닛의 이름, 재화(eg. gold, wood)
    public string name;
    public int gold;
    public int wood;
    
}


public class JKH_UnitStore : MonoBehaviour
{
    public UnitCost unitcost = new UnitCost(); //무지성 2

    //myResource
    public int Mygold;
    public int Mywood;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
