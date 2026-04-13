using UnityEditor;
using UnityEngine;
using SAPUnityEditorTools.Attributes;

namespace SAPUnityEditorTools.Editor
{
    [CustomPropertyDrawer(typeof(HorizontalLineAttribute))]
    public class HorizontalLineDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            position.height = 1;
            position.y += 5;
            EditorGUI.DrawRect(position, Color.white);
        }

        public override float GetHeight()
        {
            return base.GetHeight();
        }
    }
}