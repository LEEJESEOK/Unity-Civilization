using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductObjectButtonSetter : MonoBehaviour
{
    public TextMeshProUGUI nameTMP;
    public Image objectImage;
    public TextMeshProUGUI costTMP;


    public void SetProductObjectButton(string name, Sprite sprite, int productCost)
    {
        objectImage.sprite = sprite;
        nameTMP.text = name;
        costTMP.text = productCost.ToString();
    }
}
