
using UnityEditor.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SAPUnityEditorTools
{
#if UNITY_EDITOR
    [CustomEditor(typeof(TooltipButton))]
    public class TooltipButtonEditor : ButtonEditor
    {

    }
#endif
}