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
    public string koreanDesc;
    public int researchCost;
    public int remainCost;
    public bool isResearched;
    public List<InGameObjectId> unlockObjectId;
    public List<TechnologyId> requireTechId;


    public Technology()
    {
        id = TechnologyId.NONE;
        name = korean = "";
        koreanDesc = "";
        remainCost = researchCost = -1;
        unlockObjectId = new List<InGameObjectId>();
        requireTechId = new List<TechnologyId>();

        isResearched = false;
    }

    public Technology(TechnologyId id, string name, string korean, string koreanDesc, int researchCost, List<InGameObjectId> unlockObjectId, List<TechnologyId> requireTechId)
    {
        this.id = id;
        this.name = name;
        this.korean = korean;
        this.koreanDesc = koreanDesc;
        this.remainCost = this.researchCost = researchCost;
        this.unlockObjectId = unlockObjectId;
        this.requireTechId = requireTechId;

        isResearched = false;
    }

    public Technology(Technology technology)
    {
        this.id = technology.id;
        this.name = technology.name;
        this.korean = technology.korean;
        this.koreanDesc = technology.koreanDesc;
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
        sb.Append(string.Format("koreanDesc : {0}\n", this.koreanDesc));
        sb.Append(string.Format("researchCost : {0}\n", this.researchCost));
        sb.Append(string.Format("unlockObjectId : {0}\n", this.unlockObjectId));
        sb.Append(string.Format("requireTechId : {0}\n", this.requireTechId));

        return sb.ToString();
    }
}
