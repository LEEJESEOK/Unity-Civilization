using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

public class Player : MonoBehaviour
{
    public bool isTurn;
    public int playerId;
    public Camera playerCamera;
    public PlayerInfo info;
    int techCarryCost;

    // Start is called before the first frame update
    void Start()
    {
        info = new PlayerInfo();
        info.name = "Player " + (playerId + 1);
        gameObject.name = info.name;

        if (GameManager.instance.test)
        {
            info.gold = GameManager.instance.testStartGold;
            info.goldChange = GameManager.instance.testGoldChange;
            info.science = GameManager.instance.initialScience;
        }

        for (int i = 0; i < TechnologyDataManager.instance.technologies.Count; ++i)
        {
            Technology technology = new Technology(TechnologyDataManager.instance.technologies[i]);
            info.technologies.Add(technology);
        }

        for (int i = 0; i < GameManager.instance.initialUnits.Count; ++i)
        {
            GameObject unitObject = Instantiate(GameManager.instance.initialUnits[i]);
            Unit unit = unitObject.GetComponent<Unit>();
            unit.playerId = playerId;
            unit.SetObjectColor();

            info.units.Add(unit);

            #region test
            // 초기 위치 지정
            Vector3 pos = transform.position + Vector3.left * (1.5f) * i;
            pos.y = -0.9f;
            unitObject.transform.position = pos;
            #endregion

            unitObject.transform.rotation = Quaternion.Euler(0, 180f, 0);

            HexFogManager.instance.fieldOfViews[unit.playerId].Add(unitObject.GetComponentInChildren<FieldOfView>());
            HexFogManager.instance.units[unit.playerId].Add(unit);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTurn)
        {
            TurnUpdate();
        }
    }

    public void StartTurn()
    {
        print(string.Format("[Start Turn] {0}", name));

        // UI를 플레이어의 정보로 변경
        // 카메라
        playerCamera.gameObject.SetActive(true);

        // 자원
        if (GameManager.instance.useScience)
        {
            info.science = GameManager.instance.initialScience;
            for (int i = 0; i < info.cities.Count; ++i)
            {
                info.science += info.cities[i].totalOutput.TotalScience;
            }
        }
        UIManager.instance.UpdateResource(info.science, 0, 0, 0, info.gold, info.goldChange);

        // 연구
        if (info.ongoingTechnology != null)
            UIManager.instance.UpdateSelectedTechnology(info.ongoingTechnology);
        else
            UIManager.instance.InitSelectedTechnology();

        // 유닛
        for (int i = 0; i < info.units.Count; ++i)
        {
            Unit unit = info.units[i];

            // 이동력 회복
            unit.movePower = unit.maxMovePower;

            // TODO 체력 회복
        }

        isTurn = true;
    }

    public void TurnUpdate()
    {
        // 화면 조작
        UIManager.instance.CameraMove(playerCamera);
        UIManager.instance.CameraZoom(playerCamera);

        if (UIPanelManager.instance.isEmpty() && Input.GetKeyUp(KeyCode.Return))
        {
            GameManager.instance.TurnEnd();
        }
    }

    // 자원 생산
    // 도시의 생산 진행
    // 연구 진행
    // 유닛 회복
    public void EndTurn()
    {
        // 카메라
        playerCamera.gameObject.SetActive(false);

        // 생산 골드만큼 소지골드에 추가
        info.gold += info.goldChange;

        // 도시들의 생산 진행(생산량)
        // 완료된 생산 반영
        // 남은 생산량은 이월됨
        for (int i = 0; i < info.cities.Count; ++i)
        {
            // Territory tt = info.cities[i].GetComponent<Territory>();
            // tt.DistrictProcess();
        }

        // 선택한 연구 진행(과학량)
        // 완료된 연구 반영
        // 남은 생산량은 이월
        info.ongoingTechnology.remainCost -= info.science;
        if (techCarryCost > 0)
        {
            info.ongoingTechnology.remainCost -= techCarryCost;
            techCarryCost = 0;
        }
        // 연구 완료
        if (info.ongoingTechnology.remainCost <= 0)
        {
            info.ongoingTechnology.isResearched = true;
            techCarryCost = -info.ongoingTechnology.remainCost;
        }

        // 대기중인 유닛의 체력 회복
        for (int i = 0; i < info.units.Count; ++i)
        {

        }

        isTurn = false;
    }

    // 유닛 생성 -> 플레이어의 유닛 리스트에 추가
    public void ConstructUnit(Unit unit)
    {
        info.units.Add(unit);
    }

    // 도시 건설 -> 플레이어의 도시 리스트에 추가
    public void BuildCity(Territory city)
    {
        info.cities.Add(city);
    }

    // 새로 연구할 기술 선택
    public void ChooseResearch(Technology technology)
    {
        info.ongoingTechnology = technology;
    }
}
