using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextMeshProUGUIEx : TextMeshProUGUI
{
    [SerializeField, SearchableEnum]
    private MsgLabel _label;

    public MsgLabel Label
    {
        get => _label;
        set
        {
            _label = value;
            text = MsgAccessor.GetMessage(value);
        }
    }

    public void SetText(MsgLabel label, List<int> param)
    {
        _label = label;
        text = MsgAccessor.GetMessage(label, param);
    }
    
    public void SetText(MsgLabel label, params object[] param)
    {
        _label = label;
        text = MsgAccessor.GetMessage(label, param);
    }

    protected override void Awake()
    {
        base.Awake();
        text = MsgAccessor.GetMessage(_label);
    }
}