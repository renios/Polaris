using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseEffectManager : MonoBehaviour {

	public GameObject Effect;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Instantiate(Effect, new Vector3(mousePosition.x, mousePosition.y, 0), Quaternion.identity);
		}
	}
}
