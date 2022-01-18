using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : CombatUnit
{
    public int playerId;

    private void Awake()
    {
        playerId = GameManager.instance.currentPlayerId;
    }
}
