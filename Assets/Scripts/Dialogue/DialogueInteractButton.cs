using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueInteractButton : MonoBehaviour
    {
        public int Index;
        public Text Context;

        public void Set(int next, string text)
        {
            Index = next;
            Context.text = text;
        }

        public void Clicked()
        {
            DialogueManager.Instance.Interactor.Result = Index;
            DialogueManager.Instance.Interactor.HasResult = true;
        }
    }
}