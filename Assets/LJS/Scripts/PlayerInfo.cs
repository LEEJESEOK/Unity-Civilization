using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerInfo
{
    public string name;

    // 보유중인 유닛
    public List<GameObject> units;
    // 보유한 도시
    public List<GameObject> cities;
    // 연구한 기술
    public List<Technology> technologies;
    // 연구중인 기술
    public Technology ongoingTechnology;

    [Header("Resources")]
    // 보유한 자원
    int food;
    int production;
    int gold;
    int science;


    public PlayerInfo()
    {
        units = new List<GameObject>();
        cities = new List<GameObject>();
        technologies = new List<Technology>();

        ongoingTechnology = new Technology();

        food = production = gold = science = 0;
    }

}
