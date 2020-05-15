using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class LobbyTutorial : MonoBehaviour
    {
        public GameObject tutToAlbumPanel;

        // Use this for initialization
        void Start()
        {
            ChangeState(Variables.TutorialStep);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeState(int state)
        {
            switch(state)
            {
                case 5:
                    tutToAlbumPanel.SetActive(true);
                    break;
            }
        }
    }
}