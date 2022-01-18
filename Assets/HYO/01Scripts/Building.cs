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
        myTilePos.GetComponent<TerrainData>().objectOn.Remove(gameObject);
        List<TerrainData> datas = myTilePos.GetComponent<Territory>().data;
        for (int i = 0; i < datas.Count; i++)
        {
            datas[i].GetComponent<Renderer>().material.shader = Shader.Find("Standard");
            datas[i].myCenter = null;
        }

        Destroy(myTilePos.GetComponent<Territory>());



        if (HexFogManager.instance != null)
        {
            while (HexFogManager.instance.prevInFov.Find(x => x == gameObject))
                HexFogManager.instance.prevInFov.Remove(gameObject);
            while (HexFogManager.instance.inFov.Find(x => x == gameObject))
                HexFogManager.instance.inFov.Remove(gameObject);
        }
    }
}
