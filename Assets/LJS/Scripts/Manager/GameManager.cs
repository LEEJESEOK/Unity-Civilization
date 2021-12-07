using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    public bool test;

    [Header("Common")]
    public bool useScience;
    public bool useCulture;
    public bool useFaith;
    public bool useGold;

    [Header("Player")]
    public GameObject playerPrefab;
    public List<Player> players;
    public int initPlayerCount;
    public int currentPlayerId;

    [Header("Start Resources")]
    public int startGold = 10;


    // Start is called before the first frame update
    void Start()
    {
        InitGame();

        // 첫번째 플레이어의 차례로 시작
        players[currentPlayerId].StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        players[currentPlayerId].TurnUpdate();
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
        // 현재 플레이어의 차례 종료
        players[currentPlayerId].EndTurn();
        GameManager.instance.players[currentPlayerId].isTurn = false;


        // 다음 플레이어 차례로 전환
        currentPlayerId = (currentPlayerId + 1) % players.Count;
        players[currentPlayerId].StartTurn();
    }
}
