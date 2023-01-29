using UnityEngine;

public class RuntimeInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        var loadManager = Object.Instantiate(Resources.Load("Util/LoadManager"));
        Object.DontDestroyOnLoad(loadManager);
    }
}
