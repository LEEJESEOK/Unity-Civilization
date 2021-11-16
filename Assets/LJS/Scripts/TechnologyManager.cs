using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class TechnologyManager : Singleton<TechnologyManager>
{
    public List<Technology> technologies;

    public string fileName;


    // Start is called before the first frame update
    void Start()
    {

        technologies = new List<Technology>();

        // technologies.Add(new Technology(0, "Farm", 25, null, null));
        // technologies.Add(new Technology(1, "Mine", 25, null, null));
        // technologies.Add(new Technology(2, "Pasture", 25, null, null));
        // ArrayDataSaveText<Technology>(technologies.ToArray(), fileName);

        technologies = new List<Technology>(ArrayDataLoadText<Technology>(fileName));
        print(technologies);
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
