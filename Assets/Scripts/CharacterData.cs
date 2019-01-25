using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// '캐릭터'의 데이터입니다. (개별 카드 데이터와는 다릅니다)
/// </summary>
public class CharacterData : CharacterDataCore
{
    public bool Observed;
}

public class CharacterDataCore
{
    public int CharNumber;
    public string Name;
    public string InternalName;
    public string Lux;
    public string LYDistance;
    public string Description;
    public List<CardData> Cards;
}


public class CharacterDataGroup
{
    public int Version;
    public List<CharacterDataCore> Characters;
}