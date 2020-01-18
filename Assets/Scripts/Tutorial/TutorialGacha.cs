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
        float internalCount;

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
                    if (Vector2.Distance(manager.Scope.transform.position, new Vector3(0.2f * 100.224f, 4.6f * 100.224f, -1)) < 15f)
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
                case 6:
                    if (Variables.ObserveSkyLevel == 0)
                    {
                        internalCount += Time.deltaTime;
                        if(internalCount >= 5.5f)
                            ChangeState(++Variables.tutState);
                    }
                    break;
                case 8:
                    if (manager.Status.behaviour == ObserveBehaviour.Observing)
                        ChangeState(++Variables.tutState);
                    break;
                case 9:
                    if (manager.Status.behaviour == ObserveBehaviour.Finished)
                        ChangeState(++Variables.tutState);
                    break;
                case 10:
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
                    manager.SkyArea[0].SetActive(false);
                    ObserveManager.AllowMove = false;
                    ObserveManager.DontApplySkyLevelAtMove = true;
                    tutText.text = "망원경으로 봐도 정말 까맣네...\n앗! 저기 뭔가가 반짝인다!";
                    manager.ButtonObj.GetComponent<Button>().interactable = false;
                    canMoveTutStat = true;
                    break;
                case 2:
                    ObserveManager.AllowMove = true;
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
                case 6:
                    internalCount = 0;
                    tut1Panel.SetActive(true);
                    tutTextPanel.SetActive(true);
                    manager.ButtonObj.GetComponent<Button>().interactable = false;
                    manager.Scope.transform.position = new Vector3(0, 4.53f * 100.224f, 0);
                    ObserveManager.AllowMove = false;
                    ObserveManager.DontApplySkyLevelAtMove = false;
                    Variables.Starlight += 110;
                    tutText.text = "밤하늘이 다시 깜깜해졌네...\n어? 하늘에 뭔가 있어! 눌러보자.";
                    break;
                case 7:
                    tutText.text = "밤하늘에 별들이 생겼어!\n이제 어쩌면 다른 별도 만날 수 있을 거야.";
                    canMoveTutStat = true;
                    break;
                case 8:
                    tut1Panel.SetActive(false);
                    tutTextPanel.SetActive(false);
                    ObserveManager.AllowMove = true;
                    manager.ButtonObj.GetComponent<Button>().interactable = true;
                    canMoveTutStat = false;
                    break;
                case 9:
                    tutTextPanel.SetActive(true);
                    tutText.text = "이번엔 조금 더 시간이 필요한 건가?\n잠시 기다려보자.";
                    break;
                case 10:
                    tutText.text = "별빛이 다 모인 것 같아!\n이번에도 별을 만날 수 있겠지?";
                    tut3Panel.SetActive(true);
                    tut3Eff.SetActive(true);
                    break;
            }
        }
    }
}