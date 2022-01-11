using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings : MonoBehaviour
{
    public int playerId;
    private void Awake()
    {
        playerId = GameManager.instance.currentPlayerId;
    }
}
