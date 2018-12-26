using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
		SelectedDialogueData.SelectedCharacterName = 0;
		SelectedDialogueData.SelectedDialogueLevel = 0;
	}
	
	// Use this for initialization
	void Start ()
	{
		DialogueLevelSelectPanel.GetComponent<Button>().onClick.AddListener(delegate { DialogueLevelSelectPanel.SetActive(false); });
		InitializeLevelSelectButton();
		
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
		SelectedDialogueData.SelectedCharacterName = name;
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

	void SetDialogueLevel(int level)
	{
		SelectedDialogueData.SelectedDialogueLevel = level;
		Debug.Log(SelectedDialogueData.SelectedCharacterName + " " + level + "단계");

		GoToSingleDialogueScene();
	}

	void InitializeLevelSelectButton()
	{
		for (int i = 0; i < DialogueLevelSelectButtons.Count; i++)
		{
			var level = i;
			DialogueLevelSelectButtons[i].onClick.AddListener(delegate { SetDialogueLevel(level); });
		}
	}

	void GoToSingleDialogueScene()
	{
		SceneManager.LoadScene("SingleDialogue");
	}
}
