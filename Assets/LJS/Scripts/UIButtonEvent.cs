using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonEvent : ButtonEvent<UIButtonId>
{
    GameObject actionButton;

    // Start is called before the first frame update
    void Start()
    {
        listenerList = new List<ButtonListener<UIButtonId>>(GetComponentsInChildren<ButtonListener<UIButtonId>>(true));
        for (int i = 0; i < listenerList.Count; ++i)
            listenerList[i].AddClickCallback(ClickEvent);

        actionButton = GetComponentInChildren<ActionButtonState>().gameObject;
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
                        UIPanelManager.instance.OpenPanel("TECHNOLOGY_PANEL");
                        break;
                    case ActionState.NEXT_UNIT:
                        // Open UnitPanel
                        break;
                }
                break;

            case UIButtonId.CLOSE_CURRENT_PANEL:
                UIPanelManager.instance.CloseCurrent();
                break;

            case UIButtonId.MENU_TECH:
                UIManager.instance.UpdateTechnologyPanel();
                UIPanelManager.instance.OpenPanel("TECHNOLOGY_PANEL");
                break;
            // case UIButtonId.TECH:
            //     break;

            case UIButtonId.MENU:
                UIPanelManager.instance.OpenPanel("MENU_PANEL");
                break;
            case UIButtonId.HELP:
                UIPanelManager.instance.OpenPanel("HELP_PANEL");
                break;

            case UIButtonId.COMMAND_REST:
                break;
            case UIButtonId.COMMAND_MOVE:
                MapManager.instance.onClickMove();
                break;

            // for combat unit
            case UIButtonId.COMMAND_FORTIFICATION:
                break;

            // for settler
            case UIButtonId.COMMAND_BUILD_CITY:
                HYO_ConstructManager.instance.CreateTerritory();
                break;

            // for builder
            case UIButtonId.COMMAND_BUILD_FACILITY:
                break;
            // case UIButtonId.COMMAND_BUILD_FARM:
            //     HYO_ConstructManager.instance.CreateFacility(Facility.FARM);
            //     break;
            // case UIButtonId.COMMAND_BUILD_MINE:
            //     HYO_ConstructManager.instance.CreateFacility(Facility.MINE);
            //     break;
            // case UIButtonId.COMMAND_BUILD_CAMPUS:
            //         HYO_ConstructManager.instance.SetDistrictInfo(District.CAMPUS);
            //     break;
            // case UIButtonId.COMMAND_BUILD_COMMERCIALHUB:
            //     HYO_ConstructManager.instance.SetDistrictInfo(District.COMMERCAILHUB);
            //     break;
            // case UIButtonId.COMMAND_BUILD_INDUSTRIALZONE:
            //     HYO_ConstructManager.instance.SetDistrictInfo(District.INDUSTRIALZONE);
            //     break;
            case UIButtonId.BUILD_DISTRICT:
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
        // 이미 완료한 연구인지 검사
        if (selectedTech.isResearched)
        {
            print(string.Format("이미 완료한 연구입니다.({0})", selectedTech.korean));
            return;
        }
        // 선행 연구를 완료했는지 검사
        for (int i = 0; i < selectedTech.requireTechId.Count; ++i)
        {
            Technology tech = GameManager.instance.currentPlayer.info.technologies.Find(x => x.id == selectedTech.requireTechId[i]);
            if (tech.isResearched == false)
            {
                print(string.Format("{0}을/를 연구하지 않았습니다.", tech.korean));
                return;
            }
        }

        GameManager.instance.currentPlayer.info.ongoingTechnology = selectedTech;
        UIManager.instance.UpdateSelectedTechnology(selectedTech);
        print(string.Format("{0} 선택", selectedTech.korean));
    }

    public void BuildFacility(InGameObjectId objectId)
    {
        HYO_ConstructManager.instance.CreateFacility(objectId);
    }

    public void BuildDistrict(InGameObjectId objectId)
    {
        // HYO_ConstructManager.instance.CreateDistrict(objectId, );
    }

    public void ProductUnit(InGameObjectId objectId)
    {

    }

    public void BuyUnit(InGameObjectId objectId)
    {

    }
}
