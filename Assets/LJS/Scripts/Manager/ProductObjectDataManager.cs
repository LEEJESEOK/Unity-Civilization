using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// District
// https://namu.wiki/w/문명%206/특수지구와%20건물
// https://civilization.fandom.com/wiki/List_of_districts_in_Civ6
// Unit
// https://namu.wiki/w/문명%206/유닛
// https://civilization.fandom.com/wiki/List_of_units_in_Civ6
public class ProductObjectDataManager : Singleton<ProductObjectDataManager>
{
    public bool isSave;
    public bool isLoad;

    [SerializeField]
    string fileName = "ProductObject.json";

    public List<ProductObject> productObjects;


    // Start is called before the first frame update
    void Start()
    {
        #region test
        if (GameManager.instance.test == false)
        {
            isSave = false;
            isLoad = true;
        }
        #endregion

        productObjects = new List<ProductObject>();

        #region save
        if (isSave)
        {
            SaveProductObjects();
        }
        #endregion
        #region load
        if (isLoad)
        {
            LoadProductObjects();
            UIManager.instance.InitCityProductPanelData(productObjects);
        }
        #endregion

    }

    void SaveProductObjects()
    {
        // District
        productObjects.Add(new ProductObject(TypeIdBase.DISTRICT, InGameObjectId.CAMPUS, "Campus", "캠퍼스", 54, 0, 1, TechnologyId.WRITING));
        productObjects.Add(new ProductObject(TypeIdBase.DISTRICT, InGameObjectId.COMMERCIAL_HUB, "Commercial Hub", "상업 중심지", 54, 0, 0, TechnologyId.CURRENCY));
        productObjects.Add(new ProductObject(TypeIdBase.DISTRICT, InGameObjectId.INDUSTRIAL_ZONE, "Industrial Zone", "산업구역", 54, 0, 1, TechnologyId.APPRENTICESHIP));

        // Unit
        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.SETTLER, "Settler", "개척자", 80, 320, 0, TechnologyId.NONE));
        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.BUILDER, "Builder", "건설자", 50, 200, 0, TechnologyId.NONE));

        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.SCOUT, "Scout", "정찰병", 30, 120, 0, TechnologyId.NONE));

        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.WARRIOR, "Warrior", "전사", 40, 160, 0, TechnologyId.NONE));

        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.SLINGER, "Slinger", "투석병사", 35, 140, 0, TechnologyId.NONE));
        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.ARCHER, "Archer", "궁수", 60, 240, 1, TechnologyId.ARCHERY));

        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.SPEARMAN, "Spearman", "창병", 65, 260, 1, TechnologyId.BRONZE_WORKING));

        productObjects.Add(new ProductObject(TypeIdBase.UNIT, InGameObjectId.HEAVY_CHARIOT, "Heavy Chariot", "중전차", 65, 260, 1, TechnologyId.WHEEL));


        JsonHelper.ArrayDataSaveText<ProductObject>(productObjects.ToArray(), fileName);
    }

    void LoadProductObjects()
    {
        productObjects = new List<ProductObject>(JsonHelper.ArrayDataLoadText<ProductObject>(fileName));
    }
}
