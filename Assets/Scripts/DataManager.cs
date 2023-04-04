using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public string userName;
    public int bestScore;

    public List<SaveData> data = new List<SaveData>();
    [SerializeField] private string fileName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveJsonData<T>(List<T> toSave, string fileName)
    {
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
    }

    public List<T> ReadJsonData<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }

    private string GetPath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    private void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private string ReadFile(string path)
    {
        if(File.Exists(path)){
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
    }
        return"";
    }

    public void LoadData()
    {
        if(data != null)
                data = ReadJsonData<SaveData>(fileName);
    }

    public void ClearData()
    {
        data.Clear();
        SaveJsonData<SaveData>(data, fileName);
    }

    private bool _canFindUser;
    public void SaveUserData(string userName, int points)
    {
        if (data.Count == 0)
        {
            data.Add(new SaveData(userName, points));
        }
        else
        {
            foreach (var memberData in data)
            {
                if (memberData.userName == userName)
                {
                    memberData.bestScore = points;
                    memberData.userName = userName;
                    _canFindUser = true;
                    break;
                }
            }

            if (!_canFindUser)
            {
                data.Add(new SaveData(userName, points));
            }   
        }
       
        _canFindUser = false;
        SaveJsonData<SaveData>(data, fileName);
    }

    public bool isUserHasExist;
    public SaveData memberData;
    public SaveData GetUserHasExist(string userName)
    {
        foreach (var memberData in data)
        {
            if (memberData.userName == userName)
            {
                isUserHasExist = true;
                return this.memberData = memberData;
            }
        }

        isUserHasExist = false;
        return null;
    }
    public void GetBestScore()
    {
        if (isUserHasExist)
        {
            bestScore = memberData.bestScore;
            userName = memberData.userName;
        }
        else
        {
            if (data.Count > 0)
            {
                bestScore = data[data.Count - 1].bestScore;
                userName = data[data.Count - 1].userName;
            }
        }
    }
    public void SaveLastUserName()
    {
        SaveData data = new SaveData(userName, bestScore);
        data.userName = userName;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath +"/savefile.json" ,json);
    }

    public void LoadLastUserName()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            userName = data.userName;
        }
    }
    
    // public void SaveUserData(int bestScore)
    // {
    //     SaveData data = new SaveData();
    //     data.bestScore = bestScore;
    //     data.userName = userName;
    //     string json = JsonUtility.ToJson(data);
    //     File.WriteAllText(Application.persistentDataPath +"/savefile.json" ,json);
    // }
    
    // public void SaveUserName()
    // {
    //     SaveData data = new SaveData();
    //     data.userName = userName;
    //     string json = JsonUtility.ToJson(data);
    //     File.WriteAllText(Application.persistentDataPath +"/savefile.json" ,json);
    // }
    
    // public void LoadData()
    // {
    //     string path = Application.persistentDataPath + "/savefile.json";
    //     if (File.Exists(path))
    //     {
    //         string json = File.ReadAllText(path);
    //         SaveData data = JsonUtility.FromJson<SaveData>(json);
    //         
    //         userName = data.userName;
    //         bestScore = data.bestScore;
    //     }
    // }
    
    // public void LoadUserName()
    // {
    //     string path = Application.persistentDataPath + "/savefile.json";
    //     if (File.Exists(path))
    //     {
    //         string json = File.ReadAllText(path);
    //         SaveData data = JsonUtility.FromJson<SaveData>(json);
    //     }
    // }
}
 
[Serializable]
public class SaveData
{
    public string userName;
    public int bestScore;

    public SaveData(string name, int points)
    {
        userName = name;
        bestScore = points;
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}