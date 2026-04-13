using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using Random = UnityEngine.Random;

namespace SAPUnityEditorTools
{
    [CustomEditor(typeof(SerializableMonoBehaviour), true)]
    public class SerializableDictionaryEditor : UnityEditor.Editor
    {
        readonly List<bool> isShowing = new();
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            int ind = 0;
            var serialized = serializedObject.GetIterator();
            if (serialized.NextVisible(true))
            {
                var boxColor = Color.green;
                do
                {
                    if (serialized.propertyType == SerializedPropertyType.Generic)
                    {

                        //Check if serialized property is of SerializableDictionary type of not. Note: I did not find any other proper way to check what serialized
                        //property type is so I just compare it with string value. I need to find better way to achieve this.
                        System.Type parentType = serialized.serializedObject.targetObject.GetType();
                        System.Reflection.FieldInfo fi = parentType.GetField(serialized.propertyPath);
                        var t = fi.GetValue(serialized.serializedObject.targetObject);
                        if (t is ISerializedDictionary)
                        {
                            if (isShowing.Count <= ind)
                            {
                                isShowing.Add(true);
                            }
                            var keys = serialized.FindPropertyRelative("keys");
                            var values = serialized.FindPropertyRelative("values");
                            EditorGUILayout.BeginHorizontal();

                            SerializableDictionaryProperty p = new SerializableDictionaryProperty("#00FF00");
                            var attribute = PropertyHasAttribute<SerializableDictionaryProperty>(serialized, ref p);
                            if (p == null)
                            {
                                boxColor = Color.green;
                            }                        //Create foldout group
                            isShowing[ind] = EditorGUILayout.Foldout(isShowing[ind], GetNameFormatted(serialized.name), true);
                            GUILayout.Space(5);
                            var style = new GUIContent("+", "Add new item in the dictionary");

                            //If keys or values are not an serializable then we can't show it in inspector, so we will simply show a message to user.
                            if (keys == null || values == null)
                            {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.LabelField("No serialized key or property found. Please make sure any custom class is marked as serialized or not.");
                                continue;
                            }
                            //Get key and values element type.
                            var keyElementType = keys.arrayElementType;
                            var valueElementType = values.arrayElementType;

                            GUILayout.FlexibleSpace();

                            //Show size of the dictionary.
                            EditorGUILayout.LabelField("Size", GUILayout.Height(20), GUILayout.Width(40), GUILayout.MaxWidth(40));
                            //Add int field, so user can directly set number of element in dictionary.
                            var size = EditorGUILayout.IntField(keys.arraySize, GUILayout.Width(40), GUILayout.MaxWidth(40));
                            if (GUILayout.Button(style, GUILayout.Width(20)))
                            {
                                if (keys.arraySize == 0)
                                {
                                    keys.arraySize = 1;
                                }
                                else
                                {
                                    keys.InsertArrayElementAtIndex(keys.arraySize - 1);
                                    var key = keys.GetArrayElementAtIndex(keys.arraySize - 1);
                                    for (int i = 0; i < keys.arraySize - 1; i++)
                                    {
                                        var k = keys.GetArrayElementAtIndex(i);
                                        switch (key.propertyType)
                                        {
                                            case SerializedPropertyType.Integer:
                                                if (k.intValue == key.intValue)
                                                {
                                                    key.intValue = key.intValue + Random.Range(1, 5000);
                                                }
                                                break;
                                            case SerializedPropertyType.Float:
                                                if (k.floatValue == key.floatValue)
                                                {
                                                    key.floatValue = key.floatValue + Random.Range(1, 5000);
                                                }
                                                break;
                                            case SerializedPropertyType.String:
                                                if (k.stringValue.Equals(key.stringValue))
                                                {
                                                    key.stringValue = key.stringValue + Random.Range(1, 5000);
                                                }
                                                break;
                                            default:
                                                EditorGUILayout.LabelField("Unsupported key type or it is not serialized. If it is a custom class, please make it serializable using serializable attribute to show here");
                                                break;
                                        }
                                    }
                                }
                                values.arraySize = keys.arraySize;
                                size = keys.arraySize;
                            }
                            EditorGUILayout.EndHorizontal();
                            keys.arraySize = size;
                            values.arraySize = size;
                            //Here, I have checked if keys already contains the same key or not. because dictonary can't have same key multiple times,
                            //It can break our login at runtime. Better approch is appriciated.
                            for (int i = 0; i < keys.arraySize - 1; i++)
                            {
                                var key = keys.GetArrayElementAtIndex(i);
                                for (int j = 0; j < keys.arraySize; j++)
                                {
                                    if (j == i)
                                        continue;
                                    var k = keys.GetArrayElementAtIndex(j);
                                    switch (key.propertyType)
                                    {
                                        case SerializedPropertyType.Integer:
                                            if (k.intValue == key.intValue)
                                            {
                                                key.intValue = key.intValue + Random.Range(1, 5000);
                                            }
                                            break;
                                        case SerializedPropertyType.Float:
                                            if (k.floatValue == key.floatValue)
                                            {
                                                key.floatValue = key.floatValue + Random.Range(1, 5000);
                                            }
                                            break;
                                        case SerializedPropertyType.String:
                                            if (k.stringValue.Equals(key.stringValue))
                                            {
                                                key.stringValue = key.stringValue + Random.Range(1, 5000);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            EditorGUILayout.Space(5);
                            //Show dictionary items if foldout is expanded.
                            if (isShowing[ind])
                            {
                                //EditorGUI.indentLevel++;
                                values.arraySize = keys.arraySize;
                                if (keys.arraySize > 0)
                                {
                                    for (int i = 0; i < keys.arraySize; i++)
                                    {
                                        var rect = GUILayoutUtility.GetLastRect();
                                        rect.y += 5;
                                        rect.x -= 2;
                                        rect.width = EditorGUIUtility.currentViewWidth - 30;
                                        rect.height = 1;
                                        EditorGUI.DrawRect(rect, boxColor);
                                        GUILayout.Space(5);
                                        //var rect = new Rect(0, GUILayoutUtility.GetLastRect().y,0, 20);
                                        EditorGUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth - 50));
                                        var key = keys.GetArrayElementAtIndex(i);
                                        var value = values.GetArrayElementAtIndex(i);
                                        var width = ((EditorGUIUtility.currentViewWidth - 40) / 2f);
                                        //rect.width = width * 2f;
                                        //EditorGUI.DrawRect(rect,Color.cyan);
                                        //Check key property type and show Editor field for it. If it differs from any of these types then we will try
                                        //to show it as property field.
                                        switch (key.propertyType)
                                        {
                                            case SerializedPropertyType.Integer:
                                                EditorGUILayout.LabelField("Key", GUILayout.Height(20), GUILayout.Width(50));
                                                key.intValue = EditorGUILayout.IntField(key.intValue, GUILayout.Height(20), GUILayout.MaxWidth(width - 50));
                                                break;
                                            case SerializedPropertyType.Float:
                                                EditorGUILayout.LabelField("Key", GUILayout.Height(20), GUILayout.Width(50));
                                                key.floatValue = EditorGUILayout.FloatField(key.floatValue, GUILayout.Height(20), GUILayout.MaxWidth(width - 50));
                                                break;
                                            case SerializedPropertyType.String:
                                                EditorGUILayout.LabelField("Key", GUILayout.Height(20), GUILayout.Width(50));
                                                key.stringValue = EditorGUILayout.TextField(key.stringValue, GUILayout.Height(20), GUILayout.MaxWidth(width - 50));
                                                break;
                                            default:
                                                EditorGUILayout.LabelField("Key", GUILayout.Height(20), GUILayout.Width(50));
                                                EditorGUILayout.PropertyField(key, GUILayout.MaxWidth(width * 2 - 50));
                                                break;
                                        }
                                        bool isItemDeleted = false;
                                        if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.MaxWidth(20), GUILayout.Height(20)))
                                        {
                                            keys.DeleteArrayElementAtIndex(i);
                                            values.DeleteArrayElementAtIndex(i);
                                            isItemDeleted = true;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        if (isItemDeleted)
                                            continue;
                                        EditorGUILayout.BeginHorizontal(GUILayout.Width(EditorGUIUtility.currentViewWidth - 20));
                                        switch (value.propertyType)
                                        {
                                            case SerializedPropertyType.Integer:
                                                EditorGUILayout.LabelField("Value", GUILayout.Height(20), GUILayout.Width(50));
                                                value.intValue = EditorGUILayout.IntField(value.intValue, GUILayout.Height(20), GUILayout.MaxWidth(width - 50));
                                                break;
                                            case SerializedPropertyType.Float:
                                                EditorGUILayout.LabelField("Value", GUILayout.Height(20), GUILayout.Width(50));
                                                value.floatValue = EditorGUILayout.FloatField(value.floatValue, GUILayout.Height(20), GUILayout.MaxWidth(width - 50));
                                                break;
                                            case SerializedPropertyType.String:
                                                EditorGUILayout.LabelField("Value", GUILayout.Height(20), GUILayout.Width(50));
                                                value.stringValue = EditorGUILayout.TextField(value.stringValue, GUILayout.Height(20), GUILayout.MaxWidth(width - 50));
                                                break;
                                            default:
                                                EditorGUILayout.LabelField("Value", GUILayout.Height(20), GUILayout.Width(50));
                                                EditorGUILayout.PropertyField(value, GUILayout.MaxWidth(width * 2 - 50));
                                                break;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        //Create border line
                                        var rect1 = GUILayoutUtility.GetLastRect();
                                        rect1.x -= 2;
                                        rect1.width = 1;
                                        rect1.height += rect1.y - rect.y + 6;
                                        rect1.y = rect.y;
                                        EditorGUI.DrawRect(rect1, boxColor);
                                        rect1.x = EditorGUIUtility.currentViewWidth - 15;
                                        EditorGUI.DrawRect(rect1, boxColor);
                                        GUILayout.Space(10);
                                        rect = GUILayoutUtility.GetLastRect();
                                        rect.x -= 2;
                                        rect.y += 5;
                                        rect.width = EditorGUIUtility.currentViewWidth - 30;
                                        rect.height = 1;
                                        EditorGUI.DrawRect(rect, boxColor);
                                        //End border line
                                    }
                                }
                                //EditorGUI.indentLevel--;
                            }
                            ind++;
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(serialized, true);
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(serialized, true);
                    }
                }
                while (serialized.NextVisible(false));
            }
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// Function to make variable name formatted. There might be an easy approch to achieve this. You may tweak this to make it more performant.
        /// </summary>
        /// <param name="n">Name of the variable</param>
        /// <returns>Return name with formatting for ex. n = myVar return value will be My Var</returns>
        public string GetNameFormatted(string n)
        {
            n = n.Replace(n[0], n[0].ToString().ToUpper()[0]);
            var s = n;
            int spaceAdded = 0;
            for (int i = 1; i < n.Length; i++)
            {
                if (char.IsUpper(n[i]))
                {
                    s = s.Insert(i + spaceAdded, " ");
                    spaceAdded++;
                }
            }
            return s;
        }

        private bool PropertyHasAttribute<T>(SerializedProperty property, ref T val) where T : PropertyAttribute
        {
            System.Reflection.FieldInfo field = GetFieldFromProperty(property);
            if (field != null)
            {
                val = (T)field.GetCustomAttribute(typeof(T), true);
                return true;
            }
            else
                return false;
        }

        private System.Reflection.FieldInfo GetFieldFromProperty(SerializedProperty property)
        {
            string[] path = property.propertyPath.Split('.');
            System.Type type = serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo field = null;

            foreach (string fieldName in path)
            {
                field = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                type = field.FieldType;
            }

            return field;
        }
    }
}
