using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Player : MonoBehaviour
{
    public int playerId;
    public PlayerInfo info;
    public bool isTurn;


    // Start is called before the first frame update
    void Start()
    {
        info = new PlayerInfo(GameManager.instance.startGold);
        info.name = "Player " + playerId;
        print(info.gold);

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
        // UI를 플레이어의 정보로 변경
        StringBuilder sb = new StringBuilder();
        sb.Append("science : ");
        sb.Append(info.science);
        sb.Append(", gold : ");
        sb.Append(info.gold);
        print(sb.ToString());
        // 자원
        UIManager.instance.UpdateResource(info.science, 0, 0, 0, info.gold, info.goldChange);
        // 연구

        isTurn = true;
    }

    public void TurnUpdate()
    {
    }

    // 자원 생산
    // 도시의 생산 진행
    // 연구 진행
    // 유닛 회복
    public void EndTurn()
    {
        // 생산 골드만큼 소지골드에 추가
        info.gold += info.goldChange;

        // 도시들의 생산 진행(생산량)
        // 완료된 생산 반영
        // 남은 생산량은 이월됨
        for (int i = 0; i < info.cities.Count; ++i)
        {

        }

        // 선택한 연구 진행(과학량)
        // 완료된 연구 반영
        // 남은 생산량은 이월
        info.ongoingTechnology.remainCost -= info.science;
        // 연구 완료
        if (info.ongoingTechnology.remainCost <= 0)
        {
            info.ongoingTechnology.isResearched = true;
        }

        // 대기중인 유닛의 체력 회복
        for (int i = 0; i < info.units.Count; ++i)
        {

        }
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
