using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Technology
{
    public int id;
    public string name;
    public int researchCost;
    public List<object> unlockObject;
    public List<int> leadToTechId;

    public Technology()
    {
        id = -1;
        name = "";
        researchCost = -1;
        unlockObject = new List<object>();
        leadToTechId = new List<int>();
    }

    public Technology(int id, string name, int researchCost, List<object> unlockObject, List<int> leadToTechId)
    {
        this.id = id;
        this.name = name;
        this.researchCost = researchCost;
        this.unlockObject = unlockObject;
        this.leadToTechId = leadToTechId;
    }
}
