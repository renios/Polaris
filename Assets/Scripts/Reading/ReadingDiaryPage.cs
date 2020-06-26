using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Reading
{
    public class ReadingDiaryPage : MonoBehaviour
    {
        public GameObject[] bookmarks;
        public GameObject elementTemplate;
        public RectTransform elementParent;
        public Image seasonIcon;
        public Sprite[] seasonSprites;
        public Text groupNameText, groupIndexText, totalIndexText;

        int groupMaxIdx, groupMyIdx, totalMaxIdx, totalMyIdx;

        public void SetGroup(int constelGroup)
        {
            bookmarks[constelGroup].SetActive(true);
            groupNameText.text = Variables.ConstelGroupName[constelGroup];
            seasonIcon.sprite = seasonSprites[constelGroup];
        }
        
        public void CreateElement(int charIndex)
        {
            var newObj = Instantiate(elementTemplate);
            var newElement = newObj.GetComponent<ReadingDiaryPageElement>();
            newElement.Set(charIndex);
            newObj.transform.SetParent(elementParent);
            newObj.transform.localScale = Vector3.one;
            newObj.transform.localPosition = Vector3.zero;
            newObj.SetActive(true);
        }
        
        public void ApplyIndices(int gM, int gm, int tM, int tm)
        {
            groupMaxIdx = gM;
            groupMyIdx = gm;
            totalMaxIdx = tM;
            totalMyIdx = tm;

            groupIndexText.text = groupMyIdx + "/" + groupMaxIdx;
            totalIndexText.text = totalMyIdx + " / " + totalMaxIdx;
        }

        public IEnumerator AnimateMove(bool directionFlag, float waitSecond, System.Action afterAnimate)
        {
            if (directionFlag) // True: Incoming, False: Outgoing
            {
                transform.localEulerAngles = new Vector3(0, 90, 0);
                transform.DOLocalRotate(Vector3.zero, 0.3f);
                yield return new WaitForSeconds(waitSecond);
                if(afterAnimate != null)
                    afterAnimate.Invoke();
            }
            else
            {
                transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f);
                yield return new WaitForSeconds(waitSecond);
                transform.localEulerAngles = Vector3.zero;
                if(afterAnimate != null)
                    afterAnimate.Invoke();
            }
        }
    }
}