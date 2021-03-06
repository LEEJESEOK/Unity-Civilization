using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    public bool test;
    public int initialScience;
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
    [SerializeField]
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
    bool onClick;
    Vector3 mouseDownPosition;

    LayerMask fogLayer;

    bool isInit = false;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit == false)
            return;

        #region object select
        // TODO MouseDown 이후에 mouse좌표가 바뀌면 드래그
        if ((!UIManager.IsPointerOverUIObject()) && Input.GetMouseButtonDown(0))
        {
            onClick = true;
            mouseDownPosition = Input.mousePosition;
        }
        // 드래그, Drag
        if ((!UIManager.IsPointerOverUIObject()) && Input.GetMouseButton(0))
        {
            if (mouseDownPosition != Input.mousePosition)
            {
                onClick = false;
            }
        }
        // 클릭, Click
        if ((!UIManager.IsPointerOverUIObject()) && Input.GetMouseButtonUp(0) && onClick)
        {
            onClick = false;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            GameObjectInfo gameObjectInfo = null;
            if (Physics.Raycast(ray, out raycastHit, float.MaxValue, ~fogLayer))
            {
                currentSelect = raycastHit.transform.gameObject;
                gameObjectInfo = currentSelect.GetComponent<GameObjectInfo>();
            }

            // CurrentPlayer의 오브젝트만 조작
            // 자신의 오브젝트
            if ((gameObjectInfo != null))
            {
                UIManager.ClearUI();
                MapManager.instance.InitMoveArea();
                UIPanelManager.instance.ClosePanel("UNIT_PANEL");
                UIPanelManager.instance.ClosePanel("CITY_PANEL");

                SelectGameObject(currentSelect);
            }

        }

        if (currentSelect == null)
        {
            MapManager.instance.unitSelecting = false;
            UIManager.ClearUI();
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
            {
                UIPanelManager.instance.CloseCurrent();
            }
        }
    }

    IEnumerator StartGame()
    {
        yield return Initialize();
        isInit = true;

        // Start InGame BGM

        // 첫번째 플레이어의 차례로 시작
        yield return players[_currentPlayerId].StartTurn();
    }

    IEnumerator Initialize()
    {
        fogLayer = LayerMask.GetMask("HexFog");

        yield return InitPlyaers();

        yield return UIManager.instance.Initialize();

    }

    IEnumerator InitPlyaers()
    {
        for (int i = 0; i < initPlayerCount; ++i)
        {
            players.Add(Instantiate(playerPrefab).GetComponent<Player>());
            players[i].transform.position = startPoints[i].transform.position;
            players[i].playerId = i;
        }
        yield return HexFogManager.instance.Initialize(initPlayerCount);

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
            UIManager.instance.UpdateTechnologyPanel();
            UIPanelManager.instance.OpenPanel("TECHNOLOGY_PANEL");
            return;
        }

        // 현재 플레이어의 차례 종료
        players[_currentPlayerId].EndTurn();
        UIManager.ClearUI();


        // 다음 플레이어 차례로 전환
        _currentPlayerId = (_currentPlayerId + 1) % players.Count;
        StartCoroutine(players[_currentPlayerId].StartTurn());
    }

    public void DestroyUnit(int playerId, Unit unit)
    {
        players[playerId].info.units.Remove(unit);
    }

    public bool IsCurrentUnit()
    {
        return currentSelect != null && currentSelect.GetComponent<Unit>() != null;
    }

    public Unit GetCurrentUnit()
    {
        if (IsCurrentUnit())
            return currentSelect.GetComponent<Unit>();
        else
            return null;
    }

    public bool IsCurrentCity()
    {
        return currentSelect != null && currentSelect.GetComponent<Unit>() != null;
    }

    public Building GetCurrentCity()
    {
        if (IsCurrentCity())
            return currentSelect.GetComponent<Building>();
        else
            return null;
    }

    // 첫번째 오브젝트를 마지막 순서로
    void SetFirstToLastSibling(ref List<GameObject> list)
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 1; i < list.Count; ++i)
        {
            temp.Add(list[i]);
        }
        temp.Add(list[0]);
        list = temp;
    }

    void SelectGameObject(GameObject go)
    {
        currentSelect = go;
        GameObjectInfo objectInfo = go.GetComponent<GameObjectInfo>();

        // 타일(-1)은 포함
        // 다른 플레이어 오브젝트 제외
        // if ((objectInfo.playerId != -1) && (objectInfo.playerId != currentPlayerId))
        //     return;

        ObjectType type = objectInfo.type;

        switch (type)
        {
            // 도시 건물
            // 건물이 있는 타일
            case ObjectType.BUILDING:
                Building building = currentSelect.transform.parent.GetComponentInChildren<Building>();
                if (building != null)
                {
                    Territory territory = building.myTilePos.GetComponent<Territory>();
                    UIManager.instance.UpdateCityPanelData(territory);
                    UIPanelManager.instance.OpenPanel("CITY_PANEL");
                }
                break;
            case ObjectType.TILE:
                TerrainData terrainData = currentSelect.GetComponent<TerrainData>();
                if (terrainData.objectOn.Count > 0)
                {
                    // 타일에 있는 오브젝트 선택
                    if ((terrainData.objectOn[0].GetComponent<GameObjectInfo>().playerId == currentPlayerId))
                    {
                        SelectGameObject(terrainData.objectOn[0]);
                        SetFirstToLastSibling(ref terrainData.objectOn);
                    }
                }
                break;
            case ObjectType.NON_COMBAT_UNIT:
                currentSelectType = ObjectType.NON_COMBAT_UNIT;
                SelectNonCombatUnit();
                break;
            case ObjectType.COMBAT_UNIT:
                currentSelectType = ObjectType.COMBAT_UNIT;
                SelectCombatUnit();
                break;
        }
    }

    void SelectNonCombatUnit()
    {
        NonCombatUnit unit = currentSelect.GetComponent<NonCombatUnit>();
        if (unit.playerId != currentPlayerId)
        {
            currentSelect = null;
            return;
        }

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
        if (unit.playerId != currentPlayerId)
        {
            currentSelect = null;
            return;
        }

        UIManager.instance.UpdateUnitData(unit);

        UIManager.instance.EnableFortification();

        UIPanelManager.instance.OpenPanel("UNIT_PANEL");
    }
}
