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

    public float cameraMoveSpeed;
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
    public GameObject currentSelect;


    private void Awake()
    {
        // #region test
        // if (Application.platform != RuntimePlatform.WindowsEditor)
        //     test = false;
        // #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        InitGame();

        StartCoroutine(DelayedStartCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        #region test
        if (!UIManager.IsPointerOverUIObject() & Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, float.MaxValue))
            {
                currentSelect = hit.transform.gameObject;

                GameObjectType gameObjectType = currentSelect.GetComponent<GameObjectType>();
                if (gameObjectType != null)
                {
                    TypeIdBase type = gameObjectType.type;
                    switch (type)
                    {
                        case TypeIdBase.UNIT:
                            Unit unit = currentSelect.GetComponent<Unit>();
                            UIPanelManager.instance.OpenPanel("UNIT_PANEL");
                            break;
                        // 도시 건물
                        // 건물이 있는 타일
                        case TypeIdBase.DISTRICT:
                        case TypeIdBase.TILE:
                            Territory territory = currentSelect.GetComponent<Territory>();

                            UIPanelManager.instance.OpenPanel("CITY_PANEL");
                            break;
                    }
                }
            }
        }
        #endregion

        // esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIPanelManager.instance.CloseCurrent();
        }
    }

    void InitGame()
    {
        InitPlyaers();

        UIManager.instance.InitUI();
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
}
