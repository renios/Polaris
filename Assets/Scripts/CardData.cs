using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 개별 카드의 데이터입니다.
/// </summary>
public struct CardData
{
    public int Character;
    public string DisplayName;
    public string InternalName;
    public int Rarity;
    public string[] ProgressDesc;
    public string[] LobbyDialog;
}