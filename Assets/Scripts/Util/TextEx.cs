using UnityEngine;
using UnityEngine.UI;

public class TextEx : Text
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
        text = MsgAccessor.GetMessage(_label);
    }
}
