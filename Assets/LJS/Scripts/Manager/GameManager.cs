using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    public bool test;
    public int testStartScience;
    public int testStartGold;
    public int testGoldChange;

    [Header("Common")]
    public bool useScience;
    public bool useCulture;
    public bool useFaith;
    public bool useGold;

    // public float cameraMoveSpeed;
    public float cameraZoomSpeed;

    [Header("Player")]
    public GameObject playerPrefab;
    public int initPlayerCount;
    public List<Player> players;
    int _currentPlayerId;
    public int currentPlayerId { get => _currentPlayerId; }
    public Player currentPlayer { get => players[currentPlayerId]; }

    // 게임 시작시 기본 제공 유닛
    public List<GameObject> initialUnits;
    public List<GameObject> startPoints;

    [Header("Play")]
    public bool isSelect;
    public GameObject currentSelect;
    public ObjectType currentSelectType;

    GameObject mouseDownObject;
    GameObject mouseUpObject;

    LayerMask fogLayer;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        StartCoroutine(DelayedStartCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        #region object select
        // MouseDown 오브젝트
        if ((!UIManager.IsPointerOverUIObject()) && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ~fogLayer))
            {
                mouseDownObject = hit.transform.gameObject;
            }
        }
        // MouseUp 오브젝트
        if ((!UIManager.IsPointerOverUIObject()) && Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue, ~fogLayer))
            {
                mouseUpObject = hit.transform.gameObject;
            }
        }
        // MouseDownObject, MouseUpObject 같으면 클릭
        if ((mouseDownObject != null) && (mouseUpObject != null) && (mouseDownObject == mouseUpObject))
        {
            currentSelect = mouseDownObject;
            mouseDownObject = mouseUpObject = null;

            GameObjectInfo gameObjectInfo = currentSelect.GetComponent<GameObjectInfo>();
            if ((gameObjectInfo != null) && (gameObjectInfo.playerId == currentPlayerId))
            {
                MapManager.instance.DeleteCube();
                UIPanelManager.instance.ClosePanel("UNIT_PANEL");
                UIPanelManager.instance.ClosePanel("CITY_PANEL");

                ObjectType type = gameObjectInfo.type;
                switch (type)
                {
                    case ObjectType.NON_COMBAT_UNIT:
                        currentSelectType = ObjectType.NON_COMBAT_UNIT;
                        SelectNonCombatUnit();
                        break;
                    case ObjectType.COMBAT_UNIT:
                        currentSelectType = ObjectType.COMBAT_UNIT;
                        SelectCombatUnit();
                        break;
                    // 도시 건물
                    // 건물이 있는 타일
                    case ObjectType.BUILDING:
                        Territory territory = currentSelect.GetComponent<Territory>();

                        if (territory != null)
                            UIPanelManager.instance.OpenPanel("CITY_PANEL");
                        break;
                    case ObjectType.TILE:
                        TerrainData terrain = currentSelect.GetComponent<TerrainData>();
                        // terrain.objectOn[0];
                        break;
                }
            }
        }
        #endregion

        // esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIPanelManager.instance.isEmpty())
            {
                // UIPanelManager.instance.OpenPanel("MENU_PANEL");
            }
            else
                UIPanelManager.instance.CloseCurrent();
        }
    }

    void Initialize()
    {
        fogLayer = LayerMask.GetMask("HexFog");

        InitPlyaers();

        UIManager.instance.Initialize();
    }

    void InitPlyaers()
    {
        for (int i = 0; i < initPlayerCount; ++i)
        {
            players.Add(Instantiate(playerPrefab).GetComponent<Player>());
            players[i].transform.position = startPoints[i].transform.position;
            players[i].playerId = i;
        }

        HexFogManager.instance.init(initPlayerCount);
    }

    // 현재 플레이어의 차례를 마치고 다음 플레이어 차례 시작
    public void TurnEnd()
    {
        // 연구를 선택하지 않은 경우
        // 기존 연구가 완료된 경우
        // TODO Turn Blocker에 표시
        if ((currentPlayer.info.ongoingTechnology == null) || (currentPlayer.info.ongoingTechnology.remainCost <= 0))
        {
            print("새로운 연구를 선택해주세요");
            UIPanelManager.instance.OpenPanel("TECHNOLOGY_PANEL");
            return;
        }

        // 현재 플레이어의 차례 종료
        players[_currentPlayerId].EndTurn();

        // 다음 플레이어 차례로 전환
        _currentPlayerId = (_currentPlayerId + 1) % players.Count;
        players[_currentPlayerId].StartTurn();
        //set hexfog
        HexFogManager.instance.FindOtherTargetList(_currentPlayerId);
        HexFogManager.instance.FindOtherUnitsBuildings(_currentPlayerId);
        HexFogManager.instance.prevInFov.Clear();
    }

    IEnumerator DelayedStartCoroutine()
    {
        yield return null;

        // Start InGame BGM
        SoundManager.instance.PlayBGM(SoundManager.BGM_TYPE.BGM_INGAME);

        // 첫번째 플레이어의 차례로 시작
        players[_currentPlayerId].StartTurn();
    }

    public void DestroyUnit(int playerId, Unit unit)
    {
        players[playerId].info.units.Remove(unit);
    }

    public bool isCurrentUnit()
    {
        return currentSelect != null && currentSelect.GetComponent<Unit>() != null;
    }

    void SelectNonCombatUnit()
    {
        NonCombatUnit unit = currentSelect.GetComponent<NonCombatUnit>();
        UIManager.instance.UpdateUnitData(unit);

        switch (unit.unitType)
        {
            // 건설자 - BuildFacilityCommand 그룹 활성화
            case InGameObjectId.BUILDER:
                UIPanelManager.instance.OpenPanel("BUILD_FACILITY_COMMAND_TAB");
                break;
            // 개척자 - 도시 건설 버튼 활성화
            case InGameObjectId.SETTLER:
                UIManager.instance.EnableCityBuild();
                break;
        }

        UIPanelManager.instance.OpenPanel("UNIT_PANEL");
    }

    void SelectCombatUnit()
    {
        CombatUnit unit = currentSelect.GetComponent<CombatUnit>();
        UIManager.instance.UpdateUnitData(unit);

        UIManager.instance.EnableFortification();

        UIPanelManager.instance.OpenPanel("UNIT_PANEL");
    }
}
