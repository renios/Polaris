using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTutorial : MonoBehaviour {

    public GameObject scopeEff;
    public GameObject tutBackground;
    public GameObject tutText_1, tutText_2;
    public GameObject tutTouch;

	// Use this for initialization
	void Start() 
    {
        scopeEff.SetActive(false);
        tutBackground.SetActive(true);
        tutText_1.SetActive(true);
        tutText_2.SetActive(false);
        tutTouch.SetActive(true);
	}
	
	// Update is called once per frame
	void Update() 
    {
		switch(Variables.TutorialStep)
        {
            case 5:
                // 
                if (Input.GetMouseButton(0))
                {
                    tutBackground.SetActive(false);
                }
                break;
            case 6:
                tutBackground.SetActive(false);
                tutText_1.SetActive(false);
                tutText_2.SetActive(true);
                scopeEff.SetActive(true);
                break;
            default:
                break;
        }
	}
}
