using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    public string panelName;

    private void OnEnable()
    {
        UIManager.ResizeLayoutGroup(gameObject);
    }
}
