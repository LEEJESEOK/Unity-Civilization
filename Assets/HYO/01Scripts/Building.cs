using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int playerId;
    public int castleHp = 300;
    private void Awake()
    {
        playerId = GameManager.instance.currentPlayerId;
    }
}
