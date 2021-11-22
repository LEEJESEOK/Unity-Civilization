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
    bool useScience;
    bool useCulture;
    bool useFaith;
    bool useGold;

    [Header("Resources")]
    public GameObject resourcesWrapper;
    public GameObject scienceGroup;
    public GameObject cultureGroup;
    public GameObject faithGroup;
    public GameObject faithChangeGroup;
    public GameObject goldGroup;
    public GameObject goldChangeGroup;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitUI()
    {
        useScience = GameManager.instance.useScience;
        useCulture = GameManager.instance.useCulture;
        useFaith = GameManager.instance.useFaith;
        useGold = GameManager.instance.useGold;

        InitTechnologyPanel();
        InitResourcesPanel();
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
            faithChangeTMP = faithChangeGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            faithGroup.SetActive(false);
        }
        if (useGold)
        {
            goldGroup.SetActive(true);
            goldTMP = goldGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            goldChangeTMP = goldChangeGroup.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
        else
        {
            goldGroup.SetActive(false);
        }

        ResizeLayoutGroup(resourcesWrapper);
    }

    void InitTechnologyPanel()
    {
        technologyPanel.SetActive(false);
    }

    public void SetTechnologyPanel()
    {
        GameObject sector = Instantiate(technologySectorPrefab);
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

        technologyButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = technology.korean;
        technologyButton.GetComponent<TechnologyButtonListener>().SetButtonType(technology.id);

        return technologyButton;
    }

    public void ResourcesUpdate()
    {
        ResizeLayoutGroup(resourcesWrapper);
    }

    void ResizeLayoutGroup(GameObject layoutObject)
    {
        LayoutGroup[] layoutGroups = layoutObject.GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; ++i)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroups[i].GetComponent<RectTransform>());
    }

}
