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
    COMMAND_REST,
    COMMAND_MOVE,
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
    #endregion
}