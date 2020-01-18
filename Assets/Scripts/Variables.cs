using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Variables
{
    public static bool HasSave
    {
        get { return File.Exists(Application.persistentDataPath + "/save"); }
    }

    public static Dictionary<string, ConstelData> Constels;     // 별자리에 대한 데이터
    public static Dictionary<int, CharacterData> Characters;    // 캐릭터에 대한 데이터
    public static int CharacterVersion;
    public static int Starlight;
    public static int[] StoreUpgradeLevel; // 0: 망원경 성능 레벨, 1: 망원경 멀티-관측 레벨, 2: 로비 캐릭터 배치 수 레벨

    public static int ObserveSkyLevel; // 하늘 개방 단계

    public static bool isTutorialFinished = false; // 튜토리얼이 끝까지 진행 되었는지?
    public static int tutState = 1; // 튜토리얼 진행 상태

    public static readonly int[] FavorityThreshold = { 2, 3, 4, 5, 6 };     // 캐릭터 호감도 문턱에 관한 배열 30 70 120 180 250

    // 상점 처리 및 레벨에 따른 데이터 테이블입니다.
    public static readonly int[][] StoreUpgradeValue = new int[][]
    {
        new[]{ 0, 1, 30, 180, 720 },    // 망원경 최대 관측 시간. 단위는 분. 표시는 '최대 X분'
        new[]{ 0, 1, 2, 3 }   // 망원경 최대 동시 관측 천체 수. 표시는 '최대 X명'
    };
    public static readonly int[][] StoreUpgradeMoney = new int[][]
    {
        new[]{ 0, 1000, 3000, 10000 },
        new[]{ 0, 5000, 15000 }
    };

    public static bool CameraMove;

    public static bool isFirst;
    public static string returnSceneName;

    #region Variables for dialogue scene
    public static int DialogCharIndex;          // 대화 씬에서 참조할 캐릭터 번호
    public static int DialogCardIndex;          // 대화 씬에서 참조할 캐릭터 내 종류 번호
    public static int DialogChapterIndex;       // 대화 씬에서 참조할 해당 캐릭터 종류의 스토리 번호
    public static bool IsDialogAppended;
    public static string DialogAfterScene;      // 대화 씬이 종료된 다음 씬의 이름
    #endregion
}