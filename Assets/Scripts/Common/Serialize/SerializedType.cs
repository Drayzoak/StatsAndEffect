using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif
namespace Common.Serialize
{
    
    [Serializable]
    public class SerializedType : ISerializationCallbackReceiver {
        [SerializeField] string assemblyQualifiedName = string.Empty;
        [SerializeField]public string breakpoint = string.Empty;
        public Type Type { get; private set; }
            
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            this.assemblyQualifiedName = this.Type?.AssemblyQualifiedName ?? this.assemblyQualifiedName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (string.IsNullOrEmpty(this.assemblyQualifiedName))
            {
                Debug.LogWarning("Type is null or empty");
                return;
            }
            if (!TryGetType(this.assemblyQualifiedName, out var type)) {
                Debug.LogError($"Type {this.assemblyQualifiedName} not found");
                return;
            }
            this.Type = type;
        }

        static bool TryGetType(string typeString, out Type type) {
            type = Type.GetType(typeString);
            return type != null || !string.IsNullOrEmpty(typeString);
        }
            
        // Implicit conversion from SerializableType to Type
        public static implicit operator Type(SerializedType sType) => sType.Type;

        // Implicit conversion from Type to SerializableType
        public static implicit operator SerializedType(Type type) => new() { Type = type };
    }
    

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializedType))]
    public class SerializableTypeDrawer : PropertyDrawer {
        TypeFilterAttribute typeFilter;
        string[] typeNames, typeFullNames;

        void Initialize() {
            if (this.typeFullNames != null) return;
                
            this.typeFilter = (TypeFilterAttribute) Attribute.GetCustomAttribute(fieldInfo, typeof(TypeFilterAttribute));
                
            var filteredTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => this.typeFilter == null ? DefaultFilter(t) : this.typeFilter.Filter(t))
                .ToArray();
                
            this.typeNames = filteredTypes.Select(t => t.ReflectedType == null ? t.Name : $"t.ReflectedType.Name + t.Name").ToArray();
            this.typeFullNames = filteredTypes.Select(t => t.AssemblyQualifiedName).ToArray();
        }
            
        static bool DefaultFilter(Type type) {
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericType;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            this.Initialize();
            var typeIdProperty = property.FindPropertyRelative("assemblyQualifiedName");

            if (string.IsNullOrEmpty(typeIdProperty.stringValue)) {
                typeIdProperty.stringValue = this.typeFullNames.First();
                property.serializedObject.ApplyModifiedProperties();
            }

            var currentIndex = Array.IndexOf(this.typeFullNames, typeIdProperty.stringValue);
            var selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, this.typeNames);
                
            if (selectedIndex >= 0 && selectedIndex != currentIndex) {
                typeIdProperty.stringValue = this.typeFullNames[selectedIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public class TypeFilterAttribute : PropertyAttribute {
        public Func<Type, bool> Filter { get; }
            
        public TypeFilterAttribute(Type filterType) {
            this.Filter = type => !type.IsAbstract &&
                             !type.IsInterface &&
                             !type.IsGenericType &&
                             type.InheritsOrImplements(filterType);
        }
    }

    public static class TypeExtensions {
        /// <summary>
        /// Checks if a given type inherits or implements a specified base type.
        /// </summary>
        /// <param name="type">The type which needs to be checked.</param>
        /// <param name="baseType">The base type/interface which is expected to be inherited or implemented by the 'type'</param>
        /// <returns>Return true if 'type' inherits or implements 'baseType'. False otherwise</returns>        
        public static bool InheritsOrImplements(this Type type, Type baseType) {
            type = ResolveGenericType(type);
            baseType = ResolveGenericType(baseType);

            while (type != typeof(object)) {
                if (baseType == type || HasAnyInterfaces(type, baseType)) return true;
                    
                type = ResolveGenericType(type.BaseType);
                if (type == null) return false;
            }
                
            return false;
        }
            
        static Type ResolveGenericType(Type type) {
            if (type is not { IsGenericType: true }) return type;

            var genericType = type.GetGenericTypeDefinition();
            return genericType != type ? genericType : type;
        }

        static bool HasAnyInterfaces(Type type, Type interfaceType) {
            return type.GetInterfaces().Any(i => ResolveGenericType(i) == interfaceType);
        }
    }
    #endif
}