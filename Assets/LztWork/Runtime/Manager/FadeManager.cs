using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : SingletonMonoBehaviour<FadeManager>
{
    private const float FADE_TIME = 0.15f;

    private Image _fade;
    private readonly util.ColorValueAnim _anim = new();

    protected override void Awake()
    {
        base.Awake();
        _fade = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    public async UniTaskVoid StartFade(Action afterFadeIn, Func<bool> canFadeOut)
    {
        gameObject.SetActive(true);
        _anim.StartAnim(Color.clear, Color.black, FADE_TIME);
        await UniTask.WaitWhile(_anim.InAnimFunc);
        afterFadeIn?.Invoke();
        if (canFadeOut != null)
        {
            await UniTask.WaitUntil(canFadeOut);
        }
        _anim.StartAnim(Color.black, Color.clear, FADE_TIME);
        await UniTask.WaitWhile(_anim.InAnimFunc);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_anim.InAnim)
        {
            _fade.color = _anim.GetValue();
        }
    }
}
