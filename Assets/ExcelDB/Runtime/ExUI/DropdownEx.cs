using UnityEngine;

namespace TMPro
{
    public class DropdownEx : TMP_Dropdown
    {
        [SerializeField, SearchableEnum]
        private MsgLabel[] _labels;

        public MsgLabel[] Labels => _labels;

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
}