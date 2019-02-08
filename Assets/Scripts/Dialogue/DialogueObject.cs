using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Dialogue
{
    public class DialogueObject : MonoBehaviour
    {
        public Text DialogText;
        public GameObject ButtonTemplate;
        public RectTransform ButtonParent;

        public void Set(DialogueContent content)
        {
            if (content.Type == 2)
            {
                for (int i = 0; i < content.JuncTexts.Length; i++)
                {
                    var newObject = Instantiate(ButtonTemplate);
                    var newButton = newObject.GetComponent<DialogueButton>();
                    newButton.Label.text = content.JuncTexts[i];
                    newButton.NextPhase = content.Directions[i];
                    newObject.transform.SetParent(ButtonParent);
                    newObject.SetActive(true);
                }
            }
            else
                DialogText.text = content.DialogText;

            transform.localScale = Vector3.zero;
        }

        private void Start()
        {
            transform.DOScale(1, 0.25f);
        }

        public IEnumerator Delete()
        {
            yield return transform.DOScale(0, 0.25f);
            gameObject.SetActive(false);
        }
    }
}