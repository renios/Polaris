using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    public Animator Motion;
    public LoadingBar Bar;

    private void Awake()
    {
        Instance = this;
    }

    public static void ChangeScene(string sceneName, string motionName = "SceneFadeOut", float motionTime = 0.5f)
    {
        Instance.StartCoroutine(Instance.ChangeSceneSub(sceneName, motionName, motionTime));
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(Instance.ChangeSceneSub(sceneName, "SceneFadeOut", 0.5f));
    }

    public IEnumerator ChangeSceneSub(string sceneName, string motionName, float motionTime)
    {
        SoundManager.Play(SoundType.ClickImportant);
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
    }

    public static string GetCurrentScene()
    {
        return SceneManager.GetActiveScene().name;
    }
}