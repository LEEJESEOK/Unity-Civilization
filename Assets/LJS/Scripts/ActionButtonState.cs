using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState
{
    NEXT_TURN,
    NEXT_UNIT,
    CHOOSE_TECH_RESEARCH,
}

public class ActionButtonState : MonoBehaviour
{
    public ActionState state;
}
