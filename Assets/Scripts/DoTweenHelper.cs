using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenHelper : MonoBehaviour {

	public enum TweenFunction
	{
		None,
		DoMove,
		DOScale
	}

	public TweenFunction tween;
	public float floatValue;
	public Vector3 vectorValue;
	public float duration;
	public Ease easeType = Ease.Linear;

	// Use this for initialization
	void Start () {
		if (tween == TweenFunction.DOScale)
			transform.DOScale(floatValue, duration).SetEase(easeType);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
