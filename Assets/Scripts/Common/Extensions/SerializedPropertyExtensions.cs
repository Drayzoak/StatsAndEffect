#if UNITY_EDITOR

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
namespace Common.Extensions
{
    public static class SerializedPropertyExtensions
    {
        public static bool IsInArray(this SerializedProperty property)
        {
            return property.propertyPath.Contains("Array.data[");
        }

        public static Type GetManagedReferenceFieldType(this SerializedProperty property)
        {
            var target = property.serializedObject.targetObject;
            var targetType = target.GetType();

            var field = targetType.GetField(
                property.propertyPath.Split('.')[0],
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance
            );

            return field?.FieldType;
        }
        
        public static Type GetFieldType(SerializedProperty property)
        {
            var target = property.serializedObject.targetObject;
            var field = target.GetType().GetField(property.name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return field.FieldType;
        }
        public static int GetArrayIndex(this SerializedProperty property)
        {
            var match = Regex.Match(property.propertyPath, @"Array\.data\[(\d+)\]");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        public static SerializedProperty GetParentArray(this SerializedProperty property)
        {
            string path = property.propertyPath;
            int index = path.LastIndexOf(".Array.data[");

            if (index < 0)
                return null;

            string parentPath = path.Substring(0, index);
            return property.serializedObject.FindProperty(parentPath);
        }
    }
}
#endif
