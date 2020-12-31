using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Observe;

namespace Tutorial
{
    public class TutorialGacha : MonoBehaviour
    {
        public GameObject tutTextPanel, uiDimmerPanel, fingerAnim, buttonFingerPanel, buttonAuraEff, hider;
        public GameObject helpPanel1, helpPanel2, helpPanel3;
        public Text tutText;
        public ObserveManager manager;

        bool canMoveTutStat;
        float internalCount;

        // Use this for initialization
        void Start()
        {
            ChangeState(Variables.TutorialStep);
        }

        // Update is called once per frame
        void Update()
        {
            switch(Variables.TutorialStep)
            {
                case 2:
                    if (Vector2.Distance(manager.Scope.transform.position, new Vector3(-0.15f * 100.224f, 4.52f * 100.224f, -1)) < 15f)
                        ChangeState(++Variables.TutorialStep);
                    break;
                case 3:
                    if(manager.Status.behaviour == ObserveBehaviour.Observing)
                        ChangeState(++Variables.TutorialStep);
                    break;
                case 4:
                    if (manager.Status.behaviour == ObserveBehaviour.Finished)
                        ChangeState(++Variables.TutorialStep);
                    break;
                case 11:
                    if (Variables.ObserveSkyLevel == 0)
                        ChangeState(++Variables.TutorialStep);
                    break;
                case 12:
                    internalCount += Time.deltaTime;
                    if (internalCount >= 6)
                        ChangeState(++Variables.TutorialStep);
                    break;
                case 13:
                    if (manager.Status.behaviour == ObserveBehaviour.Observing)
                        ChangeState(++Variables.TutorialStep);
                    break;
                case 14:
                    if (manager.Status.behaviour == ObserveBehaviour.Finished)
                        ChangeState(++Variables.TutorialStep);
                    break;
            }

            if (canMoveTutStat && Input.GetMouseButtonDown(0))
                ChangeState(++Variables.TutorialStep);
        }

        public void ChangeState(int state)
        {
            switch(state)
            {
                case 1:
                    // 1단계: 첫 번째 튜토리얼 관측 씬 진입
                    // 다음 단계 이동: 터치 시
                    tutTextPanel.SetActive(true);
                    uiDimmerPanel.SetActive(true);
                    hider.SetActive(true);
                    manager.SkyArea[0].SetActive(false);
                    ObserveManager.AllowMove = false;
                    ObserveManager.DontApplySkyLevelAtMove = true;
                    tutText.text = "망원경으로 봐도 정말 까맣네...\n앗! 저기 뭔가가 반짝인다!";
                    manager.ButtonObj.GetComponent<Button>().interactable = false;

                    canMoveTutStat = true;
                    break;
                case 2:
                    // 2단계: 첫 번째 튜토리얼 관측 이동 진행
                    // 다음 단계 이동: 망원경을 가운데 뒀을 시 (Update func)
                    fingerAnim.SetActive(true);
                    tutText.text = "조금 더 망원경을 가운데 두고\n뭔지 확인해보자.";
                    ObserveManager.AllowMove = true;

                    canMoveTutStat = false;
                    break;
                case 3:
                    // 3단계: 첫 번째 튜토리얼 관측 시작 준비 (시작 버튼 강조)
                    // 다음 단계 이동: 시작 버튼이 눌렸을 때 == 관측 상태가 Observing이 되었을 때 (Update func)
                    fingerAnim.SetActive(false);
                    buttonFingerPanel.SetActive(true);
                    buttonAuraEff.SetActive(true);
                    manager.ButtonObj.GetComponent<Button>().interactable = true;
                    ObserveManager.AllowMove = false;
                    break;
                case 4:
                    // 4단계: 첫 번째 튜토리얼 관측 중...
                    // 다음 단계 이동: 관측이 완료 되었을 때 == 관측 상태가 Finished가 되었을 때 (Update func)
                    buttonFingerPanel.SetActive(false);
                    buttonAuraEff.SetActive(false);
                    tutText.text = "시간이 좀 걸리는 건가......?";
                    break;
                case 5:
                    // 5단계: 첫 번째 튜토리얼 관측 완료 및 관측(1) 안내창
                    // 다음 단계 이동: -
                    buttonFingerPanel.SetActive(true);
                    buttonAuraEff.SetActive(true);
                    helpPanel1.SetActive(true);
                    tutText.text = "별빛이 모여서 반짝이고 있어!\n얼른 확인해보자!";
                    break;
                case 11:
                    // 8단계: 두 번째 튜토리얼 관측 씬 진입
                    // 다음 단계 이동: 하늘이 열리기 시작하는 시점
                    internalCount = 0;
                    uiDimmerPanel.SetActive(true);
                    tutTextPanel.SetActive(true);
                    manager.ButtonObj.GetComponent<Button>().interactable = false;
                    manager.Scope.transform.position = new Vector3(0, 4.53f * 100.224f, 0);
                    ObserveManager.AllowMove = false;
                    ObserveManager.DontApplySkyLevelAtMove = false;
                    tutText.text = "밤하늘이 다시 깜깜해졌네...\n어? 하늘에 뭔가 있어! 눌러보자.";

                    canMoveTutStat = false;
                    break;
                case 12:
                    // 9단계: 두 번째 튜토리얼 관측 하늘 개방 중...
                    // 다음 단계 이동: 하늘이 다 열렸을 때
                    tutTextPanel.SetActive(false);
                    break;
                case 13:
                    // 10단계: 두 번째 튜토리얼 하늘이 열렸다 & 관측(2) 안내창
                    // 다음 단계 이동: 관측이 시작 되었을 때
                    helpPanel2.SetActive(true);
                    tutTextPanel.SetActive(true);
                    tutText.text = "하늘에 별들이 더 많이 보여!\n이번엔 원하는 위치로 움직여서\n마음에 드는 곳을 관측해보자.";
                    ObserveManager.AllowMove = true;
                    manager.ButtonObj.GetComponent<Button>().interactable = true;
                    break;
                case 14:
                    // 11단계: 두 번째 튜토리얼 관측 시작! & 관측(3) 안내창
                    // 다음 단계 이동: 관측이 끝났을 때
                    tutTextPanel.SetActive(false);
                    helpPanel3.SetActive(true);
                    break;
                case 15:
                    // 12단계: 두 번째 튜토리얼 관측 끝
                    tutTextPanel.SetActive(true);
                    tutText.text = "별빛이 다 모인 것 같아!\n이번에도 별을 만날 수 있겠지?";
                    buttonFingerPanel.SetActive(true);
                    buttonAuraEff.SetActive(true);
                    // 임시 튜토리얼 종료 플래그
                    Variables.TutorialFinished = true;
                    break;
            }
        }
    }
}