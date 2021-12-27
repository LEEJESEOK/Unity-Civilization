using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


[Serializable]
public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] data;
    }

    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.data;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.data = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }


    public static void DataSaveText<T>(T data, string fileName)
    {
        try
        {
            string path = Application.streamingAssetsPath + "/" + fileName;
            string json = JsonUtility.ToJson(data, true);

            if (json.Equals("{}"))
            {
                Console.WriteLine("json null");
                return;
            }

            File.WriteAllText(path, json);
            Console.WriteLine(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static void ArrayDataSaveText<T>(T[] data, string fileName)
    {
        try
        {
            string path = Application.streamingAssetsPath + "/" + fileName;
            string json = JsonHelper.ToJson<T>(data, true);

            if (json.Equals("{}"))
            {
                Console.WriteLine("json null");
                return;
            }

            File.WriteAllText(path, json);
            Console.WriteLine(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public static T DataLoadText<T>(string fileName)
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
            Console.WriteLine(e.Message);
        }

        return default;
    }

    public static T[] ArrayDataLoadText<T>(string fileNaem)
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
            Console.WriteLine(e.Message);
        }

        return default;
    }
}