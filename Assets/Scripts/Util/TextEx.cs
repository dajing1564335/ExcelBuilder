using UnityEngine;
using UnityEngine.UI;

public class TextEx : Text
{
    [SerializeField, SearchableEnum]
    private MsgLabel _msgId;

    public MsgLabel MsgId
    {
        get => _msgId;
        set
        {
            _msgId = value;
            text = MsgAccessor.GetMessage(value);
        }
    }

    protected override void Start()
    {
        text = MsgAccessor.GetMessage(_msgId);
    }
}
