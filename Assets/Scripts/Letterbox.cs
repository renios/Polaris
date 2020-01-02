using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letterbox : MonoBehaviour
{
	public Material boxMat;

	void Start()
	{
		Fit();
	}

	// Update is called once per frame
	void Update()
	{
		Fit();
	}

	public void Fit()
	{
		boxMat.SetFloat("_Width", Screen.width);
		boxMat.SetFloat("_Height", Screen.height);
	}
}
