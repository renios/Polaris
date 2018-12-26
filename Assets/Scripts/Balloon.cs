using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class Balloon : MonoBehaviour
{
	public abstract void Init();
	
	public abstract void SetBalloonData(string str);
}