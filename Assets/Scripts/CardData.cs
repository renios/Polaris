using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개별 카드의 데이터입니다.
/// </summary>
public struct CardData
{
    public string Subname;
    public string InternalSubname;
    public int Rarity;
    public string[] ChapterDesc;

    public bool Observed;
    public int Favority;
    public int StoryProgress;
}

public struct CardDataCore
{
    public string Subname;
    public string InternalSubname;
    public int Rarity;
    public string[] ChapterDesc;

    public static implicit operator CardData(CardDataCore c)
    {
        return new CardData
        {
            Subname = c.Subname,
            InternalSubname = c.InternalSubname,
            Rarity = c.Rarity,
            ChapterDesc = c.ChapterDesc.Clone() as string[],
            Observed = true,
            Favority = 0,
            StoryProgress = 0,
        };
    }
}