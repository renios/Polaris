using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouchManager : MonoBehaviour {
    GameObject Scope = null;

    private float skyRadius = 4.6f;
    private float touchBound = 3.9f;
    private float scopeRadius = 1.5f;

    private bool touchOn = false;
    private Touch tempTouchs;
    private int divideCount = 10; // divideCount만큼의 동심원 둘레에 ray를 쏩니다.

    public static bool moveAble = true;
    private Vector3 startScopePos;
    private Vector3 startMousePos;
    // private string[] charList = new string[] { "acher", "catseye", "melik", "pluto", "polaris", "sirius", "thuban", "vega", "rescha", "sualocin", "rigeleuse" }; // 캐릭터 추가하면 별자리는 자동추가됩니다.
    
    Dictionary<string, float> Constellation = new Dictionary<string, float>();
    Dictionary<string, string> Character = new Dictionary<string, string>(); // 캐릭터이름, 별자리이름
    public static Dictionary<string, float> charProb = new Dictionary<string, float>();

    // Use this for initialization
    void Start () {

        //LoadCharacter();

        Scope = GameObject.Find("Scope");
        Scope.transform.localPosition = Variables.scopePos;
        
        touchOn = false;

        characterAdd();
        ShotRay();
    }
	
    void characterAdd()
    {
        string charName = null, constellName = null;
        int charIndex = 0;

        foreach (var value in Variables.Characters.Values)
        {
            if (value.Cards[0].Observable)
            {
                charIndex = value.CharNumber;
                charName = value.InternalName;

                constellName = Variables.Characters[charIndex].ConstelKey[0];
                if (!Constellation.ContainsKey(constellName))
                    Constellation.Add(constellName, 0f);

                if (!Character.ContainsKey(charName))
                    Character.Add(charName, constellName);
            }
        }
    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        if(moveAble)
            ScopeMove();

        if (Input.GetKeyDown(KeyCode.Escape) && Variables.isTutorialFinished)
        {
            SceneChanger.Instance.ChangeScene("MainScene");
        }
    }

    void LoadCharacter()
    {
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        var constelRaw = Resources.Load<TextAsset>("Data/Constels");
        var constelGroup = JsonMapper.ToObject(constelRaw.text);

        Variables.Characters = new Dictionary<int, CharacterData>();
        foreach (CharacterDataCore data in charGroup.Characters)
        {
            Variables.Characters.Add(data.CharNumber, data);
        }

        Variables.Constels = new Dictionary<string, ConstelData>();
        foreach (JsonData data in constelGroup["constels"])
        {
            var index = (int)data["groupIndex"];
            foreach (JsonData constel in data["groupItmems"])
            {
                var newConstel = new ConstelData((string)constel["key"], (string)constel["name"], index);
                Variables.Constels.Add(newConstel.InternalName, newConstel);
            }
        }
    }
    /*
    public void ScopeMove()
    {
        Vector3 centerT = new Vector3(0f, 3.78f, -1f);
        if(Input.GetMouseButtonDown(0))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f)) * (1 / 0.522f);
            startScopePos = Scope.transform.localPosition;
        }
        else if(Input.GetMouseButton(0))
        {
            //Vector3 mos = (Input.mousePosition / 100f) + new Vector3(-5.4f, -9.6f, -1f);
            Vector3 mos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 9f)) * (1 / 0.522f);
            // Debug.Log(mos);

            if (Vector3.Distance(mos, centerT) < skyRadius)
            {
                if (Vector3.Distance(mos, centerT) >= touchBound)
                {
                    Scope.transform.localPosition = centerT + touchBound * ((mos - centerT) / Vector3.Distance(mos, centerT));
                }
                else
                {
                    Vector2 v = (Vector2)mos - startMousePos + startScopePos;
                    //v.y = 9f * (1f / 0.522f);
                    Scope.transform.localPosition = new Vector3(v.x, v.y, -1);
                }
            }
            Variables.scopePos = Scope.transform.localPosition;
            shotRay();
        }
    }
    */
    public void ScopeMove()
    {
        Vector3 center = new Vector3(-0.1f * 0.522f, 4.55f * 0.522f, -1);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0)) {
            if (!touchOn) { 
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                startScopePos = Scope.transform.position;
                if (Vector2.Distance(startMousePos, center) < skyRadius * 0.522f) {
                    touchOn = true;
                }
            }
        }
        else {
            if (touchOn) {/*
                if (Vector2.Distance(mousePos, startMousePos) < 0.01f){
                    if (Vector2.Distance(mousePos, center) > touchBound * 0.522f) {
                        Vector2 delta = mousePos - center;
                        startMousePos = center + (Vector3)(delta.normalized * touchBound * 0.522f);
                    }
                    startMousePos.z = -1;
                    Scope.transform.position = startMousePos;
                }*/
                touchOn = false;
            }
        }
        
        if (touchOn) {
            var scopePos = startScopePos + (mousePos - startMousePos);
            if (Vector2.Distance(scopePos, center) > touchBound * 0.522f) {
                Vector2 delta = scopePos - center;
                var newScopePos = center + (Vector3)(delta.normalized * touchBound * 0.522f);
                //startMousePos += scopePos - newScopePos;
                scopePos = newScopePos;
            }
            scopePos.z = -1f;
            Scope.transform.position = scopePos;
        }

        Variables.scopePos = Scope.transform.localPosition;
        ShotRay();
    }
    /*
    void DebugVector(Vector2 v, string name = null) {

        Debug.Log(name + " " + v.x + ", " + v.y);
    }
    void DebugVector(Vector3 v, string name = null) {
        DebugVector((Vector2)v, name);
    }
    */

    public void ShotRay()
    {
        charProb.Clear();
        Constellation = Constellation.ToDictionary(p => p.Key, p => 0f);

        Vector3 pos = Scope.transform.position;

        CastRay(pos);
        CastRay(pos);
        CastRay(pos);
        CastRay(pos);

        for (float i = 1; i <= divideCount; i++)
        {
            for (float j = 0; j < i * 18f; j++)
            {
                float r = scopeRadius * 1f / divideCount * i;
                float theta = 2f * Mathf.PI * (j / (18f * i)) + (2f * Mathf.PI / 18 / divideCount * (i - 1));
                pos = Scope.transform.position + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
                CastRay(pos);
            }
        }

        foreach (var key in Character.Keys)
        {
            if (key == "catseye")
                charProb.Add(key, Constellation[Character[key]] * 0.4f);
            else if (key == "polaris")
                charProb.Add(key, 0);
            else
                charProb.Add(key, Constellation[Character[key]]);
        }

        var Char_desc = charProb.OrderByDescending(p => p.Value);
        GameObject CharSprite = null;
        string nowConstellName = CastRayCenter(Scope.transform.position);

        GameObject ConstellSprite = GameObject.Find("Constell_Sprite");
        ConstellSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Constellations/Observation/" + nowConstellName);
        GameObject nowConstell = GameObject.Find("Now_Constell");
        if(nowConstellName != "null")
            nowConstell.GetComponent<TextMesh>().text = Variables.Constels[nowConstellName].Name;
        else
            nowConstell.GetComponent<TextMesh>().text = "-";

        int charIndex = 0;

        for (int i = 1; i <= 4; i++)
        {
            KeyValuePair<string, float> charRank = Char_desc.ElementAt(i-1);
            
            charIndex = 0;
            foreach(var value in Variables.Characters.Values)
            {
                if (charRank.Key == value.InternalName)
                    charIndex = value.CharNumber;
            }
            
            var rankCharacter = Variables.Characters[charIndex];
            int favority = rankCharacter.Cards[0].Favority;
            int cnt = 0, progress = 0, required = 0;

            CharSprite = GameObject.Find("Character_" + i.ToString());
            GameObject heartBarUI = GameObject.Find("HeartBarUI_" + i.ToString());
            GameObject heart = heartBarUI.transform.Find("Heart_" + i.ToString()).gameObject;
            GameObject heartBar = heartBarUI.transform.Find("HeartBar_" + i.ToString()).gameObject;
            GameObject nowFav = heartBarUI.transform.Find("Now_" + i.ToString()).gameObject;
            GameObject totalFav = heartBarUI.transform.Find("Total_" + i.ToString()).gameObject;
            GameObject favLevel = heart.transform.Find("Fav_Level_" + i.ToString()).gameObject;
            GameObject charName = GameObject.Find("Name_" + i.ToString());
            
            if (rankCharacter.Cards[0].Observed)
            {
                heart.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha/obs_heart");
                heartBarUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha/obs_heartbarbackground");
                favLevel.SetActive(true);
                heartBar.SetActive(true);

                for (; cnt < Variables.FavorityThreshold.Length; cnt++)
                {
                    if (favority < Variables.FavorityThreshold[cnt])
                        break;
                }

                if (cnt >= Variables.FavorityThreshold.Length)
                {
                    progress = 0;
                    required = 0;
                }
                else
                {
                    progress = favority - (cnt > 0 ? Variables.FavorityThreshold[cnt - 1] : 0);
                    required = Variables.FavorityThreshold[cnt] - (cnt > 0 ? Variables.FavorityThreshold[cnt - 1] : 0);
                }

                float barScale;
                if (required != 0)
                    barScale = (float)progress / (float)required;
                else
                    barScale = 1f;
                float barX = 0.575f * barScale - 0.545f;
                heartBar.transform.localScale = new Vector3(barScale, 1f, 1f);
                heartBar.transform.localPosition = new Vector3(barX, 0f, -1f);

                CharSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/" + charRank.Key + "/default/image_obs");
                nowFav.GetComponent<TextMesh>().text = progress.ToString();
                totalFav.GetComponent<TextMesh>().text = required.ToString();
                favLevel.GetComponent<TextMesh>().text = (cnt + 1).ToString();
                charName.GetComponent<TextMesh>().text = rankCharacter.Name;
            }
            else
            {
                heart.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha/obs_heartgray");
                heartBarUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Gacha/obs_heartgraybar");
                favLevel.SetActive(false);
                heartBar.SetActive(false);
                CharSprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("obs_character_unknown");
                charName.GetComponent<TextMesh>().text = "???";
            }
        }
    }

    void CastRay(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if(hit.collider != null)
        {
            if (Constellation.ContainsKey(hit.collider.name))
                Constellation[hit.collider.name]++;
        }
    }

    string CastRayCenter(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider == null)
            return "null";
        else
            return hit.collider.name;
    }

    //TODO : 씬 바꾸는 임시 코드 개선
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Variables.isTutorialFinished)
        {
            SceneChanger.Instance.ChangeScene("MainScene");
        }
    }
}
