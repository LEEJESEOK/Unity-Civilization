using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : CombatUnit
{
    private void Awake()
    {
        playerId = GameManager.instance.currentPlayerId;

    }
    private void OnDestroy()
    {
        // TODO data 제거
        //Destroy(myTilePos.GetComponent<Territory>());
        //TerrainData terrainData = myTilePos.GetComponent<TerrainData>();
        //terrainData.myCenter = null;
        //terrainData.objectOn.Remove(gameObject);
    }
}
