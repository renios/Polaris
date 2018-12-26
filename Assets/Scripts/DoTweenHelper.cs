using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenHelper : MonoBehaviour {

	public enum TweenFunction
	{
		None,
		DoMove,
		DoScale
	}

	public TweenFunction Tween;
	public float FloatValue;
	public Vector3 VectorValue;
	public float Duration;
	public Ease EaseType = Ease.Linear;

	// Use this for initialization
	void Start () {
		if (Tween == TweenFunction.DoScale)
			transform.DOScale(FloatValue, Duration).SetEase(EaseType);
	}
}
