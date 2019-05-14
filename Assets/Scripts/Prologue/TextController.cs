using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prologue
{
    public class TextController : MonoBehaviour
    {
        ImageFadeIn fadeController;
        public float FadeInTime;
        public float FadeOutTime;

        private Text TargetText;
        //for fade in
        private float start_in = 0f;
        private float end_in = 1f;
        //for fade out
        private float start_out = 1f;
        private float end_out = 0f;

        private float time = 0f;
        float fadeTime;

        private void Awake()
        {
            TargetText = GetComponent<Text>();
            fadeController = GameObject.Find("Opening1").GetComponent<ImageFadeIn>();
        }

        private void Start()
        {
            Invoke("StartFadeIn", FadeInTime);
            Invoke("StartFadeOut", FadeOutTime);
        }

        public void StartFadeIn()
        {
            StartCoroutine("FadeIn");
        }

        IEnumerator FadeIn()
        {
         
            Color color = TargetText.color;
            time = 0f;
            color.a = Mathf.Lerp(start_in, end_in, time);
            while (color.a < 1f)
            {
                time += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(start_in, end_in, time);
                TargetText.color = color;
                yield return null;
            }
        }

        public void StartFadeOut()
        {
            StartCoroutine("FadeOut");
        }

        IEnumerator FadeOut()
        {
            Color color = TargetText.color;
            time = 0f;
            color.a = Mathf.Lerp(start_out, end_out, time);
            while (color.a > 0f)
            {
                time += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(start_out, end_out, time);
                TargetText.color = color;
                yield return null;
            }
        }
    }
}