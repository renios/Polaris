using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GachaManager : MonoBehaviour {
    
    GameObject skyImage = null;
    GameObject uiCircle = null;

    public float moveLength = 60f;
    float radius = 118f;
    Vector3 prevPos = new Vector3(0, 0, 0);

    private Touch tempTouchs;
    private bool touchOn;

    Dictionary<string, float> Constellation = new Dictionary<string, float>();

    // Use this for initialization
    void Start () {

        skyImage = GameObject.Find("Sky Image");
        uiCircle = GameObject.Find("UI Circle");
        
        Constellation.Add("Draco", 0f);
        Constellation.Add("UrsaMinor", 0f);
        
        Constellation.Add("Lyra", 0f);
        Constellation.Add("Sagittarius", 0f);

        Constellation.Add("Aquarius", 0f);

        Constellation.Add("Eridanus", 0f);
        Constellation.Add("CanisMajor", 0f);
    }
	
	// Update is called once per frame
	void Update () {
        TouchManager();

        //@
        /*
        if(Input.GetMouseButton(0))
        {
            Vector3 mos = Input.mousePosition;
            uiCircle.transform.position = mos;
        }
        */
        //@

        /*
        if(uiCircle.transform.position != prevPos)
        {
            Constellation = Constellation.ToDictionary(p => p.Key, p => 0f); // Reset
            prevPos = uiCircle.transform.position;
            shotRay();
        }
        */
	}

    public void shotRay()
    {
        Vector3 pos = uiCircle.transform.position;

        CastRay(pos);
        CastRay(pos);
        CastRay(pos);
        CastRay(pos);

        for (float i = 1; i <= 200; i++)
        {
            for (float j = 0; j < i * 18f; j++)
            {
                float r = radius * 1f / 200f * i;
                float theta = 2f * Mathf.PI * (j / (18f * i)) + (2f * Mathf.PI / 18 / 200 * (i - 1));
                pos = uiCircle.transform.position + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
                CastRay(pos);
            }
        }

        float all = 0;

        foreach (var key in Constellation.Keys)
        {
            all += Constellation[key];
        }

        var Constellation_desc = Constellation.OrderByDescending(p => p.Value);

        for(int i = 0; i < 5; i++)
        {
            //@
            KeyValuePair<string, float> conRank = Constellation_desc.ElementAt(i);
            //@
            Debug.Log(conRank.Key + ": " + Mathf.Round(conRank.Value / all * 10000)/100+"%");
        }
        Debug.Log("------------------------------");
    }

    void CastRay(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider != null)
        {
            Constellation[hit.collider.name]++;
        }
    }

    public void TouchManager()
    {
        Vector2 center = new Vector2(540f, 1240f);

        touchOn = false;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                tempTouchs = Input.GetTouch(i);
                if (tempTouchs.phase == TouchPhase.Began || tempTouchs.phase == TouchPhase.Moved)
                {
                    if (Vector3.Distance(tempTouchs.position, center) < 360f)
                    {
                        if (Vector3.Distance(tempTouchs.position, center) >= 240f)
                        {
                            uiCircle.transform.position = center + 240f * ((tempTouchs.position - center / Vector3.Distance(tempTouchs.position, center)));
                        }
                        else
                        {
                            uiCircle.transform.position = tempTouchs.position;
                        }
                    }

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
