using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    public bool test;
    public float testDelay = 0.1f;
    public int testValue = 10;

    [Header("Common")]
    public bool useScience;
    public bool useCulture;
    public bool useFaith;
    public bool useGold;

    [Header("Player")]
    public GameObject playerPrefab;
    public List<Player> players;
    public int initPlayerCount;
    public int currentPlayerIdx;

    // Start is called before the first frame update
    void Start()
    {
        InitGame();

        StartCoroutine(TestCoroutine());

    }

    // Update is called once per frame
    void Update()
    {
        // for (int i = 0; i < players.Count; ++i)
        // {
        //     print(string.Format("{0} : {1}", i, players[i].isTurn));
        // }

        players[currentPlayerIdx].TurnUpdate();
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

    public void TurnEnd()
    {
        players[currentPlayerIdx].isTurn = false;
        currentPlayerIdx = (currentPlayerIdx + 1) % players.Count;


        // 다음 플레이어 차례로 전환
        players[currentPlayerIdx].StartTurn();

    }

    IEnumerator TestCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(testDelay);

            UIManager.instance.TestResourcesUpdate(testValue);
        }
    }

}
