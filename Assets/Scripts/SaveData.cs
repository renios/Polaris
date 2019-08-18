using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

// Last edited 2019-07-21 by thiEFcat
[System.Serializable]
public class SaveData
{
    public int CharVersion;
    public List<CharacterData> Characters;
    public int Starlight;
    public int[] StoreUpgradeLevel;

    public void Load()
    {
        // 처음 캐릭터 데이터를 일단 읽어옵니다.
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        Variables.CharacterVersion = charGroup.Version;
        Variables.isFirst = false;

        Variables.Characters = new Dictionary<int, CharacterData>();
        if (Variables.CharacterVersion > CharVersion) // 만약 현재 세이브 파일의 버전이 낮다면 == 캐릭터 목록이 업데이트 되었었다면...
        {
            // 읽어온 캐릭터 데이터를 통해 Variables.Characters를 구성한 뒤, 이 객체가 가지고 있는 데이터를 반복문으로 적용시킵니다.
            // 적용시킬 때, 캐릭터나 카드의 위치 변경은 이루어지지 않았다고 가정하였습니다.
            foreach (var data in charGroup.Characters)
                Variables.Characters.Add(data.CharNumber, data);
            foreach(var curData in Characters)
            {
                if(Variables.Characters.ContainsKey(curData.CharNumber))
                {
                    for (int i = 0; i < curData.Cards.Count; i++)
                    {
                        Variables.Characters[curData.CharNumber].Cards[i].Observed = curData.Cards[i].Observed;
                        Variables.Characters[curData.CharNumber].Cards[i].Favority = curData.Cards[i].Favority;
                        Variables.Characters[curData.CharNumber].Cards[i].StoryProgress = curData.Cards[i].StoryProgress;
                    }
                }
            }
        }
        else // 그렇지 않았다면...
        {
            // 이 객체가 가지고 있는 데이터를 통해 Variables.Characters를 구성합니다.
            foreach (var data in Characters)
            { Variables.Characters.Add(data.CharNumber, data); }
        }

        // 기타 변수들을 동기화시켜줍니다.
        Variables.Starlight = Starlight;
        Variables.StoreUpgradeLevel = StoreUpgradeLevel;
    }

    public void Create()
    {
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        Variables.CharacterVersion = charGroup.Version;

        Variables.Characters = new Dictionary<int, CharacterData>();
        foreach (var data in charGroup.Characters)
            Variables.Characters.Add(data.CharNumber, data);

        Starlight = 0;
        StoreUpgradeLevel = new[] { 1, 1, 1 };
        Variables.Starlight = Starlight;
        Variables.StoreUpgradeLevel = StoreUpgradeLevel;

        Variables.isFirst = true;
    }

    public void Save()
    {
        CharVersion = Variables.CharacterVersion;

        Characters = new List<CharacterData>();
        foreach (var item in Variables.Characters)
            Characters.Add(item.Value);

        Starlight = Variables.Starlight;
        StoreUpgradeLevel = Variables.StoreUpgradeLevel;
    }
}