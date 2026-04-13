using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Common
{
    public abstract class ManagedReferenceDrawer : PropertyDrawer
    {
        static Dictionary<Type, Dictionary<string, Type>> cache = new();

        protected virtual Type GetBaseType(SerializedProperty property)
        {
            // Extract base type from field
            var fieldType = fieldInfo.FieldType;

            // Handle List<T> or arrays
            if (fieldType.IsArray)
                return fieldType.GetElementType();

            if (fieldType.IsGenericType)
                return fieldType.GetGenericArguments()[0];

            return fieldType;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var baseType = this.GetBaseType(property);

            if (!cache.TryGetValue(baseType, out var typeMap))
            {
                typeMap = this.BuildTypeMap(baseType);
                cache[baseType] = typeMap;
            }

            var typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);
            
            var typeName = property.managedReferenceFullTypename;
            var displayName = this.GetShortTypeName(typeName);

            // 🔽 Dropdown
            if (EditorGUI.DropdownButton(typeRect, new GUIContent(displayName ?? $"Select {baseType.Name}"), FocusType.Keyboard))
            {
                var menu = new GenericMenu();

                if (typeMap.Count == 0)
                {
                    menu.AddDisabledItem(new GUIContent($"No {baseType.Name} found"));
                }
                else
                {
                    foreach (var kvp in typeMap)
                    {
                        var name = kvp.Key;
                        var type = kvp.Value;

                        menu.AddItem(new GUIContent(name), type.FullName == typeName, () =>
                        {
                            property.managedReferenceValue = Activator.CreateInstance(type);
                            property.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }

                menu.ShowAsContext();
            }

            // 🔥 Draw WITHOUT foldout
            if (property.managedReferenceValue != null)
            {
                this.DrawChildren(contentRect, property);
            }

            EditorGUI.EndProperty();
        }

        void DrawChildren(Rect rect, SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            float y = rect.y;

            iterator.NextVisible(true);

            while (!SerializedProperty.EqualContents(iterator, end))
            {
                float h = EditorGUI.GetPropertyHeight(iterator, true);

                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, h), iterator, true);

                y += h + EditorGUIUtility.standardVerticalSpacing;

                if (!iterator.NextVisible(false))
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.managedReferenceValue != null)
            {
                var iterator = property.Copy();
                var end = iterator.GetEndProperty();

                iterator.NextVisible(true);

                while (!SerializedProperty.EqualContents(iterator, end))
                {
                    height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;

                    if (!iterator.NextVisible(false))
                        break;
                }
            }

            return height;
        }

        Dictionary<string, Type> BuildTypeMap(Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm =>
                {
                    try { return asm.GetTypes(); }
                    catch { return Type.EmptyTypes; }
                })
                .Where(t => !t.IsAbstract && baseType.IsAssignableFrom(t))
                .ToDictionary(t => ObjectNames.NicifyVariableName(t.Name), t => t);
        }

        string GetShortTypeName(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName)) return null;

            var parts = fullTypeName.Split(' ');
            return parts.Length > 1 ? parts[1].Split('.').Last() : fullTypeName;
        }
    }
}
#endif