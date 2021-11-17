using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Common")]
    public bool test;

    [Header("Technology")]
    public GameObject technologyPanel;
    public GameObject technologyPanelContent;
    public GameObject sectorPrefab;
    public GameObject technologyButtonPrefab;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                sector = Instantiate(sectorPrefab);
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
}
