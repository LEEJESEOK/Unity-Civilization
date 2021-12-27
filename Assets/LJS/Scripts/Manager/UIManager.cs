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
    [Header("Test")]
    public Image sample;
    public List<Sprite> samples;

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

    [Header("Selected Technology")]
    public TextMeshProUGUI selectedTechnologyName;
    public Image selectedTechnologyImage;
    public Image selectedTechnologyMeter;
    public TextMeshProUGUI selectedTechnologyRemainTurn;
    #endregion

    #region City Product
    [Header("City Product Panel")]
    public GameObject productObjectButtonPrefab;
    public GameObject goldObjectButtonPrefab;
    public Transform productBuildingContent;
    public Transform productUnitContent;
    public Transform goldBuildingContent;
    public Transform goldUnitContent;
    #endregion

    RectTransform rect;
    UIButtonEvent uIButtonEvent;


    Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    bool useScience;
    bool useCulture;
    bool useFaith;
    bool useGold;

    Vector2 prevMousePosition;
    bool isLeftPressed;


    // Start is called before the first frame update
    void Start()
    {
        #region Test
        if (GameManager.instance.test)
        {
            // load sample images
            samples = new List<Sprite>(Resources.LoadAll<Sprite>("Image/Sample"));
        }
        else
        {
            mouseCapture = true;
        }
        #endregion

        rect = GetComponent<RectTransform>();
        uIButtonEvent = GetComponent<UIButtonEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.test)
        {
            #region sample image
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                sample.gameObject.SetActive(!sample.gameObject.activeSelf);
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                sample.sprite = samples[(samples.IndexOf(sample.sprite) + 1) % samples.Count];
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                sample.sprite = samples[(samples.IndexOf(sample.sprite) - 1 + samples.Count) % samples.Count];
            }
            #endregion
        }
    }

    public static void ResizeLayoutGroup(GameObject layoutObject)
    {
        LayoutGroup[] layoutGroups = layoutObject.GetComponentsInChildren<LayoutGroup>();
        for (int i = 0; i < layoutGroups.Length; ++i)
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroups[i].GetComponent<RectTransform>());
    }

    public static bool IsPointerOverUIObject()
    {
        #region old
        // PointerEventData currentEventData = new PointerEventData(EventSystem.current);
        // currentEventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        // List<RaycastResult> results = new List<RaycastResult>();
        // EventSystem.current.RaycastAll(currentEventData, results);

        // return results.Count > 0;
        #endregion
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

    #region Camera
    // 위치한 모서리에 따라 Vector3 형태로 반환
    // left : (-1, 0, 0)
    // right : (1, 0, 0)
    // up : (0, 0, 1)
    // down : (0, 0, -1)
    // Vector3 OnScreenEdge(Vector2 position)
    // {
    //     Vector3 result = Vector3.zero;
    //     // left
    //     if ((position.x <= 10))
    //         result.x = -1;
    //     // right
    //     else if ((position.x >= Screen.width - 10))
    //         result.x = 1;
    //     // up
    //     if ((position.y >= Screen.height - 10))
    //         result.z = 1;
    //     // down
    //     else if ((position.y <= 10))
    //         result.z = -1;

    //     return result;
    // }

    // 드래그했을 때 화면 이동
    public void CameraMove(Camera cam)
    {
        if (cam == null) return;
        if (UIManager.IsPointerOverUIObject()) return;

        Vector3 cameraDir = Vector3.zero;

        Vector2 mousePosition = Input.mousePosition;

        // 마우스 커서가 화면 범위에 있는지 검사
        if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                isLeftPressed = true;
                prevMousePosition = mousePosition;
            }
            if ((isLeftPressed == true) && (mousePosition != prevMousePosition))
            {
                Vector2 value = mousePosition - prevMousePosition;
                prevMousePosition = mousePosition;

                cameraDir = new Vector3(-value.x, 0, -value.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isLeftPressed = false;
            }

            cam.transform.position += cameraDir * Time.deltaTime;
        }
    }

    // 마우스 휠 입력으로 화면 줌
    // Field of View 조절(30~90, default : 60)
    public void CameraZoom(Camera cam)
    {
        if (UIPanelManager.instance.currentPanel != null) return;

        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        // zoom in
        if (wheelInput > 0)
            cam.fieldOfView -= GameManager.instance.cameraZoomSpeed;
        // zoom out
        if (wheelInput < 0)
            cam.fieldOfView += GameManager.instance.cameraZoomSpeed;

        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 30f, 90f);
    }
    #endregion

    #region TechnologyPanel
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
        technologyButtonSetter.SetTechnologyButton(technology.korean, sprite, technology.koreanDesc, technology.unlockObjectId);

        TechnologyButtonListener technologyButtonListener = technologyButton.GetComponent<TechnologyButtonListener>();
        technologyButtonListener.SetButtonType(technology.id);
        technologyButtonListener.AddClickCallback(uIButtonEvent.SelectOngoingTechnology);

        UIButtonListener uIButtonListener = technologyButton.GetComponent<UIButtonListener>();
        uIButtonEvent.AddUIListener(uIButtonListener);

        return technologyButton;
    }
    #endregion

    #region SelectedTechnology
    public void InitSelectedTechnology()
    {
        selectedTechnologyName.text = "연구 선택";
        // TODO 연구 기본 선택 이미지
        selectedTechnologyImage.sprite = Resources.Load<Sprite>("Image/WhiteCircle");
        Color color = selectedTechnologyImage.color;
        color.a = 0;
        selectedTechnologyImage.color = color;
        selectedTechnologyMeter.fillAmount = 0f;
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
        selectedTechnologyMeter.fillAmount = ((float)(technology.researchCost - technology.remainCost)) / technology.researchCost;

        // unlockObject

        int remainTurn = Mathf.CeilToInt((float)technology.remainCost / GameManager.instance.currentPlayer.info.science);
        if (remainTurn > 0)
            selectedTechnologyRemainTurn.text = "턴 : " + System.Environment.NewLine
             + remainTurn;
        else
            selectedTechnologyRemainTurn.text = System.Environment.NewLine + "방금 완성";
    }
    #endregion

    #region CityProductPanel
    // ProductObjectData의 item들을 버튼으로 생성
    public void InitCityProductPanelData(List<ProductObject> productObjects)
    {
        #region load sprite
        // load sprite 
        string path = "Image/UI/UnitClass";
        Sprite[] unitClassSprites = LoadAllSprite(path);
        for (int i = 0; i < unitClassSprites.Length; ++i)
            spriteDict[unitClassSprites[i].name] = unitClassSprites[i];

        path = "Image/District";
        Sprite[] districtSprites = LoadAllSprite(path);
        for (int i = 0; i < districtSprites.Length; ++i)
            spriteDict[districtSprites[i].name] = districtSprites[i];
        #endregion

        // product
        for (int i = 0; i < productObjects.Count; ++i)
        {
            GameObject productObjectButton = GetCityProductButton(productObjects[i], productObjectButtonPrefab);

            switch (productObjects[i].type)
            {
                case TypeIdBase.DISTRICT:
                    productObjectButton.transform.SetParent(productBuildingContent);
                    break;
                case TypeIdBase.UNIT:
                    productObjectButton.transform.SetParent(productUnitContent);
                    break;
            }
        }

        // gold
        for (int i = 0; i < productObjects.Count; ++i)
        {
            GameObject productObjectButton = GetCityProductButton(productObjects[i], goldObjectButtonPrefab);

            switch (productObjects[i].type)
            {
                case TypeIdBase.DISTRICT:
                    productObjectButton.transform.SetParent(goldBuildingContent);
                    break;
                case TypeIdBase.UNIT:
                    productObjectButton.transform.SetParent(goldUnitContent);
                    break;
            }
        }


    }

    GameObject GetCityProductButton(ProductObject productObject, GameObject buttonPrefab)
    {
        GameObject productObjectButton = Instantiate(buttonPrefab);
        productObjectButton.name = productObject.name;
        ProductObjectButtonSetter productObjectButtonSetter = productObjectButton.GetComponentInChildren<ProductObjectButtonSetter>();
        productObjectButtonSetter.SetProductObjectButton(productObject.korean, spriteDict[productObject.name], productObject.productCost);

        ProductObjectButtonListener productObjectButtonListener = productObjectButton.GetComponentInChildren<ProductObjectButtonListener>();
        productObjectButtonListener.SetButtonType(productObject.id);

        UIButtonListener uIButtonListener = productObjectButton.GetComponentInChildren<UIButtonListener>();
        uIButtonEvent.AddUIListener(uIButtonListener);

        return productObjectButton;
    }

    // 도시 선택했을 때 호출
    // 생산 가능한 건물, 유닛 최신화
    // 도시의 생산력에 따라 남은 턴수 표시
    public void UpdateCityProductPanelData(Territory territory)
    {
        // 연구 진행으로 추가된 건물 표시
        // 선택한 도시가 건설 가능한 건물만 활성화
        // 건설 불가능한 건물은 비활성화(이미 건설한 건물)

        // 생산 가능한 유닛 표시(연구)
        // 생산 불가능한 유닛들은 표시하지 않음
    }
    #endregion

    // TODO Sprite[] LoadAllSprite(path)
    Sprite[] LoadAllSprite(string path)
    {
        return Resources.LoadAll<Sprite>(path);
    }
}
