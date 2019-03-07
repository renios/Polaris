using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prologue
{
    public class ImageFadeIn : MonoBehaviour
    {
        public Sprite[] images = new Sprite[17];
        public float animTime = 2f;
        private Image TargetImage;
        //for fade in
        private float start_in = 0f;
        private float end_in = 1f;
        //for fade out
        private float start_out = 1f;
        private float end_out = 0f;

        private float time = 0f;
        private int count = 1;

        public bool finished;

        private void Awake()
        {
            finished = false;
            TargetImage = GetComponent<Image>();
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
            yield return new WaitForSeconds(0.5f);
            color.a = 0.0f;
            TargetImage.color = color;
            if(count<17) gameObject.GetComponent<Image>().sprite = images[count];
            count++;
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
        }
}