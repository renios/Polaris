using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
    [RequireComponent(typeof(Text))]
    public class ObsTimeDisplayer : MonoBehaviour
    {
        public string colorCode;
        [TextArea]
        public string prefix;
        [TextArea]
        public string suffix;
        public ObserveManager manager;

        void FixedUpdate()
        {
            if(manager.Status.behaviour == ObserveBehaviour.Observing)
            {
                var now = DateTime.Now;
                var diff = manager.Status.endTime - now;
                GetComponent<Text>().text = prefix + "<color=" + colorCode + ">" +
                    (diff.Hours > 0 ? (diff.Hours.ToString() + "시간 ") : "") +
                    (diff.Hours > 0 || diff.Minutes > 0 ? (diff.Minutes.ToString("D2") + "분 ") : "") +
                    diff.Seconds.ToString("D2") + "초</color>" + suffix;
            }
        }
    }

}