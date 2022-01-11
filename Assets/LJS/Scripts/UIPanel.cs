using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public string panelName;

    private void OnEnable()
    {
        UIManager.ResizeLayoutGroup(gameObject);
    }

    private void OnDisable()
    {
        switch (panelName)
        {
            case "UNIT_PANEL":
                // 버튼 상태 초기화
                // TODO 도시 건설 버튼 비활성화
                UIManager.instance.DisableCityBuild();

                // 시설 건설 버튼 비활성화
                UIPanelManager.instance.ClosePanel("BUILD_FACILITY_COMMAND_TAB");

                // TODO 요새화 버튼 비활성화
                UIManager.instance.DisableFortificcation();
                break;
        }
    }
}
