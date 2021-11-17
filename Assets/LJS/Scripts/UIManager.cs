using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    bool test;

    [Header("Common")]
    public bool useScience, useCulture, useFaith, useGold;

    [Header("Resources")]
    public GameObject scienceGroup;
    public GameObject cultureGroup;
    public GameObject faithGroup;
    public GameObject goldGroup;
    public TextMeshProUGUI scienceTMP;
    public TextMeshProUGUI cultureTMP;
    public TextMeshProUGUI faithTMP;
    public TextMeshProUGUI faithChangeTMP;
    public TextMeshProUGUI goldTMP;
    public TextMeshProUGUI goldChangeTMP;

    [Header("Technology")]
    public GameObject technologyPanel;
    public GameObject technologyPanelContent;
    public GameObject technologySectorPrefab;
    public GameObject technologyButtonPrefab;


    // Start is called before the first frame update
    void Start()
    {
        test = GameManager.instance.test;

        InitTechnologyPanel();
        InitResourcesPanel();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitResourcesPanel()
    {
        if (useScience)
        {
            scienceGroup.SetActive(true);
            scienceTMP = scienceGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            scienceGroup.SetActive(false);
        }
        if (useCulture)
        {
            cultureGroup.SetActive(true);
            cultureTMP = cultureGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            cultureGroup.SetActive(false);
        }
        if (useFaith)
        {
            faithGroup.SetActive(true);
            faithTMP = faithGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            faithChangeTMP = faithGroup.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            faithGroup.SetActive(false);
        }
        if (useGold)
        {
            goldGroup.SetActive(true);
            goldTMP = goldGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            goldChangeTMP = goldGroup.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            goldGroup.SetActive(false);
        }
    }

    void InitTechnologyPanel()
    {
        technologyPanel.SetActive(false);
    }

    public void SetTechnologyPanel()
    {
        GameObject sector = new GameObject();
        int currentCost = 0;
        for (int i = 0; i < TechnologyManager.instance.technologies.Count; ++i)
        {
            Technology technology = TechnologyManager.instance.technologies[i];
            if (technology.researchCost > currentCost)
            {
                currentCost = technology.researchCost;
                sector = Instantiate(technologySectorPrefab);
                sector.transform.SetParent(technologyPanelContent.transform);
            }

            GameObject technologyButton = GetTechnologyButton(technology);
            technologyButton.transform.SetParent(sector.transform);


        }
    }

    public GameObject GetTechnologyButton(Technology technology)
    {
        GameObject technologyButton = Instantiate(technologyButtonPrefab);
        technologyButton.name = technology.name;

        technologyButton.transform.GetChild(0).GetComponent<Text>().text = technology.korean;
        technologyButton.GetComponent<TechnologyButtonListener>().SetButtonType(technology.id);

        return technologyButton;
    }

    public void TestResourcesUpdate(int value)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(int.Parse(scienceTMP.text) + value);
        goldTMP.text = goldChangeTMP.text = faithTMP.text = faithChangeTMP.text = cultureTMP.text = scienceTMP.text = sb.ToString();

        Canvas.ForceUpdateCanvases();
    }
}
