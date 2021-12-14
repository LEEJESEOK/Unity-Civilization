using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    bool test;

    [Header("Common")]
    public bool mouseCapture = true;

    #region Resources
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
    public Color defaultGoldColor;
    public TextMeshProUGUI goldTMP;
    public TextMeshProUGUI goldChangeTMP;
    #endregion

    #region Technology
    [Header("Technology Panel")]
    public GameObject technologyPanel;
    public GameObject technologyPanelContent;
    public GameObject technologySectorPrefab;
    public GameObject technologyButtonPrefab;

    [Header("SelectedTechnology")]
    public TextMeshProUGUI selectedTechnologyName;
    public Image selectedTechnologyImage;
    public TextMeshProUGUI selectedTechnologyRemainTurn;


    #endregion

    UIButtonEvent uIButtonEvent;

    bool useScience;
    bool useCulture;
    bool useFaith;
    bool useGold;


    // Start is called before the first frame update
    void Start()
    {
        test = GameManager.instance.test;

        uIButtonEvent = GetComponent<UIButtonEvent>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ResizeLayoutGroup(GameObject layoutObject)
    {
        LayoutGroup[] layoutGroups = layoutObject.GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; ++i)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroups[i].GetComponent<RectTransform>());
    }

    public static bool IsPointerOverUIObject()
    {
        // PointerEventData currentEventData = new PointerEventData(EventSystem.current);
        // currentEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        // List<RaycastResult> results = new List<RaycastResult>();
        // EventSystem.current.RaycastAll(currentEventData, results);

        // return results.Count > 0;
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void InitUI()
    {
        if (mouseCapture)
            // 마우스 커서가 윈도우 밖으로 나가지 않도록 함
            Cursor.lockState = CursorLockMode.Confined;

        InitResourcesIndicator();
    }

    void InitResourcesIndicator()
    {
        useScience = GameManager.instance.useScience;
        useCulture = GameManager.instance.useCulture;
        useFaith = GameManager.instance.useFaith;
        useGold = GameManager.instance.useGold;

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

    public void SetTechnologyPanel(List<Technology> technologies)
    {
        GameObject sector = Instantiate(technologySectorPrefab);
        Destroy(sector);
        int currentCost = 0;
        for (int i = 0; i < technologies.Count; ++i)
        {
            Technology technology = technologies[i];
            if (technology.researchCost > currentCost)
            {
                currentCost = technology.researchCost;
                sector = Instantiate(technologySectorPrefab);
                sector.name = "Science " + currentCost;
                sector.transform.SetParent(technologyPanelContent.transform);
            }

            GameObject technologyButton = GetTechnologyButton(technology);
            technologyButton.transform.SetParent(sector.transform);
        }
    }

    GameObject GetTechnologyButton(Technology technology)
    {
        GameObject technologyButton = Instantiate(technologyButtonPrefab);
        technologyButton.name = technology.name;

        Sprite sprite = Resources.Load<Sprite>("Image/Technology/" + technology.name);
        TechnologyButtonSetter technologyButtonSetter = technologyButton.GetComponent<TechnologyButtonSetter>();
        technologyButtonSetter.SetTechnologyButton(technology.korean, sprite, "");

        TechnologyButtonListener technologyButtonListener = technologyButton.GetComponent<TechnologyButtonListener>();
        technologyButtonListener.SetButtonType(technology.id);
        technologyButtonListener.AddClickCallback(uIButtonEvent.SelectOngoingTechnology);

        UIButtonListener uIButtonListener = technologyButton.GetComponent<UIButtonListener>();
        uIButtonEvent.AddUIListener(uIButtonListener);

        return technologyButton;
    }

    public void UpdateResource(int science, int culture, int faith, int faithChange, int gold, int goldChange)
    {
        StringBuilder sb = new StringBuilder();

        if (useScience)
        {
            sb.Clear();
            sb.Append(science);
            scienceTMP.text = sb.ToString();
        }

        if (useCulture)
        {
            sb.Clear();
            sb.Append(culture);
            cultureTMP.text = sb.ToString();
        }

        if (useFaith)
        {
            sb.Clear();
            sb.Append(faith);
            faithTMP.text = sb.ToString();

            sb.Clear();
            sb.Append(faithChange);
            faithChangeTMP.text = sb.ToString();
        }

        if (useGold)
        {
            sb.Clear();
            sb.Append(gold);
            goldTMP.text = sb.ToString();

            sb.Clear();
            if (goldChange > 0)
                sb.Append("+");
            else if (goldChange < 0)
                sb.Append("-");
            sb.Append(goldChange);
            goldChangeTMP.text = sb.ToString();
            if (goldChange >= 0)
                goldChangeTMP.color = defaultGoldColor;
            else
                goldChangeTMP.color = Color.red;
        }

        ResizeLayoutGroup(resourcesWrapper);
    }

    public void InitSelectedTechnology()
    {
        selectedTechnologyName.text = "연구 선택";
        // TODO 연구 기본 선택 이미지
        selectedTechnologyImage.sprite = Resources.Load<Sprite>("Image/WhiteCircle");
        Color color = selectedTechnologyImage.color;
        color.a = 0;
        selectedTechnologyImage.color = color;
        selectedTechnologyRemainTurn.text = "";
    }

    public void UpdateSelectedTechnology(Technology technology)
    {
        // name(korean)
        selectedTechnologyName.text = technology.korean;
        // image
        selectedTechnologyImage.sprite = Resources.Load<Sprite>("Image/Technology/" + technology.name);
        Color color = selectedTechnologyImage.color;
        color.a = 1;
        selectedTechnologyImage.color = color;
        // unlockObject
        // remainCost -> turn
        int remainTurn = Mathf.CeilToInt((float)technology.remainCost / GameManager.instance.currentPlayer.info.science);
        if (remainTurn > 0)
            selectedTechnologyRemainTurn.text = "턴 : " + System.Environment.NewLine
             + remainTurn;
        else
            selectedTechnologyRemainTurn.text = System.Environment.NewLine + "방금 완성";
    }

    // 화면 가장자리로 마우스 이동했을 때 화면 이동
    // TODO 우클릭 드래그했을 때 화면 이동
    public void CameraMove(Camera cam)
    {
        if (cam == null) return;

        Vector3 cameraDir = Vector3.zero;

        // left
        if ((Input.mousePosition.x <= 5) && Input.mousePosition.x > -5)
            cameraDir = Vector3.left;
        // right
        if ((Input.mousePosition.x >= Screen.width - 5) && Input.mousePosition.x < Screen.width + 5)
            cameraDir = Vector3.right;
        // forward
        if ((Input.mousePosition.y >= Screen.height - 5) && (Input.mousePosition.y < Screen.height + 5))
            cameraDir = Vector3.forward;
        // back
        if ((Input.mousePosition.y <= 5) && (Input.mousePosition.y > -5))
            cameraDir = Vector3.back;

        cam.transform.position += cameraDir * GameManager.instance.cameraMoveSpeed * Time.deltaTime;
    }

    // 마우스 휠 입력으로 화면 줌
    // Field of View 조절(30~90, default : 60)
    public void CameraZoom(Camera cam)
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        // zoom in
        if (wheelInput > 0)
            cam.fieldOfView -= GameManager.instance.cameraZoomSpeed * Time.deltaTime;
        // zoom out
        if (wheelInput < 0)
            cam.fieldOfView += GameManager.instance.cameraZoomSpeed * Time.deltaTime;

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 30f, 90f);
    }
}
