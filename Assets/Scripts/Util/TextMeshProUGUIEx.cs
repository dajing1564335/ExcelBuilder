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

    protected override void Start()
    {
        base.Start();
        text = MsgAccessor.GetMessage(_label);
    }
}
