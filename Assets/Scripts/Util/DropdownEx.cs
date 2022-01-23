using UnityEngine;
using UnityEngine.UI;

public class DropdownEx : Dropdown
{
    [SerializeField]
    private MsgLabel[] _labels;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < options.Count; ++i)
        {
            options[i].text = MsgAccessor.GetMessage(_labels[i]);
        }
        captionText.text = options.Count > 0 ? options[0].text : string.Empty;
    }
}
