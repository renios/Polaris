using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Variables
{
    public static bool HasSave { get { return File.Exists(Application.persistentDataPath + "/save"); } }
    public static int[] FavorityThreshold { get { return values.accumFavTable; } }

    #region Data from SaveData.Now
    public static Dictionary<int, CharacterData> Characters { get { return SaveData.Now.charData; } }
    public static int[] StoreUpgradeLevel { get { return SaveData.Now.storeUpgradeLevel; } } // 0: 망원경 성능 레벨, 1: 망원경 멀티-관측 레벨, 2: 로비 캐릭터 배치 수 레벨
    public static int Starlight 
    { 
        get { return SaveData.Now.starlight; }
        set { SaveData.Now.starlight = value; }
    }
    public static int ObserveSkyLevel
    {
        get { return SaveData.Now.observeSkyLevel; }
        set { SaveData.Now.observeSkyLevel = value; }
    }
    public static int TutorialStep
    {
        get { return SaveData.Now.tutorialStep; }
        set { SaveData.Now.tutorialStep = value; }
    }
    public static bool TutorialFinished
    {
        get { return SaveData.Now.tutorialFinished; }
        set { SaveData.Now.tutorialFinished = value; }
    }
    #endregion

    public static Values values;

    public static Dictionary<string, ConstelData> Constels;     // 별자리에 대한 데이터

    public static bool CameraMove;

    #region Variables for dialogue scene
    public static bool IsDialogAppended;
    public static string DialogAfterScene;      // 대화 씬이 종료된 다음 씬의 이름
    #endregion

    public static string GetCharacterRootFolder(int charIndex)
    {
        return "Characters/" + Characters[charIndex].InternalName + "/";
    }

    public static int GetStoreValue(int speci, int index)
    {
        switch(speci)
        {
            case 0:
                return values.scopeTimeTable[index];
            case 1:
                return values.scopeCharCount[index];
            case 2:
                return values.lobbyCharCount[index];
        }
        return -1;
    }

    public static int GetStoreReqMoney(int speci, int index)
    {
        switch (speci)
        {
            case 0:
                return values.scopeTimeUpCost[index];
            case 1:
                return values.scopeCharUpCost[index];
            case 2:
                return values.lobbyCharUpCost[index];
        }
        return -1;
    }

    public static int GetStoreMaxLevel(int speci)
    {
        switch (speci)
        {
            case 0:
                return values.scopeTimeTable.Length;
            case 1:
                return values.scopeCharCount.Length;
            case 2:
                return values.lobbyCharCount.Length;
        }
        return -1;
    }
}