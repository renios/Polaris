﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public int CharNumber;
    public string[] ConstelKey;
    public double[] ConstelWeight;

    public string Name;
    public string InternalName;
    public bool Observable;

    public string LuxText;
    public double LuxValue;
    public string LYDistance;
    public string Description;

    public CharacterStoryData[] ChapterInfo;

    public bool Observed;
    public int Favority;
    public int StoryProgress;
    public string LastReapDate;
}

public struct CharacterDataCore
{
    public int CharNumber;
    public string[] ConstelKey;
    public double[] ConstelWeight;

    public string Name;
    public string InternalName;
    public bool Observable;

    public string LuxText;
    public double LuxValue;
    public string LYDistance;
    public string Description;

    public CharacterStoryData[] ChapterInfo;

    public static implicit operator CharacterData(CharacterDataCore c)
    {
        var d = new CharacterData()
        {
            CharNumber = c.CharNumber,
            ConstelKey = c.ConstelKey,
            ConstelWeight = c.ConstelWeight,
            Name = c.Name,
            InternalName = c.InternalName,
            Observable = c.Observable,
            LuxText = c.LuxText,
            LuxValue = c.LuxValue,
            LYDistance = c.LYDistance,
            Description = c.Description,
            ChapterInfo = c.ChapterInfo
        };
        return d;
    }

    public void AppendTo(ref CharacterData existing)
    {
        existing.ConstelKey = ConstelKey;
        existing.ConstelWeight = ConstelWeight;
        existing.Name = Name;
        existing.InternalName = InternalName;
        existing.Observable = Observable;
        existing.LuxText = LuxText;
        existing.LuxValue = LuxValue;
        existing.LYDistance = LYDistance;
        existing.Description = Description;
        existing.ChapterInfo = ChapterInfo;
    }
}

public struct CharacterDataGroup
{
    public int Version;
    public CharacterDataCore[] Characters;
}

[System.Serializable]
public struct CharacterStoryData
{
    public string Header;
    public string Description;
    public string[] Additional;
}