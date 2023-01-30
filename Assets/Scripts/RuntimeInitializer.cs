using UnityEngine;

public class RuntimeInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Util/LoadManager")));
    }
}
