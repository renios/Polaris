using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int DataVersion;
    public List<CharacterData> Characters;

    public void CheckUpdate()
    {
        // TODO: Initial 데이터를 읽고, 버전을 확인한 후 갱신시킵니다.
        //       갱신 시, 발견 여부, 호감도 등의 데이터는 따로 빼내어 저장시킵니다.
    }

    public void ApplyCharacter()
    {
        foreach (CharacterData data in Characters)
            Variables.Characters.Add(data.CharNumber, data);
    }
}