using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// '캐릭터'의 데이터입니다. (개별 카드 데이터와는 다릅니다)
/// </summary>
[System.Serializable]
public class CharacterData
{
    public int CharNumber;
    public int ConstelGroupIndex;
    public string[] ConstelKey;
    public double[] ConstelWeight;
    public string Name;
    public string InternalName;
    public string Lux;
    public double LuxValue;
    public string LYDistance;
    public string Description;

    public List<CardData> Cards;
}

public struct CharacterDataCore
{
    public int CharNumber;
    public int ConstelGroupIndex;
    public string[] ConstelKey;
    public double[] ConstelWeight;
    public string Name;
    public string InternalName;
    public string Lux;
    public double LuxValue;
    public string LYDistance;
    public string Description;
    public CardDataCore[] Cards;

    public static implicit operator CharacterData(CharacterDataCore c)
    {
        var d = new CharacterData()
        {
            CharNumber = c.CharNumber,
            ConstelGroupIndex = c.ConstelGroupIndex,
            ConstelKey = c.ConstelKey,
            ConstelWeight = c.ConstelWeight,
            Name = c.Name,
            InternalName = c.InternalName,
            Lux = c.Lux,
            LuxValue = c.LuxValue,
            LYDistance = c.LYDistance,
            Description = c.Description
        };
        d.Cards = new List<CardData>();
        for(int i = 0; i < c.Cards.Length; i++)
            d.Cards.Add(c.Cards[0]);
        return d;
    }
}


public struct CharacterDataGroup
{
    public int Version;
    public CharacterDataCore[] Characters;
}