using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������
[System.Serializable]
public class UnitCost
{
    
    //������ �̸�, ��ȭ(eg. gold, wood)
    public string name;
    public int gold;
    public int wood;
    
}


public class JKH_UnitStore : MonoBehaviour
{
    public UnitCost unitcost = new UnitCost(); //������ 2

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
