using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dialogue
{
    public class DialogueInput : MonoBehaviour, IPointerClickHandler
    {
        public bool GotInput { get; set; }
        public bool IsAllowed { get; set; }

        public void OnPointerClick(PointerEventData eventData)
        {
            //if(IsAllowed)
                GotInput = true;
        }
    }
}