using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        units = new List<GameObject>();
        cities = new List<GameObject>();
        technologies = new List<Technology>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ConstructUnit()
    {

    }

    public void ChooseResearch()
    {
        
    }

}
