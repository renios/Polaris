using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Observe
{
    public class ObserveManager : MonoBehaviour
    {
        // Static property
        public static bool AllowMove { get; set; }
        public static string PickResult { get; set; }

        // Public field
        public GameObject Scope;
        public GameObject ScopeObservingEffect, ScopeFinishedEffect, ScopeClickEffect;
        public Text ObservingTimeText;
        public GameObject ButtonObj;
        public Sprite ButtonNorm, ButtonShine;
        public GameObject ButtonTextNorm, ButtonTextFinish;
        public Text ConstelName;
        public ObsCharElement[] CharDisplay;
        public GameObject DimmerPanel;

        // Private field
        float observeTime = 16f;
        bool isTouching;
        Vector3 startScopePos, startMousePos;
        ObserveStatus status;
        Dictionary<string, ConstelObsData> constelCharData = new Dictionary<string, ConstelObsData>();
        Dictionary<string, int> constelHitCount = new Dictionary<string, int>();

        // Constant field
        const int SCOPE_CIRCLE_COUNT = 10;
        const int SCOPE_UNIT_RAYS = 18;
        const float SCENE_SCALE = 100.224f;
        const float SKY_RADIUS = 4.6f * SCENE_SCALE;
        const float TOUCH_BOUND = 3.9f * SCENE_SCALE;
        const float SCOPE_RADIUS = 1.5f * SCENE_SCALE;

        void Start()
        {
            SetConstel();
            AssignCharaToConstel();

            status = ObserveStatus.Load();
            ChangeBehaviour(status.behaviour);
            if (Variables.isFirst)
                status.isTutorial = true;

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
                if(key != 1 && Variables.Characters[key].Cards[0].Observable)
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
                ObservingTimeText.text = diff.Minutes.ToString("D2") + ":" + diff.Seconds.ToString("D2");

                if (diff.TotalSeconds <= 0)
                    ChangeBehaviour(ObserveBehaviour.Finished);
            }
        }

        public void ChangeBehaviour(ObserveBehaviour newBehaviour)
        {
            status.behaviour = newBehaviour;
            status.Save();

            switch(status.behaviour)
            {
                case ObserveBehaviour.Idle:
                    ScopeObservingEffect.SetActive(false);
                    ScopeFinishedEffect.SetActive(false);
                    ScopeClickEffect.SetActive(false);
                    ButtonTextNorm.SetActive(true);
                    ButtonTextFinish.SetActive(false);
                    ObservingTimeText.gameObject.SetActive(false);
                    ButtonObj.GetComponent<Image>().sprite = ButtonNorm;

                    AllowMove = true;
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
                    break;
                    DisplayCharOnly();
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
                if(Vector2.Distance(scopePos, center) > TOUCH_BOUND)
                {
                    var delta = scopePos - center;
                    scopePos = center + delta.normalized * TOUCH_BOUND;
                }
                scopePos.z = -1f;
                Scope.transform.position = scopePos;

                Variables.scopePos = Scope.transform.localPosition;
                ShotRay();
            }
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

        public void CastRay(Vector3 pos)
        {
            var hit = Physics2D.Raycast(pos, Vector2.zero, 0);

            if (hit.collider != null)
            {
                if (constelHitCount.ContainsKey(hit.collider.name))
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
                status.endTime = DateTime.Now.AddSeconds(observeTime);
                status.scopePos = new[] { Scope.transform.position.x, Scope.transform.position.y, Scope.transform.position.z };
                ChangeBehaviour(ObserveBehaviour.Observing);
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
                Variables.returnSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                SceneChanger.ChangeScene("GachaResult", "GachaFadeIn", 1.5f);
            }
        }

        void PickCharacter()
        {
            if(status.isTutorial)
            {
                PickResult = "polaris";
                status.isTutorial = false;
            }
            else
            {
                var sum = status.charProb.Sum(p => p.Value);
                Debug.Log("Sum of Prob coeff: " + sum);
                if (sum > 0)
                {
                    var orderedCharProb = status.charProb.OrderByDescending(p => p.Value);
                    float rnd = UnityEngine.Random.Range(0, sum);
                    Debug.Log("- rnd is: " + rnd);
                    int res = -1;
                    for (int i = 0; i < orderedCharProb.Count(); i++)
                    {
                        rnd -= orderedCharProb.ElementAt(i).Value;
                        Debug.Log("- Deducting " + orderedCharProb.ElementAt(i).Value + ". Current rnd: " + rnd);

                        if (rnd <= 0.01f)
                        {
                            res = orderedCharProb.ElementAt(i).Key;
                            Debug.Log("Breaking, because it's smaller than 0.01f. Result key: " + res);
                            break;
                        }
                    }
                    PickResult = Variables.Characters[res].InternalName;
                }
                else
                    PickResult = "noStarError";
            }
        }
    }
}