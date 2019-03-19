using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Title : MonoBehaviour {

    private float speed;
    private SpriteRenderer TitleLogo;
    private SpriteRenderer TouchToScreen;
    private bool Finish;
    private bool Stop;
    private bool SceneChanging;
    float TextEffect;

    void Awake()
    {
        TitleLogo = GameObject.Find("TitleLogo").GetComponent<SpriteRenderer>();
        TouchToScreen = GameObject.Find("TouchToScreen").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        speed = -1.0f; TextEffect = 0.0f;
        Finish = false;
        Stop = false;
        SceneChanging = false;
        TitleLogo.color = new Color(1, 1, 1, 0);
        TouchToScreen.color = new Color(1, 1, 1, 0);

        SoundManager.Play(SoundType.BgmTitle);
    }

    void Update () {
        if (!Stop&!Finish)
        {
            if (Camera.main.transform.position.y < 0.0f)
            {
                Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
                Stop = true; 
            }

            if (Camera.main.transform.position.y > 0.0f)
            {
                Camera.main.transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);
            }
        }
        else
        {
            if (Finish)
            {
                Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
                TitleLogo.color = new Color(1, 1, 1, 1);
                TouchToScreen.color = new Color(1, 1, 1, 1);
            }
            else if (TitleLogo.color.a < 1)
            {
                TextEffect += 1.0f * Time.deltaTime;
                TitleLogo.color = new Color(1, 1, 1, TextEffect);
                TouchToScreen.color = new Color(1, 1, 1, TextEffect);
            }   
            else
            {
                Finish = true;
            }
        }
    }

    public void SkipAnimation()
    {
        if (SceneChanging) return;

        if (!Finish) Finish = true;
        else
        {
            SceneChanging = true;
            StartCoroutine(ChangeScene());
        }
    }

    IEnumerator ChangeScene() {
        SoundManager.Play(SoundType.ClickImportant);
        yield return new WaitForSeconds(0.5f);
        if(Variables.HasSave)
        {
            GameManager.Instance.LoadGame();
            SceneChanger.Instance.ChangeScene("MainScene");
        }
        else
        {
            GameManager.Instance.CreateGame();
            SceneChanger.Instance.ChangeScene("Prologue");
        }
    }

    // Debug Function
    public void DeleteSave()
    {
        GameManager.Instance.DeleteGame();
        Debug.Log("Save deleted.");
    }

    // Debug Function
    public void BlackSheepWall()
    {
        GameManager.Instance.CreateGame();

        foreach(var data in Variables.Characters)
        {
            foreach(var card in data.Value.Cards)
            {
                if (card.Observable)
                    card.Observed = true;
            }
        }
        GameManager.Instance.SaveGame();

        StartCoroutine(ChangeScene());
    }
}
