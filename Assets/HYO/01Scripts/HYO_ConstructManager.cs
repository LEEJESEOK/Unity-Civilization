using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYO_ConstructManager : MonoBehaviour
{
    public static HYO_ConstructManager instance;
    private void Awake()
    {
        instance = this;
    }
    public Sprite[] icons;

    
}