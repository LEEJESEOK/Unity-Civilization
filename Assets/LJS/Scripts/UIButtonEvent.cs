using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEvent : ButtonEvent<UIButtonId>
{
    public GameObject actionButton;

    // Start is called before the first frame update
    void Start()
    {
        listenerList = new List<ButtonListener<UIButtonId>>(GetComponentsInChildren<ButtonListener<UIButtonId>>());

        for (int i = 0; i < listenerList.Count; ++i)
            listenerList[i].AddClickCallback(ClickEvent);
    }

    public override void ClickEvent(UIButtonId eventType)
    {
        switch (eventType)
        {
            case UIButtonId.ACTION:
                ActionState actionState = actionButton.GetComponent<ActionButtonState>().state;
                print(eventType + ", " + actionState);

                switch (actionState)
                {
                    case ActionState.NEXT_TURN:
                        GameManager.instance.TurnEnd();
                        break;
                    case ActionState.CHOOSE_TECH_RESEARCH:
                        break;
                    case ActionState.NEXT_UNIT:
                        break;
                }
                break;

            case UIButtonId.MENU_TECH:
                print(eventType);
                break;
        }
    }
}
