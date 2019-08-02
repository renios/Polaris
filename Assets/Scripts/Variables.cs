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

    public static readonly int[] FavorityThreshold = { 2, 3, 4, 5, 6 };     // 캐릭터 호감도 문턱에 관한 배열 30 70 120 180 250

    public static bool CameraMove;
    public static bool isTutorialFinished = false;

    public static int btnState = 0; // 0: 관측시작, 1: 관측중, 2: 관측완료, 3: 가챠결과 확인
    public static bool isFirst = true;
    public static DateTime _meetingTime;
    public static Vector3 scopePos = new Vector3(1.97f, 4.27f, -1f);

    #region Variables for dialogue scene
    public static int DialogCharIndex;          // 대화 씬에서 참조할 캐릭터 번호
    public static int DialogCardIndex;          // 대화 씬에서 참조할 캐릭터 내 종류 번호
    public static int DialogChapterIndex;       // 대화 씬에서 참조할 해당 캐릭터 종류의 스토리 번호
    public static string DialogAfterScene;      // 대화 씬이 종료된 다음 씬의 이름
    #endregion
}