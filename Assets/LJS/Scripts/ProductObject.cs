using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

[Serializable]
public class ProductObject
{
    public InGameObjectId id;
    public TypeIdBase type;
    public string name;
    public string korean;
    public int productCost;
    public int goldCost;
    public int maintenanceCost;
    public int remainCost;
    public TechnologyId requireTechId;


    public ProductObject()
    {
        id = 0;
        name = korean = "";
        remainCost = maintenanceCost = goldCost = productCost = -1;
        requireTechId = 0;
    }

    public ProductObject(TypeIdBase type, InGameObjectId id, string name, string korean, int productCost, int goldCost, int maintenanceCost, TechnologyId requireTechId)
    {
        this.type = type;
        this.id = id;
        this.name = name;
        this.korean = korean;
        this.remainCost = this.productCost = productCost;
        this.goldCost = goldCost;
        this.maintenanceCost = maintenanceCost;
        this.requireTechId = requireTechId;
    }

    public ProductObject(ProductObject productObject)
    {
        this.type = productObject.type;
        this.id = productObject.id;
        this.name = productObject.name;
        this.korean = productObject.korean;
        this.productCost = productObject.productCost;
        this.goldCost = productObject.goldCost;
        this.maintenanceCost = productObject.maintenanceCost;
        this.remainCost = productObject.remainCost;
        this.requireTechId = productObject.requireTechId;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(string.Format("type : {0}\n", this.type));
        sb.Append(string.Format("id : {0}\n", this.id));
        sb.Append(string.Format("name : {0}\n", this.name));
        sb.Append(string.Format("korean : {0}\n", this.korean));
        sb.Append(string.Format("productCost : {0}\n", this.productCost));
        sb.Append(string.Format("goldCost : {0}\n", this.goldCost));
        sb.Append(string.Format("maintenanceCost : {0}\n", this.maintenanceCost));
        sb.Append(string.Format("requireTechId : {0}\n", this.requireTechId));

        return base.ToString();
    }
}
