using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reading
{
    public class ReadingDiary : MonoBehaviour
    {
        public GameObject popupBody;
        public GameObject pageTemplate;
        public RectTransform pageParent;
        public ReadingDiaryDetailPopup detailPopup;

        [HideInInspector] public bool isAnimating;

        int currentIndex;
        List<ReadingDiaryPage> pages;
        Dictionary<int, int> groupStartIdx;
        
        public void ConstructDiary()
        {
            // Constel Group에 따라 캐릭터를 모아 줍니다.
            var groupedChar = new Dictionary<int, Dictionary<int, CharacterData>>();
            foreach (var chr in Variables.Characters)
            {
                if (!groupedChar.ContainsKey(Variables.Constels[chr.Value.ConstelKey[0]].Group))
                    groupedChar.Add(Variables.Constels[chr.Value.ConstelKey[0]].Group, new Dictionary<int, CharacterData>());
                groupedChar[Variables.Constels[chr.Value.ConstelKey[0]].Group].Add(chr.Key, chr.Value);
            }
            
            // 'Group이 섞이지 않게, 페이지당 9명씩 캐릭터 생성'을 조건으로 페이지를 생성합니다.
            pages = new List<ReadingDiaryPage>();
            var groupPageCount = new Dictionary<int, int>();
            int totalPage = 0;
            ReadingDiaryPage targetPage = null;
            for (int i = 0; i < groupedChar.Count; i++)
            {
                groupPageCount.Add(i, 0);
                int charCount = 0;
                foreach(var chr in groupedChar[i])
                {
                    if (charCount % 9 == 0)
                    {
                        var newObj = Instantiate(pageTemplate);
                        var newPage = newObj.GetComponent<ReadingDiaryPage>();
                        newPage.SetGroup(i);
                        newObj.transform.SetParent(pageParent);
                        newObj.transform.localScale = Vector3.one;
                        newObj.transform.localPosition = new Vector3(540, 0);
                        newObj.SetActive(true);
                        pages.Add(newPage);
                        targetPage = newPage;
                        groupPageCount[i]++;
                        totalPage++;
                    }

                    targetPage.CreateElement(chr.Key);
                    charCount++;
                }
            }
            
            // 각 페이지에 페이지 번호를 부여합니다. 그리고, 분명히 페이지 생성은 그룹 순서대로 되었겠죠?
            groupStartIdx = new Dictionary<int, int>();
            int gi = -1, curTot = 0, curMy = 0;
            for (int i = 0; i < pages.Count; i++, curMy++)
            {
                if (curTot == curMy)
                {
                    gi++;
                    curTot = groupPageCount[gi];
                    curMy = 0;
                    groupStartIdx[gi] = i;
                }
                pages[i].ApplyIndices(curTot, curMy + 1, totalPage, i + 1);
            }
            
            // 첫 페이지가 먼저 보이도록 조정
            pages[0].transform.SetAsLastSibling();
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            popupBody.SetActive(true);

            NavigateTo(0);
        }

        public void NavigateOnce(bool direction)
        {
            StartCoroutine(NavigateOnce_Routine(direction));
        }

        public void NavigateTo(int index)
        {
            Debug.Log("Moving from " + currentIndex + " to " + groupStartIdx[index]);
            StartCoroutine(NavigateManyPage(groupStartIdx[index]));
        }

        IEnumerator Navigate(bool directionFlag, float waitSecond)
        {
            SoundManager.Play(SoundType.AlbumPage);
            // True: Increment, False: Decrement
            if (directionFlag)
            {
                var curPage = pages[0];
                pages.RemoveAt(0);
                pages.Add(curPage);
                
                pages[0].transform.SetAsLastSibling();
                currentIndex++;
                if (currentIndex >= pages.Count)
                    currentIndex = 0;
                
                yield return pages[0].AnimateMove(true, waitSecond, null);
            }
            else
            {
                var curPage = pages[0];
                var newPage = pages[pages.Count - 1];
                pages.RemoveAt(pages.Count - 1);
                pages.Insert(0, newPage);
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = pages.Count - 1;
                
                yield return curPage.AnimateMove(false, waitSecond, () =>
                {
                    curPage.transform.SetAsFirstSibling();
                    
                });
            }
        }

        IEnumerator NavigateOnce_Routine(bool direction)
        {
            isAnimating = true;
            yield return Navigate(direction, 0.3f);
            isAnimating = false;
        }

        IEnumerator NavigateManyPage(int to)
        {
            isAnimating = true;
            
            var from = currentIndex;
            if (from < to)
            {
                for (int i = from; i < to; i++)
                    yield return Navigate(true, i == to - 1 ? 0.3f : 0.1f);
            }
            else if(to < from)
            {
                for (int i = to; i < from; i++)
                    yield return Navigate(false, i == from - 1 ? 0.3f : 0.1f);
            }

            currentIndex = to;
            isAnimating = false;
        }

        public void ShowDetail(int charIndex)
        {
            detailPopup.Show(charIndex);
        }

        public void Hide()
        {
            popupBody.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}