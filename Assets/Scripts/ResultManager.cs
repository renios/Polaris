using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour {

    public GameObject resultCharacter, nameTag, nameText, TtS, fader, effect;
    [SerializeField] private Image startStory;
    [SerializeField] private Image favorityUp;
    [SerializeField] private Image favUp;
    [SerializeField] private Image favCharPicture;
    [SerializeField] private Slider favBar;
    [SerializeField] private Text favProgressText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text storyText;
    [SerializeField] private Text favConstellation;
    [SerializeField] private Text favName;
    private bool isEnd;
    private bool levelUp;
    private string gachaResult;
    private int charIndex;
    private int nextFav;

    // Use this for initialization
    void Start () {
        isEnd = false;
        levelUp = false;
        gachaResult = null;
        charIndex = 0;
        nextFav = 0;

        fader.SetActive(true);
        StartCoroutine(gachaFadeIn(3f));
        SoundManager.Play(SoundType.GachaResult);

        if(Variables.tutState == 9)
        {
            Variables.isTutorialFinished = true;
            Variables.tutState = 10;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if(isEnd)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isEnd = false;
                TouchManager.moveAble = true;
                Variables.btnState = 0;

                foreach (var value in Variables.Characters.Values)
                {
                    if (gachaResult == value.InternalName)
                        charIndex = value.CharNumber;
                }

                var rankCharacter = Variables.Characters[charIndex];
                bool isUp = false;
                
                int progress, required;
                int beforeLevel = GameManager.Instance.CheckFavority(charIndex, 0, out progress, out required);
                if (required >= 0)
                    rankCharacter.Cards[0].Favority += 1;
                int currentLevel = GameManager.Instance.CheckFavority(charIndex, 0, out progress, out required);
                if (currentLevel > beforeLevel)
                    levelUp = true;

                GameManager.Instance.SaveGame();

                if (rankCharacter.Cards[0].Observed == false || rankCharacter.Cards[0].Favority == 1) // 첫 획득
                {
                    rankCharacter.Cards[0].Observed = true;
                    GameManager.Instance.SaveGame();
                    StartStory(Variables.returnSceneName, 0);
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
                if (isUp) // 호감도 Up!
                {
                    favorityUp.gameObject.SetActive(true);
                    favName.text = Variables.Characters[charIndex].Name;
                    favBar.maxValue = progress + required;
                    favBar.value = progress;
                    levelText.text = currentLevel.ToString();
                    favProgressText.text = progress + " / " + (progress + required);
                    favCharPicture.sprite = Resources.Load<Sprite>("Characters/" + rankCharacter.InternalName + "/default/image_obspopup");
                    if (levelUp)
                        favUp.gameObject.SetActive(true);

                    favConstellation.text = Variables.Constels[Variables.Characters[charIndex].ConstelKey[0]].Name;
                    //캐릭터 thumbnail
                }
                else
                    SceneManager.LoadScene(Variables.returnSceneName);
            }
        }
        else
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(favorityUp.gameObject.activeSelf)
                {
                    favorityUp.gameObject.SetActive(false);
                    if (levelUp)
                    {
                        startStory.gameObject.SetActive(true);
                        storyText.text = Variables.Characters[charIndex].Name + "의 새로운 대화가 열렸어요!\n대화를 지금 볼까요?";
                    }
                    else
                        SceneManager.LoadScene(Variables.returnSceneName);
                }
            }
        }
	}

    public void YesBtnDown() //Favority 오르고 나서 뜬 팝업창 Yes일 경우
    {
        StartStory(Variables.returnSceneName, nextFav + 1);
    }
    public void NoBtnDown() //Favority 오르고 나서 뜬 팝업창 Yes일 경우
    {
        SceneManager.LoadScene(Variables.returnSceneName);
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
        
        SoundManager.FadeMusicVolume(1, 1.5f);

        //NameTag FadeIn
        while (tC.a < 1f)
        {
            tC.a += Time.deltaTime;
            if (tC.a >= 1f)
                tC.a = 1f;
            textColor.a = tC.a;

            srnT.color = tC;
            tmnT.color = textColor;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //Show TTS
        yield return new WaitForSeconds(1f);
        TtS.SetActive(true);
        isEnd = true;
    }

}
