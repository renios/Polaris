using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using LitJson;

// Last edited 2020-01-18
[System.Serializable]
public class SaveData
{
    public static SaveData Now { get; private set; }

    public int charVersion;
    public Dictionary<int, CharacterData> charData;

    public int starlight, memorialPiece;
    public int[] storeUpgradeLevel;

    public bool tutorialFinished;
    public int tutorialStep;

    public int observeSkyLevel;

    public SaveData()
    {
        charData = new Dictionary<int, CharacterData>();

        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        charVersion = charGroup.Version;
        foreach (var data in charGroup.Characters)
            charData.Add(data.CharNumber, data);

        starlight = 1000;
        memorialPiece = 1;
        storeUpgradeLevel = new[] { 2, 0, 0 };
        tutorialFinished = false;
        tutorialStep = 1;
        observeSkyLevel = -1;
    }

    public static void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/save"))
        {
            var reader = new FileStream(Application.persistentDataPath + "/save", FileMode.Open);
            var formatter = new BinaryFormatter();
            Now = (SaveData)formatter.Deserialize(reader);
            reader.Close();

            Now.CheckCharacterData();
        }
        else
        {
            Now = new SaveData();
            Save();
        }
    }

    public static void Save()
    {
        var writer = new FileStream(Application.persistentDataPath + "/save", FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(writer, Now);
        writer.Close();
    }

    public static void Delete()
    {
        if (File.Exists(Application.persistentDataPath + "/save"))
            File.Delete(Application.persistentDataPath + "/save");
    }

    public void CheckCharacterData()
    {
        // 처음 캐릭터 데이터를 일단 읽어옵니다.
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);

        // 버전이 높으면(갱신되었으면), 정적 데이터들을 덮어씌웁니다.
        if(charGroup.Version > charVersion)
        {
            foreach(var data in charGroup.Characters)
            {
                if (!charData.ContainsKey(data.CharNumber))
                    charData.Add(data.CharNumber, data);
                else
                    charData[data.CharNumber].AppendFrom(data);
            }
        }
    }
}