using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prologue
{
    public class FadeBackground : MonoBehaviour
    {
        private Image Renderer;
        private Image img;

        private void Awake()
        {
            Renderer = GetComponent<Image>();
            img = GameObject.Find("Opening1").GetComponent<Image>();
        }
        public void NewImage()
        {
            gameObject.GetComponent<Image>().sprite = img.sprite;
            Color color = Renderer.color;
            color.a = 1.0f;
            Renderer.color = color;
        }
    }
}
