using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouchManager : MonoBehaviour {

    // 1. Scope Moving as Touch
    // 2. Shot Ray -> Choose Top 4

    GameObject Scope = null;

    private float skyRadius = 4.6f;
    private float touchBound = 2.5f;
    private float scopeRadius = 1.5f;

    private bool touchOn = false;
    private Touch tempTouchs;
    
    Dictionary<string, float> Constellation = new Dictionary<string, float>();
    Dictionary<string, string> Character = new Dictionary<string, string>(); // 캐릭터이름, 별자리이름

    // Use this for initialization
    void Start () {
        Scope = GameObject.Find("Scope");

        Constellation.Add("Draco", 0f);
        Constellation.Add("UrsaMinor", 0f);
        Constellation.Add("Lyra", 0f);
        Constellation.Add("Sagittarius", 0f);
        Constellation.Add("Aquarius", 0f);
        Constellation.Add("Eridanus", 0f);
        Constellation.Add("CanisMajor", 0f);

        Character.Add("Acher", "Eridanus");
        Character.Add("CatsEye", "Draco");
        Character.Add("Melik", "Aquarius");
        Character.Add("Pluto", "Sagittarius");
        Character.Add("Polaris", "UrsaMinor");
        Character.Add("Sirius", "CanisMajor");
        Character.Add("Thuvan", "Draco");
        Character.Add("Vega", "Lyra");
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        ScopeMove();
    }


    public void ScopeMove()
    {
        Vector2 center = new Vector2(-0.17f, 3.79f);
        
        touchOn = false;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                tempTouchs = Input.GetTouch(i);
                if (tempTouchs.phase == TouchPhase.Began || tempTouchs.phase == TouchPhase.Moved)
                {
                    if(Vector3.Distance(tempTouchs.position, center) < skyRadius)
                    {
                        if (Vector3.Distance(tempTouchs.position, center) >= touchBound)
                        {
                            Scope.transform.position = center + touchBound * ((tempTouchs.position - center / Vector3.Distance(tempTouchs.position, center)));
                        }
                        else
                        {
                            Scope.transform.position = tempTouchs.position;
                        }
                    }

                    touchOn = true;
                    shotRay();
                    break;
                }
            }
        }

        // Click for Test
        Vector3 centerT = new Vector3(0f, 3.78f, -1f);

        if(Input.GetMouseButton(0))
        {
            Debug.Log(Input.mousePosition);
            Vector3 mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, -1f);
            
            if (Vector3.Distance(mos, centerT) < skyRadius)
            {
                if (Vector3.Distance(mos, centerT) >= touchBound)
                {
                    Scope.transform.localPosition = centerT + touchBound * ((mos - centerT) / Vector3.Distance(mos, centerT));
                }
                else
                {
                    Scope.transform.localPosition = mos;
                }
            }
        }
    }

    public void shotRay()
    {
        Vector3 pos = Scope.transform.position;

        CastRay(pos);
        CastRay(pos);
        CastRay(pos);
        CastRay(pos);

        for (float i = 1; i <= 200; i++)
        {
            for (float j = 0; j < i * 18f; j++)
            {
                float r = scopeRadius * 1f / 200f * i;
                float theta = 2f * Mathf.PI * (j / (18f * i)) + (2f * Mathf.PI / 18 / 200 * (i - 1));
                pos = Scope.transform.position + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
                CastRay(pos);
            }
        }

        var Char_Prob = ();

        // var Constellation_desc = Constellation.OrderByDescending(p => p.Value);

        for (int i = 0; i < 4; i++)
        {
            //@
            KeyValuePair<string, float> conRank = Constellation_desc.ElementAt(i);
            //@
            //Debug.Log(conRank.Key + ": " + Mathf.Round(conRank.Value / all * 10000) / 100 + "% (" + conRank.Value + ")");
        }
    }

    void CastRay(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider != null)
        {
            Constellation[hit.collider.name]++;
        }
    }
}
