using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prologue
{
    public class BalloonController : MonoBehaviour
    {
        ImageFadeIn fadeController;
        PrologueImageController imageController;
        private Image TargetImage;
        //for fade in
        private float start_in = 0f;
        private float end_in = 1f;
        //for fade out
        private float start_out = 1f;
        private float end_out = 0f;

        private float fadeTime;
        private float waitTime;
        private float emptyTime;

        private float time = 0f;
        private int pos = 0;
        
        private Text TargetText;

        private int textNum = 0;

        private void Awake()
        {
            fadeController = GameObject.Find("Opening1").GetComponent<ImageFadeIn>();
            imageController = GameObject.Find("Prologue Controller").GetComponent<PrologueImageController>();
            TargetImage = GetComponent<Image>();
            fadeTime = fadeController.fadeTime;
            waitTime = imageController.waitTime;
            emptyTime = imageController.emptyTime;
        }

        private void Start()
        {
            StartFadeIn(); 
            float time = 0.0f;
            time += 2 * (fadeTime + waitTime);
            Invoke("StartFadeOut", time);
            time = time + 2 * emptyTime + 3 * waitTime + 5 * fadeTime;
            Invoke("StartSetPos", time);
            Invoke("StartFadeIn", time);
            time += 2 * (fadeTime + waitTime);
            Invoke("StartFadeOut", time);
            time += (fadeTime + emptyTime);
            Invoke("StartFadeIn", time);
            time += 2 * (fadeTime + waitTime);
            Invoke("StartFadeOut", time);
            time += (fadeTime + emptyTime);
            Invoke("StartSetPos", time);
            Invoke("StartFadeIn", time);
            time += 3 * (fadeTime + waitTime);
            Invoke("StartFadeOut", time);
        }

        public void StartFadeIn()
        {
            StartCoroutine("FadeIn");
        }

        IEnumerator FadeIn()
        {
            if (textNum == 0)
                TargetText = GameObject.Find("Text1").GetComponent<Text>();
            else if (textNum == 1)
                TargetText = GameObject.Find("Text2").GetComponent<Text>();
            else if (textNum == 2)
                TargetText = GameObject.Find("Text3").GetComponent<Text>();
            else
                TargetText = GameObject.Find("Text4").GetComponent<Text>();

            Color color = TargetImage.color;
            Color textColor = TargetText.color;

            time = 0f;
            color.a = Mathf.Lerp(start_in, end_in, time);
            while (color.a < 1f)
            {
                time += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(start_in, end_in, time);
                textColor.a = Mathf.Lerp(start_in, end_in, time);
                TargetImage.color = color;
                TargetText.color = textColor;
                yield return null;
            }
            textNum++;
        }

        public void StartFadeOut()
        {
            StartCoroutine("FadeOut");
        }

        IEnumerator FadeOut()
        {
            Color color = TargetImage.color;
            Color textColor = TargetText.color;
            time = 0f;
            color.a = Mathf.Lerp(start_out, end_out, time);
            textColor.a = Mathf.Lerp(start_out, end_out, time);
            while (color.a > 0f)
            {
                time += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(start_out, end_out, time);
                textColor.a = Mathf.Lerp(start_out, end_out, time);
                TargetImage.color = color;
                TargetText.color = textColor;
                yield return null;
            }
        }

        public void StartSetPos()
        {
            if (pos == 0)
            {
                transform.localPosition = new Vector3(0, 610, 0);
                pos++;
            }
            else if (pos == 1)
            {
                transform.localPosition = new Vector3(0, 690, 0);
            }
        }
        
    }
}