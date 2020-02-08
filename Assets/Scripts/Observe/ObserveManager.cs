using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Observe
{
    public class ObserveManager : MonoBehaviour
    {
        // Static property
        public static bool AllowMove { get; set; }
        public static bool DontApplySkyLevelAtMove { get; set; }
        public static string PickResult { get; set; }

        public ObserveStatus Status { get { return status; } }

        // Public field
        [Header("Observe Field Objects")]
        public GameObject Scope;
        public GameObject ScopeObservingEffect, ScopeFinishedEffect, ScopeClickEffect;
        [Header("Main Observe Button")]
        public Text ObservingTimeText;
        public GameObject ButtonObj;
        public Sprite ButtonNorm, ButtonShine;
        public GameObject ButtonTextNorm, ButtonTextFinish;
        [Header("Sky Area Objects")]
        public GameObject[] SkyArea;
        public GameObject[] SkyAreaEffects;
        public ObsSkyAreaBtn[] SkyAreaBtns;
        public GameObject[] UnlockAskPanels;
        [Header("Observe Status Display")]
        public Text ConstelName;
        public GameObject ConstelImage;
        public ObsCharElement[] CharDisplay;
        [Header("Observe Time Selecting")]
        public GameObject TimeConfirmPanel;
        public GameObject[] TimeConfirmBtn;
        public Button TimeOkRunBtn;
        public GameObject NoMoneyPanel;
        [Header("While Observing Panel")]
        public GameObject WhileObservingPanel;
        [Header("Animating after pick")]
        public GameObject DimmerPanel;
        [Header("Result Alert Panel")]
        public ObsPickResultShower ResultShower;

        // Private field
        int obsTimeIndex;
        bool isTouching;
        Vector3 startScopePos, startMousePos;
        ObserveStatus status;
        Dictionary<string, ConstelObsData> constelCharData = new Dictionary<string, ConstelObsData>();
        Dictionary<string, int> constelHitCount = new Dictionary<string, int>();

        // Constant field
        const int SCOPE_CIRCLE_COUNT = 10;
        const int SCOPE_UNIT_RAYS = 18;
        const float SCENE_SCALE = 100.224f;
        const float SKY_RADIUS = 4.2f * SCENE_SCALE;
        const float TOUCH_BOUND = 3.9f * SCENE_SCALE;
        const float SCOPE_RADIUS = 2.2f * SCENE_SCALE;

        Func<Vector2, bool>[] scopeAllowedAtPos =
        {
            p => Vector2.Distance(new Vector2(0, 4.53f * SCENE_SCALE), p) <= 1.1f * SCENE_SCALE,
            p =>
            {
                var deltaP = p - new Vector2(0, 4.53f * SCENE_SCALE);
                return deltaP.magnitude <= 1.1f * SCENE_SCALE || (deltaP.magnitude <= SKY_RADIUS && Vector2.Angle(Vector2.right, deltaP) >= 150 && Vector2.Angle(Vector2.right, deltaP) < 235);
            }
        };

        void Start()
        {
            SetConstel();
            AssignCharaToConstel();

            status = ObserveStatus.Load();
            if (status.isTutorial)
                status.isTutorial = false;
            if (!Variables.TutorialFinished && Variables.TutorialStep == 1)
                status.isTutorial = true;

            // Tutorial 2를 위한 초기화
            status.pickTryCount = Variables.GetStoreValue(1, Variables.StoreUpgradeLevel[1]);
            status.favIncrement = 1;
            
            ChangeBehaviour(status.behaviour);

            if(!status.isTutorial)
                CheckSkyAvailability();

            // 저번 관측 결과 요약을 유저가 보지 못 했을 때 == 저번 결과 데이터가 반영되지 않았을 때
            if (status.behaviour == ObserveBehaviour.Idle && !status.userChecked)
            {
                ResultShower.Show(status.charFavData);
                status.userChecked = true;
                status.Save();
            }

            isTouching = false;
        }

        public void SetConstel()
        {
            foreach(var constel in Variables.Constels.Keys)
            {
                constelCharData.Add(constel, new ConstelObsData(constel));
                constelHitCount.Add(constel, 0);
            }
        }

        public void AssignCharaToConstel()
        {
            foreach(var key in Variables.Characters.Keys)
            {
                if(key != 1 && Variables.Characters[key].Observable)
                {
                    for(int i = 0; i < Variables.Characters[key].ConstelKey.Length; i++)
                    {
                        constelCharData[Variables.Characters[key].ConstelKey[i]].AddCharacter(key, (float)Variables.Characters[key].ConstelWeight[i]);
                    }
                }
            }
        }

        void Update()
        {
            if (AllowMove)
                MoveScope();
        }

        void FixedUpdate()
        {
            if(status.behaviour == ObserveBehaviour.Observing)
            {
                var now = DateTime.Now;
                var diff = status.endTime - now;

                if (diff.TotalSeconds <= 0)
                    ChangeBehaviour(ObserveBehaviour.Finished);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            for (int i = 1; i <= SCOPE_CIRCLE_COUNT; i++)
            {
                for (int j = 0; j < i * SCOPE_UNIT_RAYS; j++)
                {
                    float r = SCOPE_RADIUS * (1f / SCOPE_CIRCLE_COUNT) * i;
                    float theta = 2 * Mathf.PI * ((float)j / (SCOPE_UNIT_RAYS * i)) + (2 * Mathf.PI / SCOPE_UNIT_RAYS / SCOPE_CIRCLE_COUNT * (i - 1));
                    Gizmos.DrawSphere(Scope.transform.position + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0), 3);
                }
            }
        }

        public void ChangeBehaviour(ObserveBehaviour newBehaviour)
        {
            status.behaviour = newBehaviour;
            status.Save();

            switch (status.behaviour)
            {
                case ObserveBehaviour.Idle:
                    ScopeObservingEffect.SetActive(false);
                    ScopeFinishedEffect.SetActive(false);
                    ScopeClickEffect.SetActive(false);
                    ButtonTextNorm.SetActive(true);
                    ButtonTextFinish.SetActive(false);
                    ObservingTimeText.gameObject.SetActive(false);
                    ButtonObj.GetComponent<Image>().sprite = ButtonNorm;

                    if (!status.isTutorial)
                        AllowMove = true;
                    if(Variables.ObserveSkyLevel >= 0)
                        ShotRay();
                    break;
                case ObserveBehaviour.Observing:
                    ScopeObservingEffect.SetActive(true);
                    ScopeFinishedEffect.SetActive(false);
                    ScopeClickEffect.SetActive(false);
                    ButtonTextNorm.SetActive(false);
                    ButtonTextFinish.SetActive(false);
                    ObservingTimeText.gameObject.SetActive(true);
                    ButtonObj.GetComponent<Image>().sprite = ButtonNorm;

                    AllowMove = false;
                    Scope.transform.position = new Vector3(status.scopePos[0], status.scopePos[1], status.scopePos[2]);
                    var centerConstel = GetCenterConstelKey();
                    if (centerConstel == "null")
                        ConstelName.text = "미지의 영역";
                    else
                        ConstelName.text = Variables.Constels[centerConstel].Name;
                    ConstelImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Constellations/Observation/" + centerConstel);

                    if (!status.isTutorial)
                        DisplayCharOnly();
                    break;
                case ObserveBehaviour.Finished:
                    ScopeObservingEffect.SetActive(false);
                    ScopeFinishedEffect.SetActive(true);
                    ScopeClickEffect.SetActive(false);
                    ButtonTextNorm.SetActive(false);
                    ButtonTextFinish.SetActive(true);
                    ObservingTimeText.gameObject.SetActive(false);
                    ButtonObj.GetComponent<Image>().sprite = ButtonShine;

                    AllowMove = false;
                    Scope.transform.position = new Vector3(status.scopePos[0], status.scopePos[1], status.scopePos[2]);
                    var centerConstel2 = GetCenterConstelKey();
                    if (centerConstel2 == "null")
                        ConstelName.text = "미지의 영역";
                    else
                        ConstelName.text = Variables.Constels[centerConstel2].Name;
                    ConstelImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Constellations/Observation/" + centerConstel2);

                    if (!status.isTutorial)
                        DisplayCharOnly();
                    break;
            }
        }

        public void MoveScope()
        {
            var center = new Vector3(0, 4.55f * SCENE_SCALE, -1);
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(Input.GetMouseButton(0))
            {
                if(!isTouching)
                {
                    startScopePos = Scope.transform.position;
                    startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    if (Vector2.Distance(startMousePos, center) < SKY_RADIUS)
                        isTouching = true;
                }
            }
            else
            {
                if (isTouching)
                    isTouching = false;
            }

            if(isTouching)
            {
                var scopePos = startScopePos + (mousePos - startMousePos);
                if(DontApplySkyLevelAtMove && Vector2.Distance(scopePos, center) > TOUCH_BOUND)
                {
                    var delta = scopePos - center;
                    scopePos = center + delta.normalized * TOUCH_BOUND;
                }
                if(!DontApplySkyLevelAtMove && Variables.ObserveSkyLevel >= 0 && !scopeAllowedAtPos[Variables.ObserveSkyLevel](scopePos))
                    scopePos = FindScopeBorderPos(Variables.ObserveSkyLevel, scopePos, 0.5f, 1);
                scopePos.z = -1f;
                Scope.transform.position = scopePos;

                ShotRay();
            }
        }

        Vector2 FindScopeBorderPos(int index, Vector2 targetPos, float divide_point, int iterate_count)
        {
            Vector2[] centerPos = { new Vector2(0, 4.53f * SCENE_SCALE), new Vector2(-1.67f * SCENE_SCALE, 4.64f * SCENE_SCALE) };

            var vec = Vector2.Lerp(centerPos[index], targetPos, divide_point);
            if (iterate_count >= 10 && scopeAllowedAtPos[index](vec))
                return vec;

            if (scopeAllowedAtPos[index](vec))
                return FindScopeBorderPos(index, targetPos, divide_point + Mathf.Pow(0.5f, iterate_count + 1), iterate_count + 1);
            else
                return FindScopeBorderPos(index, targetPos, divide_point - Mathf.Pow(0.5f, iterate_count + 1), iterate_count + 1);
        }

        public void ShotRay()
        {
            var pos = Scope.transform.position;
            constelHitCount = constelHitCount.ToDictionary(p => p.Key, p => 0);

            CastRay(pos);
            CastRay(pos);
            CastRay(pos);
            CastRay(pos);

            for (int i = 1; i <= SCOPE_CIRCLE_COUNT; i++)
            {
                for (int j = 0; j < i * SCOPE_UNIT_RAYS; j++)
                {
                    float r = SCOPE_RADIUS * (1f / SCOPE_CIRCLE_COUNT) * i;
                    float theta = 2 * Mathf.PI * ((float)j / (SCOPE_UNIT_RAYS * i)) + (2 * Mathf.PI / SCOPE_UNIT_RAYS / SCOPE_CIRCLE_COUNT * (i - 1));
                    pos = Scope.transform.position + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
                    CastRay(pos);
                }
            }

            status.charProb.Clear();
            foreach(var constel in constelHitCount)
            {
                foreach(var charIndex in constelCharData[constel.Key].charas)
                {
                    if (!status.charProb.ContainsKey(charIndex))
                        status.charProb.Add(charIndex, 0);
                    status.charProb[charIndex] += constel.Value * constelCharData[constel.Key].charWeights[charIndex];
                }
            }

            var centerConstel = GetCenterConstelKey();
            if (centerConstel == "null")
                ConstelName.text = "미지의 영역";
            else
                ConstelName.text = Variables.Constels[centerConstel].Name;
            ConstelImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Constellations/Observation/" + centerConstel);

            if (!status.isTutorial)
            {
                var orderedCharProb = status.charProb.OrderByDescending(p => p.Value);
                for (int i = 0; i < 4; i++)
                {
                    if (i >= orderedCharProb.Count())
                        CharDisplay[i].gameObject.SetActive(false);
                    else
                    {
                        CharDisplay[i].gameObject.SetActive(true);
                        CharDisplay[i].Set(orderedCharProb.ElementAt(i).Key);
                    }
                }
            }
        }

        public void CastRay(Vector3 pos)
        {
            var hit = Physics2D.Raycast(pos, Vector2.zero, 0);

            if (hit.collider != null)
            {
                if (constelCharData[hit.collider.name].observable && constelHitCount.ContainsKey(hit.collider.name))
                    constelHitCount[hit.collider.name]++;
            }
        }

        public string GetCenterConstelKey()
        {
            var hit = Physics2D.Raycast(Scope.transform.position, Vector2.zero, 0);

            if (hit.collider == null)
                return "null";
            else
                return hit.collider.name;
        }

        public void DisplayCharOnly()
        {
            var ordered = status.charProb.OrderByDescending(p => p.Value);
            for(int i = 0; i < 4; i++)
                CharDisplay[i].Set(ordered.ElementAt(i).Key);
        }

        public void ButtonPressed()
        {
            if(status.behaviour == ObserveBehaviour.Idle)
            {
                if (status.isTutorial)
                {
                    status.endTime = DateTime.Now.AddSeconds(3);
                    status.scopePos = new[] { Scope.transform.position.x, Scope.transform.position.y, Scope.transform.position.z };
                    ChangeBehaviour(ObserveBehaviour.Observing);
                }
                else
                {
                    for (int i = 0; i < TimeConfirmBtn.Length; i++)
                        TimeConfirmBtn[i].GetComponent<Image>().color = Color.white;
                    TimeOkRunBtn.interactable = false;
                    AllowMove = false;
                    TimeConfirmPanel.SetActive(true);
                }
            }
            else if(status.behaviour == ObserveBehaviour.Observing)
            {
                WhileObservingPanel.SetActive(true);
            }
            else if(status.behaviour == ObserveBehaviour.Finished)
            {
                ScopeFinishedEffect.SetActive(false);
                ScopeClickEffect.SetActive(true);
                SoundManager.Play(SoundType.ClickImportant);
                SoundManager.FadeMusicVolume(0, 1.5f);

                PickCharacter();

                status.behaviour = ObserveBehaviour.Idle;
                status.Save();

                DimmerPanel.SetActive(true);
                SceneChanger.ChangeScene("GachaResult", "GachaFadeIn", 1.5f);
            }
        }

        public void SetAllowMove(bool val)
        {
            AllowMove = val;
        }

        public void TimeBtnClicked(int index)
        {
            obsTimeIndex = index;
            for(int i = 0; i < TimeConfirmBtn.Length; i++)
            {
                if (i == index - 1)
                    TimeConfirmBtn[i].GetComponent<Image>().color = new Color32(255, 192, 255, 255);
                else
                    TimeConfirmBtn[i].GetComponent<Image>().color = Color.white;
            }
            TimeOkRunBtn.interactable = true;
        }

        public void TimeOkRunBtnClicked()
        {
            if (Variables.Starlight < TimeConfirmBtn[obsTimeIndex - 1].GetComponentInParent<ObserveTimeButton>().SpendMoney)
                NoMoneyPanel.SetActive(true);
            else
            {
                Variables.Starlight -= TimeConfirmBtn[obsTimeIndex - 1].GetComponentInParent<ObserveTimeButton>().SpendMoney;
                StartObserveWithTime();
                TimeConfirmPanel.SetActive(false);
            }
        }

        public void StartObserveWithTime()
        {
            float t = 0;
            switch(obsTimeIndex)
            {
                case 1:
                    t = UnityEngine.Random.Range(30f, 60f);
                    status.endTime = DateTime.Now.AddSeconds(t);
                    var prob = Mathf.InverseLerp(30, 60, t);
                    var dice = UnityEngine.Random.Range(0f, 1f);
                    if (dice >= prob)
                        status.favIncrement = 1;
                    else
                        status.favIncrement = 2;
                    break;
                case 2:
                    t = UnityEngine.Random.Range(10f, 30f);
                    status.endTime = DateTime.Now.AddMinutes(t);
                    status.favIncrement = Mathf.RoundToInt(Mathf.Pow(2 * t, 0.7536f) * UnityEngine.Random.Range(0.8f, 1.2f));
                    break;
                case 3:
                    t = UnityEngine.Random.Range(60f, 180f);
                    status.endTime = DateTime.Now.AddMinutes(t);
                    status.favIncrement = Mathf.RoundToInt(Mathf.Pow(2 * t, 0.7536f) * UnityEngine.Random.Range(0.8f, 1.2f));
                    break;
                case 4:
                    t = UnityEngine.Random.Range(360f, 720f);
                    status.endTime = DateTime.Now.AddMinutes(t);
                    status.favIncrement = Mathf.RoundToInt(Mathf.Pow(2 * t, 0.7536f) * UnityEngine.Random.Range(0.8f, 1.2f));
                    break;
                default:
                    Debug.LogError("No designated time table for this index.");
                    status.endTime = DateTime.Now.AddHours(1);
                    break;
            }
            status.pickTryCount = Variables.GetStoreValue(1, Variables.StoreUpgradeLevel[1]);
            status.scopePos = new[] { Scope.transform.position.x, Scope.transform.position.y, Scope.transform.position.z };
            ChangeBehaviour(ObserveBehaviour.Observing);
        }

        public void FastComplete()
        {
            if (Variables.Starlight < 300)
                NoMoneyPanel.SetActive(true);
            else
            {
                Variables.Starlight -= 300;
                ChangeBehaviour(ObserveBehaviour.Finished);
                ButtonPressed();
                WhileObservingPanel.SetActive(false);
            }
        }

        public void CancelObserving()
        {
            ChangeBehaviour(ObserveBehaviour.Idle);
        }

        void PickCharacter()
        {
            status.pickResult = new Dictionary<int, int>();
            status.charFavData = new Dictionary<int, int>();
            status.userChecked = false;

            if(status.isTutorial)
            {
                status.pickResult.Add(1, 1);
                status.isTutorial = false;
            }
            else
            {
                var sum = status.charProb.Sum(p => p.Value);
                Debug.Log("Sum of Prob coeff: " + sum);
                if (sum > 0)
                {
                    var orderedCharProb = status.charProb.OrderByDescending(p => p.Value);
                    
                    for(int i = 0; i < status.pickTryCount; i++)
                    {
                        Debug.Log("Operating Pick " + (i + 1));
                        float rnd = UnityEngine.Random.Range(0, sum);
                        Debug.Log("- rnd is: " + rnd);
                        int res = -1;
                        for (int j = 0; j < orderedCharProb.Count(); j++)
                        {
                            rnd -= orderedCharProb.ElementAt(j).Value;
                            Debug.Log("- Subtracting " + orderedCharProb.ElementAt(j).Value + ". Current rnd: " + rnd);

                            if (rnd <= 0.01f)
                            {
                                res = orderedCharProb.ElementAt(j).Key;
                                Debug.Log("Breaking, because it's smaller than 0.01f. Result key: " + res);
                                break;
                            }
                        }
                        if (status.pickResult.ContainsKey(res))
                            status.pickResult[res]++;
                        else
                            status.pickResult.Add(res, 1);
                    }
                }
                else
                    status.pickResult.Add(-1, 0);
            }

            var orderedRes = status.pickResult.OrderByDescending(p => p.Value);
            for (int i = 0; i < orderedRes.Count(); i++)
            {
                var charKey = orderedRes.ElementAt(i).Key;
                status.charFavData.Add(charKey, status.favIncrement * orderedRes.ElementAt(i).Value);

                if (!Variables.Characters[charKey].Observed)
                    status.charFavData[charKey] = 1;
                else
                {
                    float temp_prog;
                    int temp_required;
                    var nextFav = GameManager.Instance.CheckAfterFavority(charKey, status.charFavData[charKey], out temp_prog, out temp_required);
                    if (temp_required < 0)
                        status.charFavData[charKey] -= (int)temp_prog;
                }
            }
        }

        void CheckSkyAvailability()
        {
            var observedCharCount = new Dictionary<int, int>();
            foreach(var chara in Variables.Characters)
            {
                if(chara.Value.Observed)
                {
                    foreach(var constel in chara.Value.ConstelKey)
                    {
                        var groupIndex = Variables.Constels[constel].Group;
                        if (observedCharCount.ContainsKey(groupIndex))
                            observedCharCount[groupIndex]++;
                        else
                            observedCharCount.Add(groupIndex, 1);
                    }
                }
            }

            // 각 구역에 따른 조건 판단. 현재는 [0] 북극, [1] 겨울. parameter는 sky level. 나중에 조정 예정.
            Func<bool>[] allowedToOpen =
            {
                () => observedCharCount[0] > 0,     // 북극 지역의 캐릭터를 관측했는가
                () => observedCharCount[0] >= 4     // 북극 지역의 캐릭터를 4명 이상 관측했는가
            };
            for(int i = 0; i < SkyAreaBtns.Length; i++)
            {
                if (i <= Variables.ObserveSkyLevel)
                {
                    SkyAreaBtns[i].gameObject.SetActive(false);
                    SkyArea[i + 1].SetActive(true);
                }
                else
                {
                    SkyAreaBtns[i].gameObject.SetActive(true);
                    if (allowedToOpen[i]())
                        SkyAreaBtns[i].pricePanel.SetActive(true);
                    else
                        SkyAreaBtns[i].openRulePanel.SetActive(true);
                }
            }
        }

        public void UnlockSky(int index)
        {
            switch(index)
            {
                case 0:
                    Variables.Starlight -= 10;
                    break;
                case 1:
                    Variables.Starlight -= 300;
                    break;
            }
            StartCoroutine(SkyUnlockAnim(index));
        }

        IEnumerator SkyUnlockAnim(int index)
        {
            Variables.ObserveSkyLevel = index;
            GameManager.Instance.SaveGame();

            AllowMove = false;
            Scope.gameObject.SetActive(false);
            ButtonObj.GetComponent<Button>().interactable = false;

            foreach (var cap in SkyAreaBtns)
                cap.gameObject.SetActive(false);
            SkyAreaEffects[index].SetActive(true);
            yield return new WaitForSeconds(4);
            SkyArea[index + 1].SetActive(true);
            SkyArea[index + 1].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            SkyArea[index + 1].GetComponent<SpriteRenderer>().DOColor(Color.white, 1.5f);
            yield return new WaitForSeconds(1.5f);
            SkyArea[index].SetActive(false);

            CheckSkyAvailability();
            foreach (var constel in constelCharData)
                constel.Value.CheckObservability();
            ButtonObj.GetComponent<Button>().interactable = true;
            Scope.gameObject.SetActive(true);
            AllowMove = true;

            ShotRay();
        }
    }
}