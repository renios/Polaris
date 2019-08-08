using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueInteractor : MonoBehaviour
    {
        public bool HasResult { get; set; }
        public int Result { get; set; }

        public RectTransform ButtonParent;
        public GameObject ButtonTemplate;

        private List<GameObject> buttons = new List<GameObject>();

        public IEnumerator Show(string[] texts, int[] nexts)
        {
            HasResult = false;
            while(buttons.Count > 0)
            {
                Destroy(buttons[0]);
                buttons.RemoveAt(0);
            }

            for(int i = 0; i < texts.Length; i++)
            {
                var newObj = Instantiate(ButtonTemplate);
                newObj.GetComponent<DialogueInteractButton>().Set(i, texts[i]);
                newObj.transform.SetParent(ButtonParent);
                newObj.transform.localScale = Vector3.one;
                newObj.SetActive(true);
                buttons.Add(newObj);
            }
            LayoutRebuilder.MarkLayoutForRebuild(ButtonParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ButtonParent);

            yield return new WaitUntil(() => HasResult);
        }
    }
}