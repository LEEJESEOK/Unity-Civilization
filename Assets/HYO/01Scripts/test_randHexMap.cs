using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandHexCell
{
    int x;
    int y;
    int z;

    public RandHexCell(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class test_randHexMap : MonoBehaviour
{
    void Start()
    {
        RandHexCell randHexCell = new RandHexCell(1, 2, 3);
    }

    void Update()
    {
        
    }
}
