using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaManager : MonoBehaviour {

    GameObject skyImage = null;
    GameObject uiCircle = null;
    public float moveLength = 60f;
    private Touch tempTouchs;
    private bool touchOn;

    // Use this for initialization
    void Start () {
        skyImage = GameObject.Find("Sky Image");
        uiCircle = GameObject.Find("UI Circle");
    }
	
	// Update is called once per frame
	void Update () {
        touchOn = false;
		if(Input.touchCount > 0)
        {
            for(int i = 0; i < Input.touchCount; i++)
            {
                tempTouchs = Input.GetTouch(i);
                if(tempTouchs.phase == TouchPhase.Began || tempTouchs.phase == TouchPhase.Moved)
                {
                    if (Vector3.Distance(tempTouchs.position, new Vector2(540f, 602f)) < 360f)
                    {
                        if(Vector3.Distance(tempTouchs.position, new Vector2(540f, 602f)) >= 240f)
                        {
                            uiCircle.transform.position = new Vector2(540f, 602f) + 240f * ((tempTouchs.position - new Vector2(540f, 602f)) / Vector3.Distance(tempTouchs.position, new Vector2(540f, 602f)));
                        }
                        else
                        {
                            uiCircle.transform.position = tempTouchs.position;
                        }
                    }
                    // @UI와 구분 필요. 밖으로 튀어나가지 않게
                    // 360이 사실상 maximum

                    touchOn = true;
                    break;
                }
            }
        }
	}

    public void LeftBtn()
    {
        skyImage.transform.position += new Vector3(moveLength, 0, 0);
    }
    public void RightBtn()
    {
        skyImage.transform.position -= new Vector3(moveLength, 0, 0);
    }
    public void UpBtn()
    {
        skyImage.transform.position -= new Vector3(0, moveLength, 0);
    }
    public void DownBtn()
    {
        skyImage.transform.position += new Vector3(0, moveLength, 0);
    }

}
