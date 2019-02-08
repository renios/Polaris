using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueButton : MonoBehaviour
    {
        public int NextPhase;
        public Text Label;
        public DialogueObject ParentDialog;

        public void Clicked()
        {
            DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.GetButtonInput(ParentDialog, Label.text, NextPhase));
        }
    }
}