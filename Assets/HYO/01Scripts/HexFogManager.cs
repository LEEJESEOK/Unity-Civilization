using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFogManager : Singleton<HexFogManager>
{
    [SerializeField]
    public List<List<Hideable>> findTargetList;

    public List<List<FieldOfView>> fieldOfViews;

    public List<List<Hideable>> allHideables;
    public int currentPlayerId;

    private void Start()
    {
        fieldOfViews = new List<List<FieldOfView>>();
        findTargetList = new List<List<Hideable>>();
        allHideables = new List<List<Hideable>>();
    }

    public void init(int playerCount)
    {
        for (int i = 0; i < playerCount; ++i)
        {
            findTargetList.Add(new List<Hideable>());
            fieldOfViews.Add(new List<FieldOfView>());
            allHideables.Add(new List<Hideable>());

        }

    }

    private void LateUpdate()
    {
        currentPlayerId = GameManager.instance.currentPlayerId;

        MergeUnitInfo(currentPlayerId);
        HexFogAdd(currentPlayerId);
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
    }

    public void RemoveFOV(FieldOfView fov)
    {

    }
    
}
