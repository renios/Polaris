using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using LitJson;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private SaveData curSaveData;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Initialize();
    }

    public void Initialize()
    {
        var constelRaw = Resources.Load<TextAsset>("Data/Constels");
        var constelGroup = JsonMapper.ToObject(constelRaw.text);

        Variables.Constels = new Dictionary<string, ConstelData>();
        foreach (JsonData data in constelGroup["constels"])
        {
            var newConstel = new ConstelData((string)data["key"], (string)data["name"]);
            Variables.Constels.Add(newConstel.InternalName, newConstel);
        }
    }

    public void CreateGame()
    {
        curSaveData = new SaveData();
        curSaveData.Create();
    }

    public void LoadGame()
    {
        var reader = new FileStream(Application.persistentDataPath + "/save", FileMode.Open);
        var formatter = new BinaryFormatter();
        curSaveData = (SaveData)formatter.Deserialize(reader);
        reader.Close();

        curSaveData.Load();
    }

    public void SaveGame()
    {
        curSaveData.Save();

        var writer = new FileStream(Application.persistentDataPath + "/save", FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(writer, curSaveData);
        writer.Close();
    }

    // FOR DEBUG PURPOSE ONLY
    public void DeleteGame()
    {
        curSaveData = null;
        File.Delete(Application.persistentDataPath + "/save");
    }
}