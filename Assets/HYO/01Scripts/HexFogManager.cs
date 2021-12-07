using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexFogManager : Singleton<HexFogManager>
{
    [SerializeField]
    public List<Hideable> findTargetList;

    public FieldOfView[] fieldOfViews;

    public List<Hideable> allHideables = new List<Hideable>();

    private void Start()
    {
        fieldOfViews = FindObjectsOfType<FieldOfView>();
        findTargetList = new List<Hideable>();
    }

    private void LateUpdate()
    {
        MergeUnitInfo();
        HexFogAdd();
    }

    void MergeUnitInfo()
    {
        allHideables.Clear();

        for(int i =0; i < fieldOfViews.Length; i++)
        {
            for(int j =0; j<fieldOfViews[i].hideables.Count; j++)
            {
                if(allHideables.Contains(fieldOfViews[i].hideables[j]) == false)
                {
                    if(findTargetList.Contains(fieldOfViews[i].hideables[j]) == false)
                    {
                        findTargetList.Add(fieldOfViews[i].hideables[j]);
                    }
                    allHideables.Add(fieldOfViews[i].hideables[j]);
                }
            }
        }
    }


    void HexFogAdd()
    {
        for(int i =0; i < findTargetList.Count; i++)
        {
            if (allHideables.Contains(findTargetList[i]) == false)
            {
                findTargetList[i].OnFOVLeaveShow();
                findTargetList[i].OnFOVTransparency();
            }
            else
            {
                findTargetList[i].OnFOVEnterHide();
            }
        }
    }
    
}
