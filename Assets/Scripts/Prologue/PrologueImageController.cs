using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prologue
{

    public class PrologueImageController : MonoBehaviour
    {
        ImageFadeIn controller;
        FadeBackground bg;
        //이미지가 바뀌고 기다리는 시간. 
        public float waitTime;
        //빈종이 대기 시간
        public float emptyTime;
        //이미지 페이드에 걸리는 시간. Opening1 컴포넌트에서 수정 가능.
        float fadeTime;
        float totalTime;

        private void Awake()
        {
            controller = GameObject.Find("Opening1").GetComponent<ImageFadeIn>();
            bg = GameObject.Find("Image").GetComponent<FadeBackground>();
            fadeTime = controller.fadeTime;
        }
        private void Start()
        {
            Transition();
            SoundManager.Play(SoundType.BgmTitle);
            Invoke("End", totalTime);
        }
        //TODO : 씬 바꾸는 임시 코드 개선
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                End();
            }
        }
        private void Transition()
        {
            float time = 0.0f;
            for (int i = 0; i < 17; i++)
            {
                Invoke("FadeIn", time);
                Invoke("BackgroundChange", time + fadeTime);
                if (i == 2 || i == 6 || i == 9 || i == 12 || i == 16)
                {
                    time += (waitTime+emptyTime);
                }
                else
                {
                    time += (fadeTime+waitTime);
                }
                totalTime = time;
            }
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
    }
}

