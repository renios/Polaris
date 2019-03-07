using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prologue
{
    public class BalloonController : MonoBehaviour
    {
        public float animTime = 2f;
        private Image TargetImage;
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
            TargetImage = GetComponent<Image>();
        }

        private void Start()
        {
            StartFadeIn(); StartSetPos();
            Invoke("StartFadeOut", 8.0f);
            Invoke("StartFadeIn", 25.0f);
            Invoke("StartFadeOut", 33.0f);
            Invoke("StartFadeIn", 35.5f);
            Invoke("StartFadeOut", 43.5f);
            Invoke("StartFadeIn", 46.0f);
            Invoke("StartFadeOut", 58.0f);
        }

        public void StartFadeIn()
        {
            StartCoroutine("FadeIn");
        }

        IEnumerator FadeIn()
        {
            finished = false;
            Color color = TargetImage.color;
            time = 0f;
            color.a = Mathf.Lerp(start_in, end_in, time);
            while (color.a < 1f)
            {
                time += Time.deltaTime / animTime;
                color.a = Mathf.Lerp(start_in, end_in, time);
                TargetImage.color = color;
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
            Color color = TargetImage.color;
            time = 0f;
            color.a = Mathf.Lerp(start_out, end_out, time);
            while (color.a > 0f)
            {
                time += Time.deltaTime / animTime;
                color.a = Mathf.Lerp(start_out, end_out, time);
                TargetImage.color = color;
                yield return null;
            }
            finished = true;
        }

        public void StartSetPos()
        {
            StartCoroutine("SetPos");
        }

        IEnumerator SetPos()
        {
            yield return new WaitForSeconds(24.5f);
            transform.localPosition = new Vector3(0, 610, 0);
            yield return new WaitForSeconds(21.0f);
            transform.localPosition = new Vector3(0, 690, 0);
        }
    }
}