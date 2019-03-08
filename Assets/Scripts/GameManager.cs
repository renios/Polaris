using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        LoadData();
    }

    public void LoadData()
    {
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        var constelRaw = Resources.Load<TextAsset>("Data/Constels");
        var constelGroup = JsonMapper.ToObject(constelRaw.text);

        Variables.Characters = new Dictionary<int, CharacterData>();
        foreach (CharacterDataCore data in charGroup.Characters)
        {
            Variables.Characters.Add(data.CharNumber, data);
        }

        Variables.Constels = new Dictionary<string, ConstelData>();
        foreach (JsonData data in constelGroup["constels"])
        {
            var newConstel = new ConstelData((string)data["key"], (string)data["name"]);
            Variables.Constels.Add(newConstel.InternalName, newConstel);
        }
    }
}