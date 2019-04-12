using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prologue
{

    public class PrologueImageController : MonoBehaviour
    {
        ImageFadeIn controller;
        FadeBackground bg;

        private void Awake()
        {
            controller = GameObject.Find("Opening1").GetComponent<ImageFadeIn>();
            bg = GameObject.Find("Image").GetComponent<FadeBackground>();
        }
        private void Start()
        {
            Transition();
            SoundManager.Play(SoundType.BgmTitle);
        }
        private void Transition()
        {
            float time = 0.0f;
            for (int i = 0; i < 17; i++)
            {
                Invoke("FadeIn", time);
                Invoke("BackgroundChange", time + 2.1f);
                if (i == 2 || i == 6 || i == 9 || i == 12 || i == 16)
                {
                    time += 2.5f;
                }
                else
                {
                    time += 4.0f;
                }
            }
            Invoke("End", 60f);
        }
        void BackgroundChange()
        {
            bg.NewImage();
        }
        void FadeIn()
        {
            controller.StartFadeIn();
        }
        void FadeOut()
        {
            controller.StartFadeOut();
        }

        void End()
        {
            GameManager.Instance.SaveGame();
            SoundManager.Play(SoundType.BgmMain);
            SceneChanger.Instance.ChangeScene("GachaScene");
        }
        //TODO : 씬 바꾸는 임시 코드 개선
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                End();
            }
        }
    }
}

