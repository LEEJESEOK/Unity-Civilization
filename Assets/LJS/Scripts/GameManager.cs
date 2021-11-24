using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Test")]
    public bool test;
    public float testDelay = 0.1f;
    public int testValue = 10;
    public int testValueChange = 1;

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

    IEnumerator testCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        InitGame();

        testCoroutine = TestCoroutine();
        StartCoroutine(testCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
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
        // 현재 플레이어의 차례 종료
        players[currentPlayerIdx].isTurn = false;

        // 다음 플레이어 차례로 전환
        currentPlayerIdx = (currentPlayerIdx + 1) % players.Count;
        players[currentPlayerIdx].StartTurn();
    }

    IEnumerator TestCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(testDelay);

            if (test)
            {
                testValue += testValueChange;
                UIManager.instance.UpdateResource(testValue, 0, 0, 0, testValue, testValueChange);
            }
        }
    }
}
