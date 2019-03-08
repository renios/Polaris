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

    public static void ChangeScene(string sceneName, int camera, string motionName = "SceneFadeOut", float motionTime = 0.5f)
    {
        Instance.StartCoroutine(Instance.ChangeSceneSub(sceneName, motionName, motionTime, camera));
    }

    public void ChangeScene(string sceneName, int camera)
    {
        StartCoroutine(Instance.ChangeSceneSub(sceneName, "SceneFadeOut", 0.5f, camera));
    }

    public IEnumerator ChangeSceneSub(string sceneName, string motionName, float motionTime, int camera)
    {
        Motion.Play(motionName);
        yield return new WaitForSeconds(motionTime);

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);
        loading.allowSceneActivation = false;
        while (!loading.isDone)
        {
            Bar.SetValue(loading.progress);
            if (loading.progress >= 0.9f)
            {
                //0 for Myroom, 1 for Attic, 2 for else
                if (camera == 0) Camera.main.transform.position = new Vector3(0, -5.0119f, -10);
                else if (camera == 1) Camera.main.transform.position = new Vector3(0, 5.0119f, -10);
                else Camera.main.transform.position = new Vector3(0, 0, -10);
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