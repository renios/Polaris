using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour {

    public GameObject resultCharacter, nameTag, nameText, TtS, fader, effect;
    private bool isEnd = false;
    private string gachaResult = null;
    
	// Use this for initialization
	void Start () {
        fader.SetActive(true);
        StartCoroutine(gachaFadeIn(3f));
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isEnd)
        {
            if (Input.GetMouseButton(0))
            {
                TouchManager.moveAble = true;
                Variables.btnState = 0;
                SceneManager.LoadScene("GachaScene");
            }
        }
	}

    IEnumerator gachaFadeIn(float fadeInTime)
    {
        // 정보처리
        gachaResult = GachaManager.gachaResult;
        resultCharacter.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Characters/" + gachaResult + "/default/image_full");

        int charIndex = 0;
        foreach (var value in Variables.Characters.Values)
        {
            if (gachaResult == value.InternalName)
                charIndex = value.CharNumber;
        }
        nameText.GetComponent<TextMesh>().text = Variables.Characters[charIndex].Name;

        // 초기화
        TtS.SetActive(false);
        effect.SetActive(true);
        
        SpriteRenderer srFader = fader.GetComponent<SpriteRenderer>();
        Color tempColor = srFader.color;
        tempColor.a = 1f;
        srFader.color = tempColor;

        SpriteRenderer srnT = nameTag.GetComponent<SpriteRenderer>();
        Color tC = srnT.color;
        tC.a = 0f;
        srnT.color = tC;

        // 화면 FadeIn
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeInTime;
            srFader.color = tempColor;

            if (tempColor.a < 0f)
                tempColor.a = 0f;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //NameTag FadeIn
        while (tC.a < 1f)
        {
            tC.a += Time.deltaTime;
            srnT.color = tC;

            if (tC.a >= 1f)
                tC.a = 1f;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //Show TTS
        yield return new WaitForSeconds(1f);
        TtS.SetActive(true);
        isEnd = true;
    }
}
