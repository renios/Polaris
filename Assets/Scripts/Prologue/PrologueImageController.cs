using System.Collections;
using UnityEngine;

namespace Prologue {
    public class PrologueImageController : MonoBehaviour {
        ImageFadeIn imageFadeIn;
        FadeBackground bg;
        //이미지가 바뀌고 기다리는 시간. 
        public float waitTime;
        //빈종이 대기 시간
        public float emptyTime;
        //이미지 페이드에 걸리는 시간. Opening1 오브젝트와 동기화됨
        public float fadeTime;
        float totalTime;

        private void Awake() {
            imageFadeIn = GameObject.Find("Opening1").GetComponent<ImageFadeIn>();
            bg = GameObject.Find("Image").GetComponent<FadeBackground>();
            imageFadeIn.fadeTime = fadeTime;
        }

        IEnumerator Start() {
            Transition();
            SoundManager.Play(SoundType.BgmTitle);
            yield return new WaitForSeconds(totalTime);
            End();
        }
        
        //TODO : 씬 바꾸는 임시 코드 개선
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                End();
            }
        }
        private void Transition() {
            float time = 0.0f;
            for (int i = 0; i < 17; i++) {
                StartCoroutine(FadeIn(time));
                StartCoroutine(BackgroundChange(time+fadeTime));
                
                if (i == 2 || i == 6 || i == 9 || i == 12 || i == 16)
                {
                    time += (waitTime + emptyTime);
                }
                else
                {
                    time += (waitTime + fadeTime);
                }
                totalTime = time;
            }
        }
        IEnumerator BackgroundChange(float time) {
            yield return new WaitForSeconds(time);
            bg.NewImage();
        }
        IEnumerator FadeIn(float time) {
            yield return new WaitForSeconds(time);
            imageFadeIn.StartFadeIn();
        }
        void FadeOut()
        {
            imageFadeIn.StartFadeOut();
        }
        void End()
        {
            GameManager.Instance.SaveGame();
            SoundManager.Play(SoundType.BgmMain);
            SceneChanger.Instance.ChangeScene("GachaTut_2");
        }
    }
}

