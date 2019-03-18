using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour {

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
    public GameObject obsEff_1, obsEff_2, obsEff_3;
    public GameObject fader;

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
                    //mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    mos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) * (1 / 0.522f);
                    if (Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        Variables._meetingTime = DateTime.Now.AddSeconds(gachaTime);
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
                // Effect는 아래쪽 else문에 있습니다.

                if (Input.GetMouseButtonDown(0))
                {
                    //mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, 0f);
                    mos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) * (1 / 0.522f);
                    if (Vector3.Distance(mos, new Vector3(3.5f, -1.3f, 0)) <= 1.3f)
                    {
                        if (Variables.isFirst)
                        {
                            gachaResult = "polaris";
                            Variables.isFirst = false;
                        }
                        else
                        {
                            int probSum = 0;
                            foreach (var key in TouchManager.charProb)
                            {
                                probSum += (int)key.Value;
                            }

                            var Char_desc = TouchManager.charProb.OrderByDescending(p => p.Value);
                            int countProb = 0, i = 0;
                            int gachaNo = UnityEngine.Random.Range(1, probSum + 1);
                            while (countProb < gachaNo)
                            {
                                countProb += (int)(Char_desc.ElementAt(i).Value);
                                gachaResult = Char_desc.ElementAt(i).Key;
                                i++;
                            }
                        }
                        whyTwotime = false;
                        Variables.btnState = 3;
                    }
                }
                break;

            case 3:
                obStart.SetActive(false);
                obText.SetActive(false);
                obFinish.SetActive(true);

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
