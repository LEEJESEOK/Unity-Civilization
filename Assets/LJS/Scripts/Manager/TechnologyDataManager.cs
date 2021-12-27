using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

// Technology
// https://namu.wiki/w/문명%206/과학%20기술
// https://civilization.fandom.com/wiki/List_of_technologies_in_Civ6
public class TechnologyDataManager : Singleton<TechnologyDataManager>
{
    public bool isSave;
    public bool isLoad;
    public string fileName = "Technologies.json";

    public List<Technology> technologies;


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

        technologies = new List<Technology>();

        #region save
        if (isSave)
        {
            SaveTechnologies();
        }
        #endregion

        #region load
        if (isLoad)
        {
            LoadTechnologies();

            UIManager.instance.SetTechnologyPanel(technologies);
        }
        #endregion
    }

    void SaveTechnologies()
    {
        technologies.Add(new Technology(TechnologyId.POTTERY, "Pottery", "도예", "", 25, null, null));
        technologies.Add(new Technology(TechnologyId.ANIMAL_HUSBANDRY, "Animal Husbandry", "목축업", "", 25, null, null));
        technologies.Add(new Technology(TechnologyId.MINING, "Mining", "채광", "", 25, new List<InGameObjectId> { InGameObjectId.MINE }, null));

        technologies.Add(new Technology(TechnologyId.IRRIGATION, "Irrigation", "관개", "", 50, null, new List<TechnologyId> { TechnologyId.POTTERY }));
        technologies.Add(new Technology(TechnologyId.WRITING, "Writing", "문자", "", 50, new List<InGameObjectId> { InGameObjectId.CAMPUS }, new List<TechnologyId> { TechnologyId.POTTERY }));
        technologies.Add(new Technology(TechnologyId.ARCHERY, "Archery", "궁술", "", 50, new List<InGameObjectId> { InGameObjectId.ARCHER }, new List<TechnologyId> { TechnologyId.ANIMAL_HUSBANDRY }));

        technologies.Add(new Technology(TechnologyId.MASONRY, "Masonry", "석조 기술", "", 80, null, new List<TechnologyId> { TechnologyId.MINING }));
        technologies.Add(new Technology(TechnologyId.BRONZE_WORKING, "Bronze Working", "", "청동 기술", 80, new List<InGameObjectId> { InGameObjectId.SPEARMAN }, new List<TechnologyId> { TechnologyId.MINING }));
        technologies.Add(new Technology(TechnologyId.WHEEL, "Wheel", "바퀴", "", 80, new List<InGameObjectId> { InGameObjectId.HEAVY_CHARIOT }, new List<TechnologyId> { TechnologyId.MINING }));

        technologies.Add(new Technology(TechnologyId.CURRENCY, "Currency", "화폐", "", 144, new List<InGameObjectId> { InGameObjectId.COMMERCIAL_HUB }, new List<TechnologyId> { TechnologyId.WRITING }));
        technologies.Add(new Technology(TechnologyId.HORSEBACK_RIDING, "Horseback Riding", "기마술", "", 144, null, new List<TechnologyId> { TechnologyId.ARCHERY }));
        technologies.Add(new Technology(TechnologyId.IRON_WORKING, "Iron Working", "철제 기술", "", 144, null, new List<TechnologyId> { TechnologyId.BRONZE_WORKING }));

        technologies.Add(new Technology(TechnologyId.MATHEMATICS, "Mathematics", "수학", "", 240, null, new List<TechnologyId> { TechnologyId.CURRENCY }));
        technologies.Add(new Technology(TechnologyId.CONSTRUCTION, "Construction", "건축", "", 240, null, new List<TechnologyId> { TechnologyId.MASONRY, TechnologyId.HORSEBACK_RIDING }));
        technologies.Add(new Technology(TechnologyId.ENGINEERING, "Engineering", "공학", "", 240, null, new List<TechnologyId> { TechnologyId.WHEEL }));

        technologies.Add(new Technology(TechnologyId.MILITARY_TACTICS, "Military Tactics", "군사 전술", "", 360, null, new List<TechnologyId> { TechnologyId.MATHEMATICS }));
        technologies.Add(new Technology(TechnologyId.APPRENTICESHIP, "Apprenticeship", "도제제도", "", 360, new List<InGameObjectId> { InGameObjectId.INDUSTRIAL_ZONE }, new List<TechnologyId> { TechnologyId.CURRENCY, TechnologyId.HORSEBACK_RIDING }));
        technologies.Add(new Technology(TechnologyId.MACHINERY, "Machinery", "기계", "", 360, null, new List<TechnologyId> { TechnologyId.IRON_WORKING, TechnologyId.ENGINEERING }));

        technologies.Add(new Technology(TechnologyId.EDUCATION, "Education", "교육", "", 468, null, new List<TechnologyId> { TechnologyId.MATHEMATICS, TechnologyId.APPRENTICESHIP }));
        technologies.Add(new Technology(TechnologyId.STIRRUPS, "Stirrups", "등자", "", 468, null, new List<TechnologyId> { TechnologyId.HORSEBACK_RIDING }));
        technologies.Add(new Technology(TechnologyId.MILITARY_ENGINEERING, "Military Engineering", "군사 공학", "", 468, null, new List<TechnologyId> { TechnologyId.CONSTRUCTION }));
        technologies.Add(new Technology(TechnologyId.CASTLES, "Castles", "성", "", 468, null, new List<TechnologyId> { TechnologyId.CONSTRUCTION }));

        technologies.Add(new Technology(TechnologyId.MASS_PRODUCTION, "Mass Production", "대량 생산", "", 720, null, new List<TechnologyId> { TechnologyId.EDUCATION, TechnologyId.MILITARY_TACTICS }));
        technologies.Add(new Technology(TechnologyId.BANKING, "Banking", "은행업", "", 720, null, new List<TechnologyId> { TechnologyId.EDUCATION, TechnologyId.STIRRUPS }));
        technologies.Add(new Technology(TechnologyId.PRINTING, "Printing", "인쇄술", "", 720, null, new List<TechnologyId> { TechnologyId.MACHINERY }));
        technologies.Add(new Technology(TechnologyId.GUNPOWDER, "Gunpowder", "화약", "", 720, null, new List<TechnologyId> { TechnologyId.APPRENTICESHIP, TechnologyId.STIRRUPS, TechnologyId.MILITARY_ENGINEERING }));

        technologies.Add(new Technology(TechnologyId.SIEGE_TACTICS, "Siege Tactics", "공성 전략", "", 730, null, new List<TechnologyId> { TechnologyId.CASTLES }));
        technologies.Add(new Technology(TechnologyId.METAL_CASTING, "Metal Casting", "주조", "", 730, null, new List<TechnologyId> { TechnologyId.GUNPOWDER }));
        technologies.Add(new Technology(TechnologyId.ASTRONOMY, "Astronomy", "천문학", "", 730, null, new List<TechnologyId> { TechnologyId.EDUCATION }));

        JsonHelper.ArrayDataSaveText<Technology>(technologies.ToArray(), fileName);
    }

    void LoadTechnologies()
    {
        technologies = new List<Technology>(JsonHelper.ArrayDataLoadText<Technology>(fileName));
    }

}
