using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFogManager : Singleton<HexFogManager>
{
    [SerializeField]
    public List<Hideable> findTargetList;

    private void Start()
    {
        findTargetList = new List<Hideable>();
    }
}
