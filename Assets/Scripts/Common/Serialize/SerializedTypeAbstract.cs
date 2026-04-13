using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Common.Serialize
{
    [Serializable]
    public class SerializedType<T> : ISerializationCallbackReceiver where T : class
    {
        [SerializeField] private string assemblyQualifiedName = string.Empty;

        public Type Type { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.assemblyQualifiedName = this.Type?.AssemblyQualifiedName ?? this.assemblyQualifiedName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(this.assemblyQualifiedName))
                return;

            this.Type = Type.GetType(this.assemblyQualifiedName);
        }

        public static implicit operator Type(SerializedType<T> sType) => sType.Type;

        public static implicit operator SerializedType<T>(Type type)
        {
            if (type != null && !typeof(T).IsAssignableFrom(type))
                throw new Exception($"Type {type} is not assignable to {typeof(T)}");

            return new SerializedType<T> { Type = type };
        }
    }
    
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializedType<>), true)]
    public class SerializedTypeDrawer : PropertyDrawer
    {
        private string[] _typeNames;
        private string[] _typeFullNames;

        private void Initialize(Type baseType)
        {
            if (_typeFullNames != null) return;

            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    !t.IsGenericType &&
                    baseType.IsAssignableFrom(t))
                .ToArray();

            _typeNames = types.Select(t => t.Name).ToArray();
            _typeFullNames = types.Select(t => t.AssemblyQualifiedName).ToArray();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var baseType = fieldInfo.FieldType.GetGenericArguments()[0];
            Initialize(baseType);

            var prop = property.FindPropertyRelative("assemblyQualifiedName");

            if (string.IsNullOrEmpty(prop.stringValue) && _typeFullNames.Length > 0)
            {
                prop.stringValue = _typeFullNames[0];
                property.serializedObject.ApplyModifiedProperties();
            }

            int index = Array.IndexOf(_typeFullNames, prop.stringValue);
            int newIndex = EditorGUI.Popup(position, label.text, index, _typeNames);

            if (newIndex != index && newIndex >= 0)
            {
                prop.stringValue = _typeFullNames[newIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
    #endif
}