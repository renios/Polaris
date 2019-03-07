using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prologue
{
    public class TextController : MonoBehaviour
    {
        public float animTime = 2f;
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

        public bool finished;

        private void Awake()
        {
            finished = false;
            TargetText = GetComponent<Text>();
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
            finished = false;
            Color color = TargetText.color;
            time = 0f;
            color.a = Mathf.Lerp(start_in, end_in, time);
            while (color.a < 1f)
            {
                time += Time.deltaTime / animTime;
                color.a = Mathf.Lerp(start_in, end_in, time);
                TargetText.color = color;
                yield return null;
            }
            finished = true;
        }

        public void StartFadeOut()
        {
            StartCoroutine("FadeOut");
        }

        IEnumerator FadeOut()
        {
            finished = false;
            Color color = TargetText.color;
            time = 0f;
            color.a = Mathf.Lerp(start_out, end_out, time);
            while (color.a > 0f)
            {
                time += Time.deltaTime / animTime;
                color.a = Mathf.Lerp(start_out, end_out, time);
                TargetText.color = color;
                yield return null;
            }
            finished = true;
        }
    }
}