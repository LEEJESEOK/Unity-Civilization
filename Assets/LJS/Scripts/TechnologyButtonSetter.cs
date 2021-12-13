using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TechnologyButtonSetter : MonoBehaviour
{
    public TextMeshProUGUI nameTMP;
    public Image image;

    public void SetTechnologyButton(string name, Sprite sprite, string desc)
    {
        nameTMP.text = name;
        image.sprite = sprite;
    }

}
