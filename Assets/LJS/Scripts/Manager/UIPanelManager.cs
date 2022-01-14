using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIPanelManager : Singleton<UIPanelManager>
{
    [SerializeField]
    List<UIPanel> panels;

    // int m_OpenParameterId;
    // Animator m_Open;
    // UIPanel _currentPanel;
    public List<UIPanel> openedPanel = new List<UIPanel>();
    // public UIPanel currentPanel { get => _currentPanel; }
    public UIPanel currentPanel
    {
        get
        {
            if (openedPanel.Count > 0) return openedPanel[openedPanel.Count - 1];
            else return null;
        }
    }
    GameObject m_PreviouslySelected;


    private void Start()
    {
        panels = new List<UIPanel>(GetComponentsInChildren<UIPanel>(true));

        // Close All Panel
        for (int i = 0; i < panels.Count; ++i)
        {
            panels[i].gameObject.SetActive(false);
        }
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

    void SetSelected(GameObject gameObject)
    {
        if (gameObject == null)
            return;

        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OpenPanel(UIPanel panel)
    {
        if (panel == null)
            return;

        if (openedPanel.Contains(panel))
        {
            SetSelected(panel.gameObject);
            return;
        }

        panel.gameObject.SetActive(true);
        panel.transform.SetAsLastSibling();

        GameObject firstObject = FindFirstEnabledSelectable(panel.gameObject);
        SetSelected(firstObject);

        openedPanel.Add(panel);
    }

    public void OpenPanel(string panelName)
    {
        OpenPanel(panels.Find(x => x.panelName == panelName));
    }

    public void ClosePanel(UIPanel panel)
    {
        if (panel == null)
            return;

        panel.gameObject.SetActive(false);
        openedPanel.Remove(panel);
        if (currentPanel != null)
            SetSelected(currentPanel.gameObject);
    }

    public void ClosePanel(string panelName)
    {
        ClosePanel(openedPanel.Find(x => x.panelName == panelName));
    }

    public void CloseCurrent()
    {
        ClosePanel(currentPanel);
    }

    public bool isEmpty()
    {
        return openedPanel.Count == 0;
    }
}
