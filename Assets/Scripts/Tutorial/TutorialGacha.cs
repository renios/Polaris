using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observe;

namespace Tutorial
{
    public class TutorialGacha : MonoBehaviour
    {
        public GameObject tutTextPanel, tut1Panel, tut2Anim, tut3Panel, tut3Eff, hider;
        public Text tutText;
        public ObserveManager manager;

        bool canMoveTutStat;

        // Use this for initialization
        void Start()
        {
            ChangeState(Variables.tutState);
        }

        // Update is called once per frame
        void Update()
        {
            switch(Variables.tutState)
            {
                case 2:
                    if (Vector2.Distance(manager.Scope.transform.position, new Vector3(0.13f * 100.224f, 4.52f * 100.224f, -1)) < 10f)
                        ChangeState(++Variables.tutState);
                    break;
                case 3:
                    if(manager.Status.behaviour == ObserveBehaviour.Finished)
                        ChangeState(++Variables.tutState);
                    break;
                case 4:
                    if (manager.Status.behaviour == ObserveBehaviour.Idle)
                        Variables.tutState++;
                    break;
                case 7:
                    if (manager.Status.behaviour == ObserveBehaviour.Observing)
                        ChangeState(++Variables.tutState);
                    break;
                case 8:
                    if (manager.Status.behaviour == ObserveBehaviour.Finished)
                        ChangeState(++Variables.tutState);
                    break;
                case 9:
                    if (manager.Status.behaviour == ObserveBehaviour.Idle)
                        Variables.tutState++;
                    break;
            }

            if (canMoveTutStat && Input.GetMouseButtonDown(0))
                ChangeState(++Variables.tutState);
        }

        public void ChangeState(int state)
        {
            switch(state)
            {
                case 1:
                    tutTextPanel.SetActive(true);
                    tut1Panel.SetActive(true);
                    hider.SetActive(true);
                    tutText.text = "망원경으로 봐도 정말 까맣네...\n앗! 저기 뭔가가 반짝인다!";
                    manager.ButtonObj.GetComponent<Button>().interactable = false;
                    canMoveTutStat = true;
                    break;
                case 2:
                    tut2Anim.SetActive(true);
                    tutText.text = "조금 더 망원경 가운데 두고\n뭔지 확인해보자.";
                    ObserveManager.AllowMove = true;
                    canMoveTutStat = false;
                    break;
                case 3:
                    tut2Anim.SetActive(false);
                    tut3Panel.SetActive(true);
                    tut3Eff.SetActive(true);
                    manager.ButtonObj.GetComponent<Button>().interactable = true;
                    ObserveManager.AllowMove = false;
                    break;
                case 4:
                    tutText.text = "별빛이 모여서 반짝이고 있어?\n얼른 확인해보자!";
                    break;
                case 6: // EVERYTHING RESET
                    tut1Panel.SetActive(true);
                    tutTextPanel.SetActive(true);
                    tutText.text = "밤하늘에 별들이 생겼어!\n어쩌면 다른 별도 만날 수 있을 거야.";
                    manager.ButtonObj.GetComponent<Button>().interactable = false;
                    canMoveTutStat = true;
                    break;
                case 7:
                    tut1Panel.SetActive(false);
                    tutTextPanel.SetActive(false);
                    ObserveManager.AllowMove = true;
                    manager.ButtonObj.GetComponent<Button>().interactable = true;
                    canMoveTutStat = false;
                    break;
                case 8:
                    tutTextPanel.SetActive(true);
                    tutText.text = "이번엔 조금 더 시간이 필요한 건가?\n잠시 기다려보자.";
                    break;
                case 9:
                    tutText.text = "별빛이 다 모인 것 같아!\n이번에도 별을 만날 수 있겠지?";
                    tut3Panel.SetActive(true);
                    tut3Eff.SetActive(true);
                    break;
            }
        }
    }
}