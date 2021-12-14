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

    public List<GameObject> initialUnits;

    [Header("Start Resources")]
    public static int startGold = 10;


    // Start is called before the first frame update
    void Start()
    {
        testStartGold = startGold;

        InitGame();

        StartCoroutine(DelayedStartCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        players[_currentPlayerId].TurnUpdate();




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
            players[i].playerId = i;
        }
    }

    // 현재 플레이어의 차례를 마치고 다음 플레이어 차례 시작
    public void TurnEnd()
    {
        if (currentPlayer.info.ongoingTechnology == null)
        {
            print("연구를 선택해주세요");
            return;
        }
        if (currentPlayer.info.ongoingTechnology.remainCost <= 0)
        {
            print("새로운 연구를 선택해주세요");
            return;
        }

        // 현재 플레이어의 차례 종료
        players[_currentPlayerId].EndTurn();
        GameManager.instance.players[_currentPlayerId].isTurn = false;


        // 다음 플레이어 차례로 전환
        _currentPlayerId = (_currentPlayerId + 1) % players.Count;
        players[_currentPlayerId].StartTurn();
    }

    IEnumerator DelayedStartCoroutine()
    {
        yield return null;
        // 첫번째 플레이어의 차례로 시작
        players[_currentPlayerId].StartTurn();
    }
}
