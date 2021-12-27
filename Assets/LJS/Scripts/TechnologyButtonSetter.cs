using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TechnologyButtonSetter : MonoBehaviour
{
    public TextMeshProUGUI nameTMP;
    public Image image;
    public TextMeshProUGUI descTMP;
    // TODO unlockObject

    public void SetTechnologyButton(string name, Sprite sprite, string desc, List<InGameObjectId> unlockObjectId)
    {
        nameTMP.text = name;
        image.sprite = sprite;
        // TODO unlockObject
        descTMP.text = desc;
    }

}
