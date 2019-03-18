using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개별 카드의 데이터입니다.
/// </summary>
[System.Serializable]
public class CardData
{
    public string Subname;
    public string InternalSubname;
    public int Rarity;
    public bool Observable;
    public CardStoryData[] ChapterInfo;

    public bool Observed;
    public int Favority;
    public int StoryProgress;
}

public class CardDataCore
{
    public string Subname;
    public string InternalSubname;
    public int Rarity;
    public bool Observable;
    public CardStoryData[] ChapterInfo;

    public static implicit operator CardData(CardDataCore c)
    {
        return new CardData
        {
            Subname = c.Subname,
            InternalSubname = c.InternalSubname,
            Rarity = c.Rarity,
            Observable = c.Observable,
            ChapterInfo = c.ChapterInfo.Clone() as CardStoryData[],
            //Observed = c.Observable,
            Observed = false,
            Favority = 0,
            StoryProgress = 0,
        };
    }
}

[System.Serializable]
public class CardStoryData
{
    public string Header;
    public string Description;
    public string[] Additional;
}