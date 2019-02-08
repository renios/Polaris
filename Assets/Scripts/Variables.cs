using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Variables
{
    public static Dictionary<int, CharacterData> Characters;

    public static readonly int[] FavorityThreshold = { 30, 70, 120, 180, 250 };

    #region Variables for dialogue scene
    public static int DialogCharIndex;
    public static int DialogCardIndex;
    public static int DialogChapterIndex;
    public static string DialogAfterScene;
    #endregion
}