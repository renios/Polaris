using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CharacterStarlight클래스는 유니티 에디터 상에서 직접 컴포넌트로 붙이지 않고, LobbyManager클래스에서 SD 캐릭터를 생성할 때 AddComponenet<CharacterStarlight>를 통해 붙여줍니다.
//캐릭터에 따른 별빛 생산 효율 등 개체별로 다른 값을 가지는 것들은, 추가적인 기획이 필요해 보입니다.
public class CharacterStarlight : MonoBehaviour
{
    public float sps = 1; //이 값(초)당 별빛 생성
    public float criteriaPoint = 20; //이 값보다 쌓인 별빛이 많으면 말풍선 띄움

    private DateTime lastDate;
    private TimeSpan span;
    private bool overPoint = false; //수확 가능한 포인트 이상 쌓였는가
    private Transform starBalloon;
    private int characterIndex;
    private int cardIndex;

    /// <summary>
    /// 두 DateTime의 차 TimeSpan을 이용하여 별빛의 양을 계산합니다. "yyyy-MM-dd-hh-mm-ss"의 string 형식으로 CardData.LastReapDate 등에 저장합니다.
    /// </summary>
    public string LastDate {
        get {
            return lastDate.Year.ToString() + "-" + lastDate.Month.ToString() + "-" + lastDate.Day.ToString() + "-" + lastDate.Hour.ToString() + "-" + lastDate.Minute.ToString() + "-" + lastDate.Second.ToString();
        }
        set {
            if (value != null && value != "")
            {
                string[] s = value.Split('-');
                lastDate = new DateTime(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]), int.Parse(s[4]), int.Parse(s[5]));
            }
            else
            {
                lastDate = DateTime.Now;
                Variables.Characters[characterIndex].Cards[cardIndex].LastReapDate = this.LastDate;
                GameManager.Instance.SaveGame();
            }
        }
    }

    /// <summary>
    /// CharacterStarlight클래스가 Variables.Characters[i].Cards[j]의 데이터를 가져오기 위해 { i, j }의 형식으로 사용합니다.
    /// </summary>
    public int[] CharacterData {
        get {
            return new int[2] { characterIndex, cardIndex };
        }
        set {
            characterIndex = value[0];
            cardIndex = value[1];
        }
    }

    void Start()
    {
        this.LastDate = Variables.Characters[characterIndex].Cards[cardIndex].LastReapDate;
        starBalloon = Instantiate(Resources.Load<Transform>("Prefabs/star_balloon"), transform.position + new Vector3(0.5f, 0.6f, 0f), Quaternion.identity);
        starBalloon.SetParent(this.transform);
        starBalloon.gameObject.SetActive(false);
    }


    void Update()
    {
        //최적화가 필요하다면 n초당 한 번씩만 체크하도록 변경 가능.
        if(!overPoint)
        {
            span = DateTime.Now - lastDate;
            if(span.TotalSeconds > criteriaPoint)
            {
                overPoint = true;
                starBalloon.gameObject.SetActive(true);
            }
        }
    }

    public void OnBalloonClicked() // TODO : UI상 tween 애니메이션 적용, 별빛 수확 후 데이터 저장.
    {
        overPoint = false;
        span = DateTime.Now - lastDate;
        lastDate = DateTime.Now;
        int startlight = (int)(0.5 + span.TotalSeconds / sps); //반올림
        Variables.Starlight += startlight;
        Variables.Characters[characterIndex].Cards[cardIndex].LastReapDate = this.LastDate;
        starBalloon.gameObject.SetActive(false);
        GameManager.Instance.SaveGame(); //말풍선 클릭될 때마다 수확한 별빛 데이터를 저장하기 위해 호출.
    }
}
