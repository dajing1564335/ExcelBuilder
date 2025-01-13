using System;
using UnityEngine;

public enum Scene
{
}

public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    private AsyncOperation _asyncOperation;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneAsync(Scene scene)
    {
        switch (scene)
        {
            default:
                LoadSceneAsync(scene, null, null);
                break;
        }
    }

    private void LoadSceneAsync(Scene scene, Action afterFadeIn, Func<bool> canFadeOut)
    {
        _asyncOperation = null;
        FadeManager.Instance.StartFade(
        () =>
        {
            afterFadeIn?.Invoke();
            //...
        },
        () =>
        {
            if (_asyncOperation != null) return _asyncOperation.isDone;
            if (canFadeOut?.Invoke() ?? true)
            {
                _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync((int)scene);
            }
            return false;
        }).Forget();
    }
}
