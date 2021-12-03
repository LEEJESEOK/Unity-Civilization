using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UIPanelManager : MonoBehaviour
{
    // [SerializeField]
    // Animator initiallyOpen;
    [SerializeField]
    UIPanel initiallyOpen;
    [SerializeField]
    List<UIPanel> panels;

    // int m_OpenParameterId;
    // Animator m_Open;
    UIPanel currentPanel;
    GameObject m_PreviouslySelected;

    // const string k_OpenTransitionName = "Open";
    // const string k_ClosedTransitionName = "Closed";


    private void OnEnable()
    {
        // m_OpenParameterId = Animator.StringToHash(k_OpenTransitionName);

        if (initiallyOpen == null)
            return;

        OpenPanel(initiallyOpen);
    }

    private void Start()
    {
        panels = new List<UIPanel>(GetComponentsInChildren<UIPanel>(true));
    }

    public void OpenPanel(UIPanel panel)
    {
        if (currentPanel == panel)
            return;

        panel.gameObject.SetActive(true);
        GameObject newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

        panel.transform.SetAsLastSibling();

        CloseCurrent();

        m_PreviouslySelected = newPreviouslySelected;

        currentPanel = panel;

        GameObject firstObject = FindFirstEnabledSelectable(panel.gameObject);
        SetSelected(firstObject);
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
        if (currentPanel == null)
            return;

        SetSelected(m_PreviouslySelected);
        currentPanel.gameObject.SetActive(false);
        currentPanel = null;
    }

    void SetSelected(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
