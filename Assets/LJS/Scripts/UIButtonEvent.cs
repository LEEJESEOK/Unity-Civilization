using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEvent : ButtonEvent<UIButtonId>
{
    public GameObject actionButton;

    UIPanelManager uIPanelManager;

    // Start is called before the first frame update
    void Start()
    {
        listenerList = new List<ButtonListener<UIButtonId>>(GetComponentsInChildren<ButtonListener<UIButtonId>>());
        for (int i = 0; i < listenerList.Count; ++i)
            listenerList[i].AddClickCallback(ClickEvent);

        uIPanelManager = GetComponent<UIPanelManager>();
    }

    public override void ClickEvent(UIButtonId eventType)
    {
        switch (eventType)
        {
            case UIButtonId.ACTION:
                ActionState actionState = actionButton.GetComponent<ActionButtonState>().state;

                switch (actionState)
                {
                    case ActionState.NEXT_TURN:
                        GameManager.instance.TurnEnd();
                        break;
                    case ActionState.CHOOSE_TECH_RESEARCH:
                        // Open TechPanel
                        break;
                    case ActionState.NEXT_UNIT:
                        break;
                }
                break;

            case UIButtonId.MENU_TECH:
                UIPanelManager.instance.OpenPanel("TECHNOLOGY_PANEL");
                break;

            case UIButtonId.TECH:
                print("select tech button");
                break;
        }
    }

    public void AddUIListener(ButtonListener<UIButtonId> listener)
    {
        listener.AddClickCallback(ClickEvent);
        listenerList.Add(listener);
    }

    public void SelectOngoingTechnology(TechnologyId technologyId)
    {

        Technology selectedTech = GameManager.instance.currentPlayer.info.technologies.Find(x => x.id == technologyId);
        GameManager.instance.currentPlayer.info.ongoingTechnology = selectedTech;
        print(selectedTech);
    }
}
