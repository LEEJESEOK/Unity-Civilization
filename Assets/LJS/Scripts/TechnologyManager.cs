using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TechnologyManager : Singleton<TechnologyManager>
{
    public bool isSave;
    public bool isLoad;
    public string fileName;

    public List<Technology> technologies;


    // Start is called before the first frame update
    void Start()
    {
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

            UIManager.instance.SetTechnologies(technologies);
        }
        #endregion
    }

    void SaveTechnologies()
    {
        technologies.Add(new Technology(TechnologyId.Pottery, "Pottery", "도예", 25, null, null));
        technologies.Add(new Technology(TechnologyId.AnimalHusbandry, "Animal Husbandry", "목축업", 25, null, null));
        technologies.Add(new Technology(TechnologyId.Mining, "Mining", "채광", 25, null, null));

        technologies.Add(new Technology(TechnologyId.Irrigation, "Irrigation", "관개", 50, null, new List<TechnologyId> { TechnologyId.Pottery }));
        technologies.Add(new Technology(TechnologyId.Writing, "Writing", "문자", 50, null, new List<TechnologyId> { TechnologyId.Pottery }));
        technologies.Add(new Technology(TechnologyId.Archery, "Archery", "궁술", 50, null, new List<TechnologyId> { TechnologyId.AnimalHusbandry }));

        technologies.Add(new Technology(TechnologyId.Masonry, "Masonry", "석조 기술", 80, null, new List<TechnologyId> { TechnologyId.Mining }));
        technologies.Add(new Technology(TechnologyId.BronzeWorking, "Bronze Working", "청동 기술", 80, null, new List<TechnologyId> { TechnologyId.Mining }));
        technologies.Add(new Technology(TechnologyId.Wheel, "Wheel", "바퀴", 80, null, new List<TechnologyId> { TechnologyId.Mining }));

        technologies.Add(new Technology(TechnologyId.Currency, "Currency", "화폐", 144, null, new List<TechnologyId> { TechnologyId.Writing }));
        technologies.Add(new Technology(TechnologyId.HorsebackRiding, "Horseback Riding", "기마술", 144, null, new List<TechnologyId> { TechnologyId.Archery }));
        technologies.Add(new Technology(TechnologyId.IronWorking, "Iron Working", "철제 기술", 144, null, new List<TechnologyId> { TechnologyId.BronzeWorking }));

        technologies.Add(new Technology(TechnologyId.Mathematics, "Mathematics", "수학", 240, null, new List<TechnologyId> { TechnologyId.Currency }));
        technologies.Add(new Technology(TechnologyId.Construction, "Construction", "건축", 240, null, new List<TechnologyId> { TechnologyId.Masonry, TechnologyId.HorsebackRiding }));
        technologies.Add(new Technology(TechnologyId.Engineering, "Engineering", "공학", 240, null, new List<TechnologyId> { TechnologyId.Wheel }));

        technologies.Add(new Technology(TechnologyId.MilitaryTactics, "Military Tactics", "군사 전술", 360, null, new List<TechnologyId> { TechnologyId.Mathematics }));
        technologies.Add(new Technology(TechnologyId.Apprenticeship, "Apprenticeship", "도제제도", 360, null, new List<TechnologyId> { TechnologyId.Currency, TechnologyId.HorsebackRiding }));
        technologies.Add(new Technology(TechnologyId.Machinery, "Machinery", "기계", 360, null, new List<TechnologyId> { TechnologyId.IronWorking, TechnologyId.Engineering }));

        technologies.Add(new Technology(TechnologyId.Education, "Education", "교육", 468, null, new List<TechnologyId> { TechnologyId.Mathematics, TechnologyId.Apprenticeship }));
        technologies.Add(new Technology(TechnologyId.Stirrups, "Stirrups", "등자", 468, null, new List<TechnologyId> { TechnologyId.HorsebackRiding }));
        technologies.Add(new Technology(TechnologyId.MilitaryEngineering, "Military Engineering", "군사 공학", 468, null, new List<TechnologyId> { TechnologyId.Construction }));
        technologies.Add(new Technology(TechnologyId.Castles, "Castles", "성", 468, null, new List<TechnologyId> { TechnologyId.Construction }));

        technologies.Add(new Technology(TechnologyId.MassProduction, "Mass Production", "대량 생산", 720, null, new List<TechnologyId> { TechnologyId.Education, TechnologyId.MilitaryTactics }));
        technologies.Add(new Technology(TechnologyId.Baking, "Banking", "은행업", 720, null, new List<TechnologyId> { TechnologyId.Education, TechnologyId.Stirrups }));
        technologies.Add(new Technology(TechnologyId.Printing, "Printing", "인쇄술", 720, null, new List<TechnologyId> { TechnologyId.Machinery }));
        technologies.Add(new Technology(TechnologyId.Gunpowder, "Gunpowder", "화약", 720, null, new List<TechnologyId> { TechnologyId.Apprenticeship, TechnologyId.Stirrups, TechnologyId.MilitaryEngineering }));

        technologies.Add(new Technology(TechnologyId.SiegeTactics, "Siege Tactics", "공성 전략", 730, null, new List<TechnologyId> { TechnologyId.Castles }));
        technologies.Add(new Technology(TechnologyId.MetalCasting, "Metal Casting", "주조", 730, null, new List<TechnologyId> { TechnologyId.Gunpowder }));
        technologies.Add(new Technology(TechnologyId.Astronomy, "Astronomy", "천문학", 730, null, new List<TechnologyId> { TechnologyId.Education }));

        ArrayDataSaveText<Technology>(technologies.ToArray(), fileName);
    }

    void LoadTechnologies()
    {
        technologies = new List<Technology>(ArrayDataLoadText<Technology>(fileName));
    }

    public void DataSaveText<T>(T data, string fileName)
    {
        try
        {
            string path = Application.streamingAssetsPath + "/" + fileName;
            string json = JsonUtility.ToJson(data, true);

            if (json.Equals("{}"))
            {
                print("json null");
                return;
            }

            File.WriteAllText(path, json);
            print(json);
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    public void ArrayDataSaveText<T>(T[] data, string fileName)
    {
        try
        {
            string path = Application.streamingAssetsPath + "/" + fileName;
            string json = JsonHelper.ToJson<T>(data, true);

            if (json.Equals("{}"))
            {
                print("json null");
                return;
            }

            File.WriteAllText(path, json);
            print(json);
        }
        catch (Exception e)
        {
            print(e.Message);
        }
    }

    public T DataLoadText<T>(string fileName)
    {
        try
        {
            string path = Application.streamingAssetsPath + "/" + fileName;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                T t = JsonUtility.FromJson<T>(json);
                return t;
            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }

        return default;
    }

    public T[] ArrayDataLoadText<T>(string fileNaem)
    {
        try
        {
            string path = Application.streamingAssetsPath + "/" + fileNaem;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                T[] t = JsonHelper.FromJson<T>(json);

                return t;
            }
        }
        catch (Exception e)
        {
            print(e.Message);
        }

        return default;
    }

}
