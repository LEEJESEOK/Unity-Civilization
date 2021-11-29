using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPanelManager : MonoBehaviour
{
    [SerializeField]
    Animator initiallyOpen;
    [SerializeField]
    List<UIPanel> panels;

    int m_OpenParameterId;
    Animator m_Open;
    GameObject m_PreviouslySelected;

    const string k_OpenTransitionName = "Open";
    const string k_CloseTransitionName = "Close";


    private void OnEnable()
    {
        m_OpenParameterId = Animator.StringToHash(k_OpenTransitionName);

        if (initiallyOpen == null)
            return;

        OpenPanel(initiallyOpen);
    }

    private void Start()
    {
        panels = new List<UIPanel>(GetComponentsInChildren<UIPanel>(true));
    }

    public void OpenPanel(Animator animator)
    {
        if (m_Open == animator)
            return;

        animator.gameObject.SetActive(true);
        GameObject newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

        animator.transform.SetAsLastSibling();

        CloseCurrent();

        m_PreviouslySelected = newPreviouslySelected;

        m_Open = animator;
        m_Open.SetBool(m_OpenParameterId, true);

        GameObject firstObject = FindFirstEnabledSelectable(animator.gameObject);
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
        if (m_Open == null)
            return;

        m_Open.SetBool(m_OpenParameterId, false);
        SetSelected(m_PreviouslySelected);
        StartCoroutine(DisablePanelDelayed(m_Open));
        m_Open = null;
    }

    IEnumerator DisablePanelDelayed(Animator animator)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            if (!animator.IsInTransition(0))
                closedStateReached = animator.GetCurrentAnimatorStateInfo(0).IsName(k_CloseTransitionName);

            wantToClose = !animator.GetBool(m_OpenParameterId);

            yield return new WaitForEndOfFrame();
        }

        if (wantToClose)
            animator.gameObject.SetActive(false);
    }

    void SetSelected(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
