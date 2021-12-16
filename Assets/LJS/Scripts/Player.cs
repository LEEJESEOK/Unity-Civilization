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
        info.name = "Player " + playerId;
        gameObject.name = info.name;

        if (GameManager.instance.test)
        {
            info.gold = GameManager.instance.testStartGold;
            info.goldChange = GameManager.instance.testGoldChange;
            info.science = GameManager.instance.testStartScience;
        }

        for (int i = 0; i < TechnologyManager.instance.technologies.Count; ++i)
        {
            Technology technology = new Technology(TechnologyManager.instance.technologies[i]);
            info.technologies.Add(technology);
        }

        for (int i = 0; i < GameManager.instance.initialUnits.Count; ++i)
        {
            GameObject unit = Instantiate(GameManager.instance.initialUnits[i]);

            #region  test
            if (i == 0)
                unit.transform.position = new Vector3(-1.6f, -0.9f, -0.25f);
            else
                unit.transform.position = new Vector3(1.8f, -0.9f, -0.25f);
            #endregion
            unit.transform.rotation = Quaternion.Euler(0, 180f, 0);

            info.units.Add(unit);

            HexFogManager.instance.fieldOfViews.Add(unit.GetComponentInChildren<FieldOfView>());

            unit.SetActive(false);
        }
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
        // 카메라
        playerCamera.gameObject.SetActive(true);

        // 자원
        print(string.Format("[Start Turn] {0}", name));
        UIManager.instance.UpdateResource(info.science, 0, 0, 0, info.gold, info.goldChange);
        // 연구
        if (info.ongoingTechnology != null)
            UIManager.instance.UpdateSelectedTechnology(info.ongoingTechnology);
        else
        {
            UIManager.instance.InitSelectedTechnology();
        }

        // 유닛 변경
        if (GameManager.instance.test)
        {
            for (int i = 0; i < info.units.Count; ++i)
                info.units[i].SetActive(true);
        }

        isTurn = true;
    }

    public void TurnUpdate()
    {
        // 화면 조작
        UIManager.instance.CameraMove(playerCamera);
        UIManager.instance.CameraZoom(playerCamera);
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
            Territory tt = info.cities[i].GetComponent<Territory>();
            tt.DistrictProcess();
            
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

        // test 테스트
        if (GameManager.instance.test)
        {
            for (int i = 0; i < info.units.Count; ++i)
                info.units[i].SetActive(false);
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
