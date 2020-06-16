using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ShapedButton : MonoBehaviour
{
	Sprite beforeSprite;

	void Start()
	{
		GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
	}
}
