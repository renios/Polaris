using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Tutorial
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TutorialPushAlert : MonoBehaviour
    {
        public float fadeinTime;
        public float displayTime;
        public float fadeoutTime;

        float internalClock;

        void Update()
        {
            if (internalClock <= displayTime + fadeoutTime)
            {
                if (internalClock <= fadeinTime)
                    GetComponent<CanvasGroup>().alpha = Mathf.InverseLerp(0, fadeinTime, internalClock);
                else if (fadeinTime < internalClock && internalClock <= displayTime)
                    GetComponent<CanvasGroup>().alpha = 1;
                else if (internalClock > displayTime)
                    GetComponent<CanvasGroup>().alpha = Mathf.InverseLerp(displayTime + fadeoutTime, displayTime, internalClock);
                internalClock += Time.deltaTime;
                
                if(internalClock > displayTime + fadeoutTime)
                    gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            internalClock = 0;
        }
    }
}