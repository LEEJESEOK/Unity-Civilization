using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInfo info;
    public int playerId;
    public bool isTurn;

    // Start is called before the first frame update
    void Start()
    {
        info = new PlayerInfo();
        info.name = "Player " + playerId;

        gameObject.name = info.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurn)
            TurnUpdate();
    }

    public void StartTurn()
    {
        isTurn = true;
    }

    public void TurnUpdate()
    {
    }

    public void EndTurn()
    {

    }

    // 유닛 생성 -> 플레이어의 유닛 리스트에 추가
    public void ConstructUnit(GameObject unit)
    {
        info.units.Add(unit);
    }

    // 도시 건설 -> 플레이어의 도시 리스트에 추가
    public void BuildCity(GameObject city)
    {
        info.cities.Add(city);
    }

    // 새로 연구할 기술 선택
    public void ChooseResearch(Technology technology)
    {
        info.ongoingTechnology = technology;
    }
}
