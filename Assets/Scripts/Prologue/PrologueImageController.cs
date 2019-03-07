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
    }
}

