using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace Album
{
    public class AlbumStoryElement : MonoBehaviour
    {
        public Text StoryHeader, StoryDescription;
        public Text priceText;
        public GameObject AddiInfoTemplate;
        public RectTransform AddiInfoParent;
        public GameObject InfoPanel, BuyButton, Mask;
        public AlbumCharInfo page;
        
        [HideInInspector] public int charIndex, storyIndex;

        int unlockPrice;
        
        private List<GameObject> addis = new List<GameObject>();

        public void Show(int chI, int stI)
        {
            while(addis.Count > 0)
            {
                Destroy(addis[0]);
                addis.RemoveAt(0);
            }

            InfoPanel.SetActive(true);
            charIndex = chI;
            storyIndex = stI;
            StoryHeader.text = Variables.Characters[chI].ChapterInfo[stI].Header;
            StoryDescription.text = Variables.Characters[chI].ChapterInfo[stI].Description;
            for(int i = 0; i < Variables.Characters[chI].ChapterInfo[stI].Additional.Length; i++)
            {
                var newObj = Instantiate(AddiInfoTemplate);
                newObj.GetComponentInChildren<Text>().text = Variables.Characters[chI].ChapterInfo[stI].Additional[i];
                newObj.transform.SetParent(AddiInfoParent);
                newObj.transform.localScale = Vector3.one;
                newObj.transform.localPosition = Vector3.zero;
                newObj.SetActive(true);
                addis.Add(newObj);
            }
            BuyButton.SetActive(false);
            Mask.SetActive(false);
        }

        public void SetBuyable(int chI, int stI)
        {
            charIndex = chI;
            storyIndex = stI;

            unlockPrice = Variables.StoryUnlockCost[storyIndex];
            priceText.text = unlockPrice.ToString();
            
            BuyButton.SetActive(true);
            InfoPanel.SetActive(false);
            Mask.SetActive(false);
        }

        public void UnlockStory()
        {
            StartCoroutine(UnlockStory_Routine());
        }

        IEnumerator UnlockStory_Routine()
        {
            if (!GameManager.Instance.MoneyPayable(MoneyType.MemorialPiece, unlockPrice))
                yield return MessageSet.Now.ShowNoMoneyAlert(MoneyType.MemorialPiece);
            else
            {
                var accepted = false;
                yield return MessageSet.Now.ShowMoneySpendAsk("스토리를 개방하시겠습니까?", MoneyType.MemorialPiece,
                    unlockPrice, r => { accepted = r; });
                if (accepted)
                {
                    GameManager.Instance.PayMoney(MoneyType.MemorialPiece, unlockPrice);
                    Variables.Characters[charIndex].StoryUnlocked++;
                    GameManager.Instance.SaveGame();
                    
                    page.UpdateStoryList(charIndex);
                }
            }
        }
    }
}