using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RichTextSubstringHelper;

namespace Dialogue
{
    public class DialogueDisplayer : MonoBehaviour
    {
        public bool IsFullyAnimated { get; private set; }

        public Text Talker, Context;
        public Image ForeImage;
        public GameObject EndIndicator;
        public Sprite DefaultForeSprite;

        public void ClearAll()
        {
            //ForeImage.sprite = DefaultForeSprite;
            //Talker.text = "";
            Context.text = "";
            IsFullyAnimated = true;
            EndIndicator.SetActive(false);
        }

        public void ClearText()
        {
            //Talker.text = "";
            Context.text = "";
        }

        public IEnumerator DisplayText(string talker, string context)
        {
            IsFullyAnimated = false;

            if(talker != null)
                Talker.text = talker;
            yield return TextAnimating(context);
        }

        public void DisplayForeImage(string imageKey)
        {
            //ForeImage.sprite = Resources.Load<Sprite>(DialogueManager.ImageRootPath + imageKey);
            if (imageKey == "default")
                ForeImage.sprite = Resources.Load<Sprite>(DialogueManager.DialogRoot + "image_dialogue");
            else
                ForeImage.sprite = Resources.Load<Sprite>(DialogueManager.DialogRoot + "image_dialogue_" + imageKey);
        }

        IEnumerator TextAnimating(string context)
        {
            int rtMaxCount = context.RichTextLength();
            int rtCount = 0;

            while (!IsFullyAnimated)
            {
                if(DialogueManager.Instance.Receiver.GotInput)
                {
                    Context.text = context;
                    DialogueManager.Instance.Receiver.GotInput = false;
                    IsFullyAnimated = true;
                }
                else
                {
                    Context.text = context.RichTextSubString(rtCount);
                    if (rtCount >= rtMaxCount)
                    {
                        IsFullyAnimated = true;
                        DialogueManager.Instance.Receiver.IsAllowed = true;
                    }
                    else
                    {
                        rtCount++;
                        yield return new WaitForSeconds(1f / 50);
                    }
                }
            }
            EndIndicator.SetActive(true);
            yield return new WaitUntil(() => (DialogueManager.Instance.Receiver.GotInput));
            DialogueManager.Instance.Receiver.GotInput = false;
            EndIndicator.SetActive(false);
        }
    }
}