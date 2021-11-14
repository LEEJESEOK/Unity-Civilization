using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Technology
{
    public string name;
    public int researchCost;
    public List<System.Object> unlockObject;
    public List<Technology> leadToTech;
}
