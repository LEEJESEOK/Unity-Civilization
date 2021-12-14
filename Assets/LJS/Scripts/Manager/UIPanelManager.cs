using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIPanelManager : Singleton<UIPanelManager>
{
    [SerializeField]
    UIPanel initiallyOpen;
    [SerializeField]
    List<UIPanel> panels;

    // int m_OpenParameterId;
    // Animator m_Open;
    UIPanel _currentPanel;
    public UIPanel currentPanel { get => _currentPanel; }
    GameObject m_PreviouslySelected;


    private void OnEnable()
    {
        if (initiallyOpen == null)
            return;

        OpenPanel(initiallyOpen);
    }

    private void Start()
    {
        panels = new List<UIPanel>(GetComponentsInChildren<UIPanel>(true));

        // Close All Panel
        for (int i = 0; i < panels.Count; ++i)
        {
            panels[i].gameObject.SetActive(false);
        }
    }

    public void OpenPanel(UIPanel panel)
    {
        if (_currentPanel == panel)
            return;

        panel.gameObject.SetActive(true);
        GameObject newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

        panel.transform.SetAsLastSibling();

        CloseCurrent();

        m_PreviouslySelected = newPreviouslySelected;

        _currentPanel = panel;

        GameObject firstObject = FindFirstEnabledSelectable(panel.gameObject);
        SetSelected(firstObject);
    }

    public void OpenPanel(string panelName)
    {
        OpenPanel(panels.Find(panel => panel.panelName == panelName));
    }

    static GameObject FindFirstEnabledSelectable(GameObject gameObject)
    {
        GameObject findObject = null;
        Selectable[] selectables = gameObject.GetComponentsInChildren<Selectable>(true);
        for (int i = 0; i < selectables.Length; ++i)
        {
            if (selectables[i].IsActive() && selectables[i].IsInteractable())
            {
                findObject = selectables[i].gameObject;
                break;
            }
        }

        return findObject;
    }

    public void CloseCurrent()
    {
        if (_currentPanel == null)
            return;

        SetSelected(m_PreviouslySelected);
        _currentPanel.gameObject.SetActive(false);
        _currentPanel = null;
    }

    void SetSelected(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
