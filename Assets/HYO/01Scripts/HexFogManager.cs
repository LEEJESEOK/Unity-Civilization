using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFogManager : Singleton<HexFogManager>
{
    [SerializeField]
    //HexFog
    public List<List<Hideable>> findTargetList;
    public List<List<FieldOfView>> fieldOfViews;
    public List<List<Hideable>> allHideables;
    public List<Hideable> otherTargetList;

    //Unit&Buildings
    public List<List<Unit>> units;
    public List<List<GameObject>> buildings;
    public List<GameObject> otherUnitsBuildings;

    public List<GameObject> inFov; 
    public List<GameObject> prevInFov;

    public int currentPlayerId;

    private void Start()
    {
        fieldOfViews = new List<List<FieldOfView>>();
        findTargetList = new List<List<Hideable>>();
        allHideables = new List<List<Hideable>>();
        otherTargetList = new List<Hideable>();

        units = new List<List<Unit>>();
        buildings = new List<List<GameObject>>();
        otherUnitsBuildings = new List<GameObject>();

        inFov = new List<GameObject>();
        prevInFov = new List<GameObject>();

        //FindOtherUnitsBuildings(currentPlayerId);
    }

    public void init(int playerCount)
    {
        for (int i = 0; i < playerCount; ++i)
        {
            findTargetList.Add(new List<Hideable>());
            fieldOfViews.Add(new List<FieldOfView>());
            allHideables.Add(new List<Hideable>());

            units.Add(GameManager.instance.players[i].info.units);
            buildings.Add(new List<GameObject>());
        }
        
    }

    private void LateUpdate()
    {
        currentPlayerId = GameManager.instance.currentPlayerId;

        MergeUnitInfo(currentPlayerId);
        HexFogAdd(currentPlayerId);


        for (int i = 0; i < prevInFov.Count; i++)
        {
           prevInFov[i].SetActive(false);
        }
        prevInFov.Clear();

        for (int i = 0; i < inFov.Count; i++)
        {
            inFov[i].SetActive(true);
            prevInFov.Add(inFov[i]);
        }
        inFov.Clear();
    }

    void MergeUnitInfo(int id)
    {
        allHideables[id].Clear();

        for(int i =0; i < fieldOfViews[id].Count; i++)
        {
            for(int j =0; j<fieldOfViews[id][i].hideables.Count; j++)
            {
                if(allHideables[id].Contains(fieldOfViews[id][i].hideables[j]) == false)
                {
                    if(findTargetList[id].Contains(fieldOfViews[id][i].hideables[j]) == false)
                    {
                        findTargetList[id].Add(fieldOfViews[id][i].hideables[j]);
                    }
                    allHideables[id].Add(fieldOfViews[id][i].hideables[j]);
                }
            }
        }
    }


    public void FindOtherTargetList(int id)
    {
        //set hexfog
        for(int i=0; i< findTargetList.Count; i++)
        {
            if (findTargetList[i] != findTargetList[id])
            {
                for(int j = 0; j < findTargetList[i].Count; j++)
                {
                    otherTargetList.Add(findTargetList[i][j]);
                }
            }
        }
    }
    public void FindOtherUnitsBuildings(int id)
    {
        //set unit & buildings
        for (int b = 0; b < units.Count; b++)
        {
            if (b != id)
            {
                for (int c = 0; c < units[b].Count; c++)
                {
                    otherUnitsBuildings.Add(units[b][c].gameObject);
                }
                for (int d = 0; d < buildings[b].Count; d++)
                {
                    otherUnitsBuildings.Add(buildings[b][d]);
                }
            }
            else
            {
                for (int f = 0; f < units[b].Count; f++)
                {
                    units[b][f].gameObject.SetActive(true);
                }
                for (int g = 0; g < buildings[b].Count; g++)
                {
                    buildings[b][g].SetActive(true);
                }
            }
        }
    }

    void HexFogAdd(int id)
    {
        for(int i =0; i < findTargetList[id].Count; i++)
        {
            if (allHideables[id].Contains(findTargetList[id][i]) == false)
            {
                findTargetList[id][i].OnFOVLeaveShow();
                findTargetList[id][i].OnFOVTransparency();
            }
            else
            {
                findTargetList[id][i].OnFOVEnterHide();
            }
        }

        //set hexfog
        for(int a = 0; a < otherTargetList.Count; a++)
        {
            if(findTargetList[id].Contains(otherTargetList[a]) == false)
            {
                otherTargetList[a].OnFOVLeaveShow();
            }
        }

        otherTargetList.Clear();


        //set unit &building
        for (int e = 0; e < otherUnitsBuildings.Count; e++)
        {
            otherUnitsBuildings[e].GetComponent<Hideable>().OnFOVHideUnits();
        }

        otherUnitsBuildings.Clear();
    }

    public void RemoveFOV(FieldOfView fov)
    {

    }

}
