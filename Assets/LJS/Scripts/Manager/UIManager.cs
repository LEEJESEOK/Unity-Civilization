using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
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

    #region Unit Panel
    [Header("Unit Panel")]
    [SerializeField]
    GameObject buildCityButton;
    [SerializeField]
    GameObject fortificationButton;

    public Image unitImage;
    public Image hpMeter;
    public GameObject meleeAttackGroup;
    public GameObject rangeAttackGroup;
    public GameObject movePowerGroup;
    public GameObject buildCountGroup;
    public TextMeshProUGUI rangeAttackTMP;
    public TextMeshProUGUI meleeAttackTMP;
    public TextMeshProUGUI movePowerTMP;
    public TextMeshProUGUI buildCountTMP;
    #endregion

    #region City Panel
    [Header("City Panel")]
    public Image cityImage;
    public TextMeshProUGUI cityFoodTMP;
    public TextMeshProUGUI cityProductTMP;
    public TextMeshProUGUI cityGoldTMP;
    public TextMeshProUGUI cityScienceTMP;
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

    #region TileInfo UI
    public GameObject tileInfo;
    public Vector3 mousePos;
    public float popupTime = 2;
    public float currentTime;
    bool isOpenPopup;
    public TextMeshProUGUI tileInfoText;
    #endregion

    RectTransform rect;
    UIButtonEvent uIButtonEvent;


    Dictionary<string, Sprite> spriteDict = new Dictionary<string, Sprite>();

    bool useScience;
    bool useCulture;
    bool useFaith;
    bool useGold;

    Vector2 prevMousePosition;
    static Vector3 camOffset = new Vector3(0, 3, -4);
    bool isLeftPressed;
    public bool isInit;

    LayerMask mapLayer;
    LayerMask fogLayer;


    // Start is called before the first frame update
    void Start()
    {
        #region Test
        // if (GameManager.instance.test)
        // {
        //     // load sample images
        //     samples = new List<Sprite>(Resources.LoadAll<Sprite>("Image/Sample"));
        // }
        // else
        {
            mouseCapture = true;
        }
        #endregion

        rect = GetComponent<RectTransform>();
        uIButtonEvent = GetComponent<UIButtonEvent>();

        // BuildFacilityButton ????????? ??????
        // StartCoroutine(CheckMouseOnTile());
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit == false)
            return;

        // if (GameManager.instance.test)
        // {
        //     #region sample image
        //     if (Input.GetKeyDown(KeyCode.Alpha0))
        //     {
        //         sample.gameObject.SetActive(!sample.gameObject.activeSelf);
        //     }
        //     if (Input.GetKeyDown(KeyCode.UpArrow))
        //     {
        //         sample.sprite = samples[(samples.IndexOf(sample.sprite) + 1) % samples.Count];
        //     }
        //     if (Input.GetKeyDown(KeyCode.DownArrow))
        //     {
        //         sample.sprite = samples[(samples.IndexOf(sample.sprite) - 1 + samples.Count) % samples.Count];
        //     }
        //     #endregion
        // }

        TileInfoPopUp();
    }

    public static void ClearUI()
    {
        MapManager.instance.InitMoveArea();
        UIPanelManager.instance.ClosePanel("UNIT_PANEL");
        UIPanelManager.instance.ClosePanel("CITY_PANEL");
        MapManager.instance.MarkDisabled();
    }

    // IEnumerator CheckMouseOnTile()
    // {
    //     yield return null;

    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, float.MaxValue, fogLayer | mapLayer))
    //     {
    //         if (hit.transform.gameObject.layer == LayerMask.NameToLayer("HexFog"))
    //         { 

    //         }
    //         else
    //         {

    //         }


    //     }
    // }

    public void TileInfoPopUp()
    {
        if (Camera.main == null)
        {
            return;
        }

        Ray rayPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        RaycastHit hitInfo;


        /* 
           1.1 ?????? ???????????? ???????????? ????????????
              2. ?????? 2?????? ????????????
              3. ???????????? ????????? ????????? ????????? ??????????????????.
              4. ?????? ????????? ????????? ????????? ???????????????.
          1.2 ??????????????? ????????? ???????????? ???????????? ????????? ????????????.
        */

        if (isOpenPopup == false)
            mousePos = Input.mousePosition;

        if (Physics.Raycast(rayPoint, out hit, 1000, HYO_ConstructManager.instance.fogLayer))
        {
            if (hit.transform.gameObject.GetComponent<MeshRenderer>().enabled == false)
            {
                if (Physics.Raycast(rayPoint, out hitInfo, float.MaxValue, HYO_ConstructManager.instance.layerMask))
                {
                    if (mousePos == Input.mousePosition)
                    {
                        currentTime += Time.deltaTime;
                        if (currentTime > popupTime)
                        {
                            isOpenPopup = true;

                            Transform tileTemp = hitInfo.transform;

                            if (tileTemp.GetComponent<TerrainData>() != null && !IsPointerOverUIObject())
                            {
                                UpdateTileInfo(tileTemp.GetComponent<TerrainData>());

                                //????????????
                                Vector3 pos = mousePos;
                                pos.x += 150f;


                                // if (pos.x < 250f) pos.x = 250f;

                                // if (pos.x > (1920f - 250f)) pos.x = (1920f - 250f);

                                // if (pos.y < 300f) pos.y = 300f;

                                // if (pos.y > (1080f - 300f)) pos.y = (1080f - 300f);

                                pos.x = Mathf.Clamp(pos.x, 125, 1920 - 125);
                                pos.y = Mathf.Clamp(pos.y, 150, 1080 - 150);

                                tileInfo.transform.position = new Vector3(pos.x, pos.y);

                                tileInfo.SetActive(true);
                            }
                            currentTime = 0;

                        }
                    }
                    else if (tileInfo.activeSelf == true)
                    {
                        isOpenPopup = false;

                        tileInfo.SetActive(false);
                    }

                }
            }
        }
    }

    internal void EnableCityBuild()
    {
        buildCityButton.SetActive(true);
    }

    internal void DisableCityBuild()
    {
        buildCityButton.SetActive(false);
    }

    internal void EnableFortification()
    {
        fortificationButton.SetActive(true);
    }

    internal void DisableFortificcation()
    {
        fortificationButton.SetActive(false);
    }

    public void GetTileInfo(TerrainType type, GameObject center, int move, int food, int prod)
    {
        tileInfoText.text = type.ToString() + Environment.NewLine;
        tileInfoText.text += "?????????:" + center.ToString() + Environment.NewLine;
        tileInfoText.text += "?????????:" + move + Environment.NewLine;
        tileInfoText.text += food + "??????" + Environment.NewLine;
        tileInfoText.text += prod + "?????????" + Environment.NewLine;
    }

    void UpdateTileInfo(TerrainData terrainData)
    {
        tileInfoText.text = "<b>" + terrainData.terrainType.ToString() + "</b>" + Environment.NewLine;
        if (terrainData.myCenter != null)
        {
            Territory territory = terrainData.myCenter.GetComponent<Territory>();
            if (territory != null)
                tileInfoText.text += "?????????:" + GameManager.instance.players[territory.tt_playerId].name + Environment.NewLine;
        }
        tileInfoText.text += "?????????:" + terrainData.output.movePower + Environment.NewLine;
        tileInfoText.text += terrainData.output.food + "<sprite name=food>" + Environment.NewLine;
        tileInfoText.text += terrainData.output.productivity + "<sprite name=production>" + Environment.NewLine;
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

    Sprite[] LoadAllSprite(string path)
    {
        return Resources.LoadAll<Sprite>(path);
    }

    public IEnumerator Initialize()
    {
        if (mouseCapture)
            // ????????? ????????? ????????? ????????? ????????? ????????? ???
            Cursor.lockState = CursorLockMode.Confined;

        InitResourcesIndicator();
        InitUnitPanel();
        isInit = true;
        yield return null;
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
    // ????????? ???????????? ?????? Vector3 ????????? ??????
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

    // ??????????????? ??? ?????? ??????
    public void CameraMove(Camera cam)
    {
        if (cam == null) return;

        Vector3 cameraDir = Vector3.zero;

        Vector2 mousePosition = Input.mousePosition;

        // ????????? ????????? ?????? ????????? ????????? ??????
        if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (UIManager.IsPointerOverUIObject()) return;

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

        if (Input.GetKeyUp(KeyCode.Space) && (GameManager.instance.IsCurrentUnit()))
        {
            Vector3 currentPos = GameManager.instance.currentSelect.transform.position;
            currentPos.y = camOffset.y;
            currentPos.z += camOffset.z;

            cam.transform.position = currentPos;
        }

        // TODO ???????????? ?????? ??????

        // ???????????? ?????? ????????? ????????? ????????? ??????
        Vector3 pos = cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, -28, 8);
        pos.z = Mathf.Clamp(pos.z, -28, 16);

        cam.transform.position = pos;
    }

    // ????????? ??? ???????????? ?????? ???
    // Field of View ??????(30~90, default : 60)
    public void CameraZoom(Camera cam)
    {
        if (UIManager.IsPointerOverUIObject()) return;

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

    #region UnitPanel
    public void InitUnitPanel()
    {
        #region load component
        meleeAttackTMP = meleeAttackGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rangeAttackTMP = rangeAttackGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        movePowerTMP = movePowerGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        buildCountTMP = buildCountGroup.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        #endregion

        DisableCityBuild();
        UIPanelManager.instance.ClosePanel("BUILD_FACILITY_COMMAND_TAB");
        DisableFortificcation();
    }

    void UpdateUnitCommonData(Unit unit)
    {
        meleeAttackGroup.SetActive(false);
        rangeAttackGroup.SetActive(false);
        buildCountGroup.SetActive(false);

        // TODO ?????? ?????????
        print(unit.unitType);
        print(unitImage == null);
        unitImage.sprite = Resources.Load<Sprite>("Image/UI/ObjectPanel/UnitPanel/Class/" + unit.unitType.ToString());
        print(unitImage.sprite.name);

        // ?????? ??????
        float hpRatio = (float)unit.hp / 100f;
        hpMeter.fillAmount = hpRatio;
        hpMeter.color = new Color(1 - hpRatio, hpRatio, 0f);

        // ?????????
        movePowerTMP.text = unit.movePower.ToString() + "/" + unit.maxMovePower.ToString();
    }

    public void UpdateUnitData(CombatUnit unit)
    {
        UpdateUnitCommonData(unit);

        meleeAttackGroup.SetActive(true);
        rangeAttackGroup.SetActive(true);

        meleeAttackTMP.text = unit.meleeAttack.ToString();
        rangeAttackTMP.text = unit.rangeAttack.ToString();
    }

    public void UpdateUnitData(NonCombatUnit unit)
    {
        UpdateUnitCommonData(unit);

        if (unit.unitType == InGameObjectId.BUILDER)
        {
            buildCountGroup.SetActive(true);
            buildCountTMP.text = unit.buildCount.ToString() + "/" + unit.maxBuildCount.ToString();
        }
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

        Sprite sprite = Resources.Load<Sprite>("Image/UI/Technology/" + technology.name);
        TechnologyButtonSetter technologyButtonSetter = technologyButton.GetComponent<TechnologyButtonSetter>();
        technologyButtonSetter.SetTechnologyButton(technology.korean, sprite, technology.koreanDesc, technology.unlockObjectId);

        TechnologyButtonListener technologyButtonListener = technologyButton.GetComponent<TechnologyButtonListener>();
        technologyButtonListener.SetButtonType(technology.id);
        technologyButtonListener.AddClickCallback(uIButtonEvent.SelectOngoingTechnology);

        return technologyButton;
    }

    public void UpdateTechnologyPanel()
    {
        TechnologyButtonListener[] technologyButtonListeners = GetComponentsInChildren<TechnologyButtonListener>(true);
        PlayerInfo currentPlayer = GameManager.instance.currentPlayer.info;

        for (int i = 0; i < technologyButtonListeners.Length; ++i)
        {
            // ?????? ?????? ??????
            bool interactableState = true;

            // ?????? ????????? ??????????????? ??????
            List<TechnologyId> requireTechIds = currentPlayer.technologies.Find(x => x.id == technologyButtonListeners[i].buttonType).requireTechId;
            for (int j = 0; j < requireTechIds.Count; ++j)
                if (currentPlayer.technologies.Find(x => x.id == requireTechIds[j]).isResearched == false)
                    interactableState = false;

            // ?????? ????????? ??????????????? ??????
            if (currentPlayer.technologies.Find(x => x.id == technologyButtonListeners[i].buttonType).isResearched)
                interactableState = false;

            technologyButtonListeners[i].GetComponent<Button>().interactable = interactableState;
        }
    }
    #endregion

    #region SelectedTechnology
    public void InitSelectedTechnology()
    {
        selectedTechnologyName.text = "?????? ??????";
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
        selectedTechnologyImage.sprite = Resources.Load<Sprite>("Image/UI/Technology/" + technology.name);
        Color color = selectedTechnologyImage.color;
        color.a = 1;
        selectedTechnologyImage.color = color;
        selectedTechnologyMeter.fillAmount = ((float)(technology.researchCost - technology.remainCost)) / technology.researchCost;

        // unlockObject

        int remainTurn = Mathf.CeilToInt((float)technology.remainCost / GameManager.instance.currentPlayer.info.science);
        if (remainTurn > 0)
            selectedTechnologyRemainTurn.text = "??? : " + System.Environment.NewLine
             + remainTurn;
        else
            selectedTechnologyRemainTurn.text = System.Environment.NewLine + "?????? ??????";
    }
    #endregion

    #region CityProductPanel
    // ProductObjectData??? item?????? ???????????? ??????
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

        #region set button
        for (int i = 0; i < productObjects.Count; ++i)
        {
            ProductObject productObject = productObjects[i];
            GameObject productObjectButton = GetCityProductButton(productObject, productObjectButtonPrefab);
            GameObject goldObjectButton = GetCityProductButton(productObject, goldObjectButtonPrefab);

            switch (productObject.type)
            {
                case TypeIdBase.DISTRICT:
                    productObjectButton.transform.SetParent(productBuildingContent);
                    productObjectButton.GetComponentInChildren<ProductObjectButtonListener>().AddClickCallback(uIButtonEvent.BuildDistrict);
                    // ?????? ?????? ???????????? ?????? ??????
                    goldObjectButton.transform.SetParent(goldBuildingContent);
                    goldObjectButton.SetActive(false);
                    break;
                case TypeIdBase.UNIT:
                    productObjectButton.transform.SetParent(productUnitContent);
                    productObjectButton.GetComponentInChildren<ProductObjectButtonListener>().AddClickCallback(uIButtonEvent.ProductUnit);
                    goldObjectButton.transform.SetParent(goldUnitContent);
                    productObjectButton.GetComponentInChildren<ProductObjectButtonListener>().AddClickCallback(uIButtonEvent.BuyUnit);
                    break;
            }

            // productObjectButton.SetActive(false);
            // goldObjectButton.SetActive(false);
        }
        #endregion
    }

    // TODO Event Listener
    // build district, product unit, buy unit
    GameObject GetCityProductButton(ProductObject productObject, GameObject buttonPrefab)
    {
        GameObject productObjectButton = Instantiate(buttonPrefab);
        productObjectButton.name = productObject.name;

        ProductObjectButtonSetter productObjectButtonSetter = productObjectButton.GetComponentInChildren<ProductObjectButtonSetter>();
        productObjectButtonSetter.SetProductObjectButton(productObject.korean, spriteDict[productObject.name], productObject.productCost);

        ProductObjectButtonListener productObjectButtonListener = productObjectButton.GetComponentInChildren<ProductObjectButtonListener>();
        productObjectButtonListener.SetButtonType(productObject.id);

        return productObjectButton;
    }

    // ?????? ???????????? ??? ??????
    public void UpdateCityPanelData(Territory territory)
    {
        cityImage.sprite = Resources.Load<Sprite>("Image/UI/ObjectPanel/CityPanel/Castle");
        cityFoodTMP.text = territory.totalOutput.Totalfood.ToString();
        cityProductTMP.text = territory.totalOutput.TotalProductivity.ToString();
        cityGoldTMP.text = territory.totalOutput.TotalGold.ToString();
        cityScienceTMP.text = territory.totalOutput.TotalScience.ToString();

        UpdateCityProductPanelData(territory);
    }

    // ?????? ????????? ??????, ?????? ?????????
    // ????????? ???????????? ?????? ?????? ?????? ??????
    public void UpdateCityProductPanelData(Territory territory)
    {
        ProductObjectButtonListener[] buttonListeners = GetComponentsInChildren<ProductObjectButtonListener>(true);
        ProductObjectButtonSetter[] buttonSetter = GetComponentsInChildren<ProductObjectButtonSetter>(true);

        // ?????? ????????? ??????????????? ?????????????????? ??????
        // ?????? ????????? ????????? ????????? ??????
        // ?????? ????????? ????????? ???????????? ???
        for (int i = 0; i < buttonListeners.Length; ++i)
        {
            ProductObject productObject = ProductObjectDataManager.instance.productObjects.Find(x => x.id == buttonListeners[i].buttonType);

            // // ???????????? ?????? ??????????????? ???????????? ??????
            bool isUnlocked = (productObject.requireTechId == TechnologyId.NONE)
                            || (GameManager.instance.currentPlayer.info.technologies.Find(x => x.id == productObject.requireTechId).isResearched);
            buttonListeners[i].transform.parent.gameObject.SetActive(isUnlocked);
            // buttonListeners[i].GetComponent<Button>().interactable = isUnlocked;

            if (isUnlocked == false)
                continue;


            // TODO ????????? ?????? ?????? ??????
            // ?????? ???????????? ????????? ?????? ????????? ??????

            // ????????? ????????? ??????????????? ?????? ?????? ??????
            buttonSetter[i].UpdateCost(productObject.productCost / territory.totalOutput.TotalProductivity);
        }

        ResizeLayoutGroup(gameObject);
    }
    #endregion
}
