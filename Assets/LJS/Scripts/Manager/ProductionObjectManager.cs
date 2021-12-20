using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionObjectManager : Singleton<ProductionObjectManager>
{
    List<ProductionObjectId> products;


    // Start is called before the first frame update
    void Start()
    {
        products = new List<ProductionObjectId>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
