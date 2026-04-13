#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.UIElements;
namespace Common.Helper
{
    public static class PropertySetterHelper
    {
        public static void BindElement<T>(
            INotifyValueChanged<T> field,
            Func<T> getter,
            Action<T> setter,
            SerializedProperty property,
            Action<ChangeEvent<T>> invoke = null)
        {
            // Initialize UI value
            field.SetValueWithoutNotify(getter());

            field.RegisterValueChangedCallback(evt =>
            {
                if (Equals(evt.previousValue, evt.newValue))
                    return;

                var target = property.serializedObject.targetObject;

                // Enable Undo support
                Undo.RecordObject(target, "Inspector Change");

                // Apply setter
                setter(evt.newValue);
                
                // Update serialized object
                var so = property.serializedObject;
                so.Update();
                so.ApplyModifiedProperties();
                invoke?.Invoke(evt);
                // Mark object dirty
                EditorUtility.SetDirty(target);
            });
        }
    }
}
#endif
