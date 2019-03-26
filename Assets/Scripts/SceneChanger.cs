using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    public Animator Motion;
    public LoadingBar Bar;
    private bool isChanging;

    private void Awake()
    {
        Instance = this;
        isChanging = false;
    }

    public static void ChangeScene(string sceneName, string motionName = "SceneFadeOut", float motionTime = 0.5f)
    {
        if (!Instance.isChanging)
        {
            Instance.StartCoroutine(Instance.ChangeSceneSub(sceneName, motionName, motionTime));
            Instance.isChanging = true;
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (!isChanging)
        {
            StartCoroutine(Instance.ChangeSceneSub(sceneName, "SceneFadeOut", 0.5f));
            isChanging = true;
        }
    }

    public IEnumerator ChangeSceneSub(string sceneName, string motionName, float motionTime)
    {
        if (GetCurrentScene() != "TitleScene")
        {
            SoundManager.Play(SoundType.ClickImportant);
        }
        
        Motion.Play(motionName);
        yield return new WaitForSeconds(motionTime);

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loading.allowSceneActivation = false;
        while (!loading.isDone)
        {
            Bar.SetValue(loading.progress);
            if (loading.progress >= 0.9f)
            {
                loading.allowSceneActivation = true;
            }
            yield return null;
        }
        isChanging = false;
    }

    public static string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }
}