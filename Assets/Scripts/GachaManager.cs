using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour {

    [SerializeField]

    private DateTime _nowTime, _nowServerDateTime, _nowLocalDateTime;
    private TimeSpan _duration;

    private TimeSpan _diff;
    private double _diffMillisecs = 0d;
    private double _maxDiff = 15000d;
    private bool _useLocal = false;
    private DateTime _meetingTime;
    private Vector3 mos = Vector3.zero;

    public GameObject _mn, _sc;
    public GameObject obStart, obText, obFinish;
    public GameObject obsEff_1, obsEff_2, obsEff_3;

    // Use this for initialization
    private void Start()
    {

    }

    private void FixedUpdate()
    {
        switch(Variables.btnState)
        {
            case 0: // 관측시작
                obStart.SetActive(true);
                obText.SetActive(false);
                obFinish.SetActive(false);
                obsEff_1.SetActive(true);
                obsEff_2.SetActive(false);
                obsEff_3.SetActive(false);

                if (Input.GetMouseButtonDown(0))
                {
                    mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    if(Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        _meetingTime = DateTime.Now.AddSeconds(16);
                        Variables.btnState = 1;
                        TouchManager.moveAble = false;
                    }
                }
                break;

            case 1: // 관측중
                obStart.SetActive(false);
                obText.SetActive(true);
                obFinish.SetActive(false);
                obsEff_1.SetActive(false);
                obsEff_2.SetActive(true);
                obsEff_3.SetActive(false);

                Timer();
                break;

            case 2: // 관측완료
                obStart.SetActive(false);
                obText.SetActive(false);
                obFinish.SetActive(true);
                obsEff_1.SetActive(false);
                obsEff_2.SetActive(true);
                obsEff_3.SetActive(false);

                if (Input.GetMouseButtonDown(0))
                {
                    mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    if (Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        Variables.btnState = 0;
                        TouchManager.moveAble = true;
                    }
                }
                break;

            default:
                break;
        }
    }

    public void Timer()
    {
        _nowTime = DateTime.Now;
        _diff = _meetingTime - _nowTime;

        if (_diff.Seconds <= 0f)
            Variables.btnState = 2;

        if (_diff.Minutes / 10 != 0)
            _mn.GetComponent<TextMesh>().text = _diff.Minutes.ToString();
        else
            _mn.GetComponent<TextMesh>().text = "0" + _diff.Minutes.ToString();

        if (_diff.Seconds / 10 != 0)
            _sc.GetComponent<TextMesh>().text = _diff.Seconds.ToString();
        else
            _sc.GetComponent<TextMesh>().text = "0" + _diff.Seconds.ToString();

    }

    #region NTPTIME

    //NTP time 을 NIST 에서 가져오기
    // 4초 이내에 한번 이상 요청 하면, ip가 차단됩니다.

    public static DateTime GetDummyDate()
    {
        DateTime now = DateTime.Now.Add(System.TimeSpan.FromHours(-9));
        return now; //to check if we have an online date or not.
    }

    public static DateTime GetNISTDate()
    {
        System.Random ran = new System.Random(DateTime.Now.Millisecond);
        DateTime date = GetDummyDate();
        string serverResponse = string.Empty;

        // NIST 서버 목록
        string[] servers = new string[] {
            "time.bora.net",
            //"time.nuri.net",
            //"ntp.kornet.net",
            //"time.kriss.re.kr",
            //"time.nist.gov",
            //"maths.kaist.ac.kr",
            "nist1-ny.ustiming.org",
            "time-a.nist.gov",
            "nist1-chi.ustiming.org",
            "time.nist.gov",
            "ntp-nist.ldsbc.edu",
            "nist1-la.ustiming.org"
        };

        // 너무 많은 요청으로 인한 차단을 피하기 위해 한 서버씩 순환한다. 5번만 시도한다.
        for (int i = 0; i < 5; i++)
        {
            try
            {
                // StreamReader(무작위 서버)
                StreamReader reader = new StreamReader(new System.Net.Sockets.TcpClient(servers[ran.Next(0, servers.Length)], 13).GetStream());
                serverResponse = reader.ReadToEnd();
                reader.Close();

                // 시그니처가 있는지 확인한다.
                if (serverResponse.Length > 47 && serverResponse.Substring(38, 9).Equals("UTC(NIST)"))
                {
                    // 날짜 파싱
                    int jd = int.Parse(serverResponse.Substring(1, 5));
                    int yr = int.Parse(serverResponse.Substring(7, 2));
                    int mo = int.Parse(serverResponse.Substring(10, 2));
                    int dy = int.Parse(serverResponse.Substring(13, 2));
                    int hr = int.Parse(serverResponse.Substring(16, 2));
                    int mm = int.Parse(serverResponse.Substring(19, 2));
                    int sc = int.Parse(serverResponse.Substring(22, 2));

                    if (jd > 51544)
                        yr += 2000;
                    else
                        yr += 1999;

                    date = new DateTime(yr, mo, dy, hr, mm, sc);

                    // Exit the loop
                    break;
                }
            }
            catch (Exception e)
            {
                /* 아무것도 하지 않고 다음 서버를 시도한다. */
            }
        }
        return date;
    }
    #endregion
}
