using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JKH_Note : MonoBehaviour
{
    void Start()
    {
        Dictionary<string, int> unitCost = new Dictionary<string, int>();
        //Ancient
        unitCost.Add("scout", 1);
        unitCost.Add("warrior", 2);
        unitCost.Add("slinger", 1);
        unitCost.Add("archer", 1);
        unitCost.Add("spearman", 1);
        unitCost.Add("heavyChariot", 1);
        unitCost.Add("Galley", 1);

        //2개 이상은 어떻게 함? sl
    }

    void Update()
    {
        
    }
}
