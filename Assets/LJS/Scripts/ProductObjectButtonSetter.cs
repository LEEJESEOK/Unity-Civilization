using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductObjectButtonSetter : MonoBehaviour
{
    public Image objectImage;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI costTMP;


    public void SetProductObjectButton(Sprite sprite, string name, int productCost)
    {
        objectImage.sprite = sprite;
        nameTMP.text = name;
        costTMP.text = productCost.ToString();
    }
}
