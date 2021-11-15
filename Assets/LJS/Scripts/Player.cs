using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Player
{
    // 보유중인 유닛
    public List<GameObject> units;
    // 보유한 도시
    public List<GameObject> cities;
    // 연구한 기술
    public List<Technology> technologies;
    // 연구중인 기술
    public Technology ongoingTechnology;
    // 보유한 자원
    int food, production, gold, science;


    public Player()
    {
        units = new List<GameObject>();
        cities = new List<GameObject>();
        technologies = new List<Technology>();

        ongoingTechnology = new Technology();

        food = production = gold = science = 0;
    }

    // 유닛 생성 -> 플레이어의 유닛 리스트에 추가
    public void ConstructUnit(GameObject unit)
    {
        units.Add(unit);
    }

    // 도시 건설 -> 플레이어의 도시 리스트에 추가
    public void BuildCity(GameObject city)
    {
        cities.Add(city);
    }

    // 새로 연구할 기술 선택
    public void ChooseResearch(Technology technology)
    {
        ongoingTechnology = technology;
    }

}
