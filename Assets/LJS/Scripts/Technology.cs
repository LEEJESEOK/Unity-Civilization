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
    public int[] unlockObjectId;
    public int[] leadToTechId;

    public Technology()
    {
        id = -1;
        name = "";
        researchCost = -1;
        unlockObjectId = new int[] { };
        leadToTechId = new int[] { };
    }

    public Technology(int id, string name, int researchCost, int[] unlockObjectId, int[] leadToTechId)
    {
        this.id = id;
        this.name = name;
        this.researchCost = researchCost;
        this.unlockObjectId = unlockObjectId;
        this.leadToTechId = leadToTechId;
    }
}
