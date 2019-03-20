using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour {

    public GameObject resultCharacter, nameTag, nameText, TtS, fader, effect;
    private bool isEnd = false;
    private string gachaResult = null;
    private int charIndex = 0;

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
                foreach (var value in Variables.Characters.Values)
                {
                    if (gachaResult == value.InternalName)
                        charIndex = value.CharNumber;
                }

                var rankCharacter = Variables.Characters[charIndex];
                bool isUp = false;
                int nextFav = 0;
                
                rankCharacter.Cards[0].Favority += 1;
                GameManager.Instance.SaveGame();

                if (rankCharacter.Cards[0].Observed == false) // 첫 획득
                {
                    rankCharacter.Cards[0].Observed = true;
                    GameManager.Instance.SaveGame();
                    StartStory("GachaScene", 0);
                    return;
                }

                for (int i = 0; i < 5; i++)
                {
                    if (rankCharacter.Cards[0].Favority == Variables.FavorityThreshold[i])
                    {
                        isUp = true;
                        nextFav = i;
                    }
                }

                if(isUp) // 호감도 Up!
                    StartStory("GachaScene", nextFav + 1);

                else
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
        TextMesh tmnT = nameText.GetComponent<TextMesh>();

        Color tC = srnT.color;
        Color textColor = tmnT.color;
        tC.a = 0f;
        textColor.a = 0f;
        srnT.color = tC;
        tmnT.color = textColor;

        // 화면 FadeIn
        while (tempColor.a > 0f)
        {
            tempColor.a -= Time.deltaTime / fadeInTime;

            if (tempColor.a < 0f)
                tempColor.a = 0f;
            srFader.color = tempColor;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        //NameTag FadeIn
        while (tC.a < 0.5f)
        {
            tC.a += Time.deltaTime;
            if (tC.a >= 0.5f)
                tC.a = 0.5f;
            textColor.a = tC.a * 2;

            srnT.color = tC;
            tmnT.color = textColor;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //Show TTS
        yield return new WaitForSeconds(1f);
        TtS.SetActive(true);
        isEnd = true;
    }

    public void StartStory(string nextScene, int storyIndex)
    {
        Variables.DialogAfterScene = nextScene;
        Variables.DialogCharIndex = charIndex;
        Variables.DialogCardIndex = 0;
        Variables.DialogChapterIndex = storyIndex;
        SceneManager.LoadScene("NewDialogScene");
        //SceneChanger.Instance.ChangeScene("NewDialogScene");
    }
}
