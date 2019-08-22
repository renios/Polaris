using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaTut : MonoBehaviour {

    [SerializeField]

    private DateTime _nowTime, _nowServerDateTime, _nowLocalDateTime;
    private TimeSpan _duration;

    private TimeSpan _diff;
    private double _diffMillisecs = 0d;
    private double _maxDiff = 15000d;
    private bool _useLocal = false;
    private Vector3 mos = Vector3.zero;

    public GameObject _mn, _sc;
    public GameObject obStart, obText, obFinish;
    public GameObject obBtn;
    public GameObject obsEff_1, obsEff_2, obsEff_3;
    public GameObject fader;
    public GameObject scope;
    public GameObject tutTouch_1, tutArrow_1, tutTouch_2;
    public GameObject tutEff_1, tutEff_2;
    public GameObject tutBg_1, tutBg_2;

    public GameObject tutText_1, tutText_2, tutText_3, tutText_4, tutText_5, tutText_6;

    private int gachaTime = 16;
    private int charIndex = 0;
    public static string gachaResult = null; // Character Name
    private bool whyTwotime = false;

    // Use this for initialization
    private void Start()
    {
        Color tempColor = fader.GetComponent<SpriteRenderer>().color;
        tempColor.a = 0f;
        fader.GetComponent<SpriteRenderer>().color = tempColor;
        
        scope.transform.localPosition = new Vector3(-1.1f, 3.79f, -7f);
    }

    private void FixedUpdate()
    {
        switch(Variables.tutState)
        {
            case 1:
                TouchTut.moveAble = false;
                obBtn.transform.localPosition = new Vector3(3.48f, -0.29f, -5f);
                tutEff_1.SetActive(false);

                tutText_1.SetActive(true);
                tutText_2.SetActive(false);
                tutText_3.SetActive(false);

                tutTouch_1.SetActive(true);
                tutArrow_1.SetActive(true);
                tutTouch_2.SetActive(false);

                tutBg_1.SetActive(true);
                tutBg_2.SetActive(false);

                if (Input.GetMouseButton(0))
                    Variables.tutState = 2;
                break;
            case 2:
                tutText_1.SetActive(false);
                tutText_2.SetActive(true);

                if(Input.GetMouseButton(0))
                {
                    tutTouch_1.SetActive(false);
                    tutArrow_1.SetActive(false);
                    TouchTut.moveAble = true;
                }

                if(Vector2.Distance(scope.transform.localPosition, new Vector2(-0.13f, 4.55f)) <= 0.42f)
                {
                    Variables.tutState = 3;
                    TouchTut.moveAble = false;
                    obBtn.transform.localPosition = new Vector3(3.48f, -0.29f, -7f);
                    tutEff_1.SetActive(true);
                    tutTouch_2.SetActive(true);
                }
                break;
            case 3:
                if (Input.GetMouseButtonDown(0))
                {
                    //mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    mos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) * (1 / 0.522f);
                    if (Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        tutTouch_2.SetActive(false);
                        tutText_2.SetActive(false);
                        tutText_3.SetActive(true);
                        Variables.tutState = 4;
                        Variables._meetingTime = DateTime.Now.AddSeconds(1); // 튜토리얼은 1초 컷
                        Variables.btnState = 1;
                    }
                }
                break;

            case 7:
                TouchTut.moveAble = false;
                tutText_3.SetActive(true);
                tutBg_1.SetActive(false);
                tutBg_2.SetActive(true);
                tutText_3.SetActive(false);
                tutText_4.SetActive(true);
                tutTouch_2.SetActive(false);
                obBtn.transform.localPosition = new Vector3(3.48f, -0.29f, -5f);
                if (Input.GetMouseButtonDown(0))
                {
                    tutText_3.SetActive(false);
                    tutBg_2.SetActive(false);
                    Variables.tutState = 8;
                    TouchTut.moveAble = true;
                }
                break;

            case 8:
                if (Input.GetMouseButtonDown(0))
                {
                    //mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    mos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) * (1 / 0.522f);
                    if (Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        tutTouch_2.SetActive(false);
                        tutText_4.SetActive(false);
                        tutText_5.SetActive(true);
                        Variables.tutState = 9;
                        Variables._meetingTime = DateTime.Now.AddSeconds(11);
                        Variables.btnState = 1;
                    }
                }
                break;

            default:
                break;
        }
        
        switch(Variables.btnState)
        {
            case 0: // 관측시작
                obBtn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha_New/obs_mainbutton");
                obStart.SetActive(true);
                obText.SetActive(false);
                obFinish.SetActive(false);
                obsEff_1.SetActive(false);
                obsEff_2.SetActive(false);
                obsEff_3.SetActive(false);
                break;

            case 1: // 관측중
                obBtn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha_New/obs_mainbutton");
                obStart.SetActive(false);
                obText.SetActive(true);
                obFinish.SetActive(false);
                obsEff_1.SetActive(true);
                obsEff_2.SetActive(false);
                obsEff_3.SetActive(false);

                if(Input.GetKeyDown(KeyCode.Space))
                    Variables._meetingTime = DateTime.Now.AddSeconds(1);

                Timer();
                break;

            case 2: // 관측완료
                obBtn.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha_New/obs_circle_completed");
                obStart.SetActive(false);
                obText.SetActive(false);
                obFinish.SetActive(true);
                obsEff_1.SetActive(false);
                obsEff_2.SetActive(true);
                obsEff_3.SetActive(false);

                if(Variables.tutState == 9)
                {
                    tutText_5.SetActive(false);
                    tutText_6.SetActive(true);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    //mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    mos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) * (1 / 0.522f);
                    if (mos.y > -1.44f || Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        if(Variables.tutState == 4)
                        {
                            GachaManager.gachaResult = "polaris";
                            Variables.isFirst = false;
                        }
                        else if(Variables.tutState == 9)
                        {
                            String[] canSD = new String[] { "thuban", "catseye", "vega", "rescha", "melik", "acher", "rigeleuse" };
                            int randSD = (int)UnityEngine.Random.Range(0f, 7f);
                            GachaManager.gachaResult = canSD[randSD];
                        }
                    }

                    Variables.btnState = 3;
                }
                break;

            case 3:
                obStart.SetActive(false);
                obText.SetActive(false);
                obFinish.SetActive(true);

                if (Variables.tutState == 4)
                {
                    Variables.returnSceneName = "MainTut";
                }
                else if(Variables.tutState == 9)
                {
                    Variables.returnSceneName = "MainScene";
                }
                StartCoroutine(GachaFadeOut(1.5f)); // 1.5f
                
                break;

            default:
                break;
        }
    }

    IEnumerator GachaFadeOut(float fadeOutTime)
    {
        SpriteRenderer sr = fader.GetComponent<SpriteRenderer>();
        Color tempColor = sr.color;

        obsEff_1.SetActive(false);
        obsEff_2.SetActive(false);
        obsEff_3.SetActive(true);

        while (tempColor.a < 1f)
        {
            tempColor.a += Time.deltaTime / fadeOutTime;

            if (tempColor.a >= 1f)
                tempColor.a = 1f;

            sr.color = tempColor;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Variables.tutState = 5;
        Variables.btnState = 0;
        SceneManager.LoadScene("GachaResult");
    }

    public void Timer()
    {
        _nowTime = DateTime.Now;
        _diff = Variables._meetingTime - _nowTime;

        if (_diff.Seconds <= 0f)
        {
            obsEff_1.SetActive(false);
            obsEff_2.SetActive(true);
            obsEff_3.SetActive(false);
            Variables.btnState = 2;
        }

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
