using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public string panelName;

    protected void OnEnable()
    {
        switch (panelName)
        {
            case "UNIT_PANEL":
                // 버튼 상태 초기화
                // 개척자 -> 도시 건설 버튼 표시
                // 전투 유닛 -> 요새화 버튼 표시
                break;
        }

        UIManager.ResizeLayoutGroup(gameObject);
    }


}
