using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : CombatUnit
{
    private void Awake()
    {
        playerId = GameManager.instance.currentPlayerId;
    }
}
