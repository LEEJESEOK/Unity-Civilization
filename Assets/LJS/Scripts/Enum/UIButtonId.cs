using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIButtonId
{
    NONE,

    #region SYSTEM
    CLOSE_CURRENT_PANEL,
    ACTION,
    NOTICE,

    MENU_TECH,
    MENU_CIVIC,
    MENU_GREATPEOPLE,

    MENU,
    HELP,

    TECH,
    #endregion

    #region UNIT COMMAND
    // 비활성화
    COMMAND_REST,
    // 이동
    COMMAND_MOVE,
    // 요새화
    COMMAND_FORTIFICATION, 

    // settler
    COMMAND_BUILD_CITY,
    COMMAND_DESTROY,
    // builder
    COMMAND_BUILD_FACILITY,
    #endregion

    #region CITY PRODUCT
    // district
    BUILD_DISTRICT,
    PRODUCT_UNIT,
    BUY_UNIT,
    #endregion
}