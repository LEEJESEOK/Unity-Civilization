using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInfo
{
    public string name;

    // 보유중인 유닛
    public List<Unit> units;
    // 보유한 도시
    public List<Territory> cities;
    // 연구한 기술
    public List<Technology> technologies;
    // 연구중인 기술
    public Technology ongoingTechnology;

    [Header("Resources")]
    // 보유한 자원
    public int food;
    public int production;
    public int science;
    public int culture;
    public int faith;
    public int faithChange;
    public int gold;
    public int goldChange;


    public PlayerInfo()
    {
        units = new List<Unit>();
        cities = new List<Territory>();
        technologies = new List<Technology>();
        ongoingTechnology = null;
    }
}
