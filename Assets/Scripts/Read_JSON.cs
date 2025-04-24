using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSON_operations : MonoBehaviour
{

    const string file_location = "Assets/Data/Variables/";


    public static string Read_file(string path)
    {
        string content = "";
        using (StreamReader sr = new StreamReader(file_location + path))
        {
            content = sr.ReadToEnd();
            GameLog.Message(content);
        }
        return content;
    }

    public static List<T> From<T>(string data)
    { 
        GameLog.Message(data);
        Wrap<T> wrapper = JsonUtility.FromJson<Wrap<T>>(data);
        GameLog.Message($"{wrapper.items.Count}");
        return wrapper.items;
    }

    [Serializable]
    private class Wrap<T>
    {
        public List<T> items;
    }
}
