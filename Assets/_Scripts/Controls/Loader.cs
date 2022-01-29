using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{

    private class LoadingMonoBehaviour : MonoBehaviour { }

    public enum SceneName { LoadingScene, MainMenuScene, EndlessFight };

    private static Action onLoaderCallback;
    private static AsyncOperation asyncOperation;

    public static void Load(SceneName scene)
    {
        onLoaderCallback = () =>
        {
            //SceneManager.LoadSceneAsync(scene.ToString());
            GameObject loadingGameObject = new GameObject("Loading Game Object");
            loadingGameObject.AddComponent<LoadingMonoBehaviour>().StartCoroutine(LoadSceneAsync(scene));
        };


        SceneManager.LoadScene(SceneName.LoadingScene.ToString());
    }

    private static IEnumerator LoadSceneAsync(SceneName scene)
    {
        yield return null;
        asyncOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public static float GetLoadingProgress()
    {
        if (asyncOperation != null)
            return asyncOperation.progress;
        else return 1f;
    }

    public static void LoaderCallback()
    {
        //triggered after first Update
        if (onLoaderCallback != null)
        {
            onLoaderCallback();
            onLoaderCallback = null;
        }
    }
}
