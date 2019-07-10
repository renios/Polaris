using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public struct DialogueData
    {
        public DialoguePhase[] Dialogues;
    }

    public struct DialoguePhase
    {
        public int Phase;
        public DialogueContent[] Contents;
    }

    public struct DialogueContent
    {
        public int Type;
        public string Talker;
        public string DialogText;
        public string[] JuncTexts;
        public int[] Directions;
        public int NextPhase;
        public string ImageKey;
        public string BgmKey;
        public string EffectKey;
    }
}