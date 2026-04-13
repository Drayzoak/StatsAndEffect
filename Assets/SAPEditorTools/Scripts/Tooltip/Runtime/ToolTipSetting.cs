using TMPro;
using UnityEngine;
namespace SAPUnityEditorTools
{
    [CreateAssetMenu(fileName = "TooltipSettings", menuName = "Tool tip/Tool tip settings")]
    public class ToolTipSetting : ScriptableObject
    {
        public TMP_FontAsset tmp_font_asset;
        public int fontSize = 20;
        public FontStyles fontStyles;
        public TextAlignmentOptions alignment;
    }
}