using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour
{
    public string panelName;

    private void OnEnable()
    {
        UIManager.ResizeLayoutGroup(gameObject);
    }

    private void OnDisable()
    {
        if (UIManager.instance == null)
            return;

        switch (panelName)
        {
            case "UNIT_PANEL":
                // 이동 영역 초기화
                MapManager.instance.InitMoveArea();

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
