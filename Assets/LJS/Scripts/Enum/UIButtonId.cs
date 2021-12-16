using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIButtonId
{
    NONE,

    #region SYSTEM
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
    COMMAND_BUILD_FARM,
    COMMAND_BUILD_MINE,
    #endregion

}