using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStarlight : MonoBehaviour
{
    public float sps = 1; //이 값(초)당 별빛 생성
    public float criteriaPoint = 10; //이 값보다 쌓인 별빛이 많으면 말풍선 띄움

    private DateTime lastDate;
    private TimeSpan span;
    private bool overPoint = false; //수확 가능한 포인트 이상 쌓였는가
    private Transform starBalloon;
    private int characterIndex;
    private int cardIndex;

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
            }
        }
    }

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
    }
}
