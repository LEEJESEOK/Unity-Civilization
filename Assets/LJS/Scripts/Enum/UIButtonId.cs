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
    COMMAND_ATTACK,
    COMMAND_DESTROY,

    // settler
    COMMAND_BUILD_CITY,

    // builder
    COMMAND_BUILD_FARM,
    COMMAND_BUILD_MINE,

    //district
    COMMAND_BUILD_CAMPUS,
    COMMAND_BUILD_COMMERCIALHUB,
    COMMAND_BUILD_INDUSTRIALZONE

    #endregion

}