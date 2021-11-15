using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TechnologyManager : Singleton<TechnologyManager>
{
    public bool isLoadJson;
    public string fileName;

    public List<Technology> technologies;




    // Start is called before the first frame update
    void Start()
    {

        technologies = new List<Technology>();

        #region save
        if(isLoadJson == false)
        {
            technologies.Add(new Technology(211, "Pottery", 25, null, null));
            technologies.Add(new Technology(212, "Animal Husbandry", 25, null, null));
            technologies.Add(new Technology(213, "Mining", 25, null, null));

            technologies.Add(new Technology(223, "Irrigation", 50, null, new int[]{0}));
            technologies.Add(new Technology(224, "Writing", 50, null, new int[]{0}));
            technologies.Add(new Technology(225, "Archery", 50, null, new int[]{1}));

            technologies.Add(new Technology(231, "Masonry", 80, null, null));
            technologies.Add(new Technology(232, "Bronze Working", 80, null, null));
            technologies.Add(new Technology(233, "Wheel", 80, null, null));

            technologies.Add(new Technology(312, "Currency", 144, null, null));
            technologies.Add(new Technology(313, "Horseback Riding", 144, null, null));
            technologies.Add(new Technology(314, "Iron Working", 144, null, null));

            technologies.Add(new Technology(322, "Mathematics", 240, null, null));
            technologies.Add(new Technology(323, "Construction", 240, null, null));
            technologies.Add(new Technology(324, "Engineering", 240, null, null));

            technologies.Add(new Technology(411, "Military Tactics", 360, null, null));
            technologies.Add(new Technology(412, "Apprenticeship", 360, null, null));
            technologies.Add(new Technology(413, "Machinery", 360, null, null));

            technologies.Add(new Technology(421, "Education", 468, null, null));
            technologies.Add(new Technology(422, "Stirrups", 468, null, null));
            technologies.Add(new Technology(423, "Military Engineering", 468, null, null));
            technologies.Add(new Technology(424, "Castle", 468, null, null));

            technologies.Add(new Technology(512, "Mass Production", 720, null, null));
            technologies.Add(new Technology(513, "Banking", 720, null, null));
            technologies.Add(new Technology(514, "Printing", 720, null, null));
            technologies.Add(new Technology(515, "Gunpowder", 720, null, null));

            ArrayDataSaveText<Technology>(technologies.ToArray(), fileName);
        }
        #endregion

        #region load
        if (isLoadJson)
        {
            // technologies = new List<Technology>(ArrayDataLoadText<Technology>(fileName));
            // print(technologies);
        }
        #endregion
    }

    // Update is called once per frame
    void Update()
    {

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
