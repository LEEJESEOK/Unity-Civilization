using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[Serializable]
public class Technology
{
    public TechnologyId id;
    public string name;
    public string korean;
    public int researchCost;
    public int remainCost;
    public bool isResearched;
    public int[] unlockObjectId;
    public List<TechnologyId> requireTechId;


    public Technology()
    {
        id = 0;
        name = korean = "";
        researchCost = -1;
        remainCost = -1;
        unlockObjectId = new int[] { };
        requireTechId = new List<TechnologyId>();

        isResearched = false;
    }

    public Technology(TechnologyId id, string name, string korean, int researchCost, int[] unlockObjectId, List<TechnologyId> requireTechId)
    {
        this.id = id;
        this.name = name;
        this.korean = korean;
        this.researchCost = researchCost;
        this.remainCost = this.researchCost;
        this.unlockObjectId = unlockObjectId;
        this.requireTechId = requireTechId;

        isResearched = false;
    }

    public Technology(Technology technology)
    {
        this.id = technology.id;
        this.name = technology.name;
        this.korean = technology.korean;
        this.researchCost = technology.researchCost;
        this.remainCost = technology.remainCost;
        this.isResearched = technology.isResearched;
        this.unlockObjectId = technology.unlockObjectId;
        this.requireTechId = technology.requireTechId;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(string.Format("id : {0}\n", this.id));
        sb.Append(string.Format("name : {0}\n", this.name));
        sb.Append(string.Format("korean : {0}\n", this.korean));
        sb.Append(string.Format("researchCost : {0}\n", this.researchCost));
        sb.Append(string.Format("unlockObjectId : {0}\n", this.unlockObjectId));
        sb.Append(string.Format("requireTechId : {0}\n", this.requireTechId));

        return sb.ToString();
    }
}
