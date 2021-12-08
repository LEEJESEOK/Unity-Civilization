using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public string panelName;

    private void OnEnable()
    {
        switch (panelName)
        {
            case "TECHNOLOGY_PANEL":
                // 스크롤 위치 초기화
                GetComponentInChildren<ScrollRect>();
                break;
        }
    }
}
