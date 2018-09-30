using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueSelectButtonManager : MonoBehaviour
{

	public List<Button> DialogueSelectButtons;
	public Button LeftArrowButton;
	public Button RightArrowButton;
	public Text PageText;
	public GameObject DialogueLevelSelectPanel;
	public List<Button> DialogueLevelSelectButtons;
	
	private List<CharacterName> characterNameList;
	private int maxPage;
	private int currentPage = 0;
	private int numberOfButtons;

	void Awake()
	{
		
	}
	
	// Use this for initialization
	void Start ()
	{
		DialogueLevelSelectPanel.GetComponent<Button>().onClick.AddListener(delegate { DialogueLevelSelectPanel.SetActive(false); });
		
		numberOfButtons = DialogueSelectButtons.Count;
		maxPage = Mathf.CeilToInt(Enum.GetNames(typeof(CharacterName)).Length / (float) numberOfButtons);
		LoadDialogueData();
		
		LeftArrowButton.onClick.AddListener(delegate { ClickArrowButton(-1); });
		RightArrowButton.onClick.AddListener(delegate { ClickArrowButton(1); });
		PageText.text = (currentPage + 1) + "/" + maxPage;
		
		SetFirstPage();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SetFirstPage()
	{
		for (int i = 0; i < numberOfButtons; i++)
		{
			if (i < characterNameList.Count)
			{
				CharacterName name = characterNameList[i];
				DialogueSelectButtons[i].enabled = true;
				DialogueSelectButtons[i].GetComponentInChildren<Text>().text = name.ToString();
				DialogueSelectButtons[i].onClick.AddListener(delegate { ClickNameButton(name); });
			}
			else
			{
				DialogueSelectButtons[i].enabled = false;
				DialogueSelectButtons[i].GetComponentInChildren<Text>().text = "--";
			}
		}
		

		LeftArrowButton.gameObject.SetActive(false);
	}

	void ClickNameButton(CharacterName name)
	{
		// TODO: 클릭한 캐릭터 이름 저장
		
		DialogueLevelSelectPanel.SetActive(true);
	}

	void ClickArrowButton(int change)
	{
		currentPage += change;

		LeftArrowButton.gameObject.SetActive(currentPage != 0);
		RightArrowButton.gameObject.SetActive(currentPage != maxPage-1);
		PageText.text = (currentPage + 1) + "/" + maxPage;
		
		for (int i = 0; i < numberOfButtons; i++)
		{
			if (currentPage * numberOfButtons + i < characterNameList.Count)
			{
				CharacterName name = characterNameList[currentPage * numberOfButtons + i];
				DialogueSelectButtons[i].enabled = true;
				DialogueSelectButtons[i].GetComponentInChildren<Text>().text = name.ToString();
				DialogueSelectButtons[i].onClick.AddListener(delegate { ClickNameButton(name); });
			}
			else
			{
				DialogueSelectButtons[i].enabled = false;
				DialogueSelectButtons[i].GetComponentInChildren<Text>().text = "--";
			}
		}
	}

	void LoadDialogueData()
	{
		characterNameList = new List<CharacterName>();
		for (int i = 0; i < Enum.GetNames(typeof(CharacterName)).Length; i++)
		{
			characterNameList.Add((CharacterName) i);
		}
	}

//	for (int j = 0; j < 7; j++)
//	{
//		switch (j) {
//			case 0: childItemData.mName = "만남"; break;
//			case 1: childItemData.mName = "1단계"; break;
//			case 2: childItemData.mName = "2단계"; break;
//			case 3: childItemData.mName = "3단계"; break;
//			case 4: childItemData.mName = "4단계"; break;
//			case 5: childItemData.mName = "5단계"; break;
//			case 6: childItemData.mName = "결말"; break;
//			default: childItemData.mName = "--"; break;
//		}
//	}
}
