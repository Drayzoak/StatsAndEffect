
using System;
using Common.Extensions;
using StatAndEffects.Editor.Stat;
using UnityEngine.UIElements;
using StatAndEffects.Stat;
using UnityEditor;

namespace StatAndEffects.Editor
{
    [CustomPropertyDrawer(typeof(AbstractStat))]
    public class AbstractStatDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                return null;
            }
            
            if (property.managedReferenceValue == null)
            {
                
                // Extract base type from field
                var fieldType = fieldInfo.FieldType;

                // Handle List<T> or arrays
                if (fieldType.IsArray)
                    fieldType = fieldType.GetElementType();

                if (fieldType.IsGenericType)
                    fieldType = fieldType.GetGenericArguments()[0];

                if (fieldType.IsAbstract)
                    fieldType = typeof(PrimaryStat);
                
                var instance = Activator.CreateInstance(fieldType);
                property.managedReferenceValue = instance;
                property.serializedObject.ApplyModifiedProperties();
            }

            AbstractStatElement statElement;
            switch (property.managedReferenceValue.GetType().Name)
            {
                case nameof(VitalStat):
                    statElement = new VitalStatElement();
                    break;
                case nameof(PrimaryStat):
                    statElement = new PrimaryStatElement();
                    break;
                case nameof(LevelStat):
                    statElement = new LevelStatElement();
                    break;
                case nameof(AttributeStat):
                    statElement = new AttributeStatElement();
                    break;
                default:
                    statElement = new PrimaryStatElement();
                    break;
            }

            statElement.dataSource = property.managedReferenceValue;
            statElement.SetSerializedProperty(property);
            statElement.Initialize();
            
            bool isListStat = property.IsInArray();
            int index = property.GetArrayIndex();
            
            
            statElement.changeStatType = ( className) =>
            {
                ReplaceStatInstance(property, className);
            };

            
            statElement.SetStat(
                property.managedReferenceValue.GetType().Name,
                index
            );

            bool allowTypeChange = false;

            if (isListStat)
            {
                var fieldType = fieldInfo.FieldType;                
                
                if (fieldType.IsArray)
                    fieldType = fieldType.GetElementType();

                if (fieldType.IsGenericType)
                    fieldType = fieldType.GetGenericArguments()[0];

                if (fieldType.IsAbstract)
                    allowTypeChange = true;
            }
            else
            {
                statElement.Name = fieldInfo.Name;
            }

            statElement.SetTypeChangeEnabled(allowTypeChange);
            container.Add(statElement);

            return container;
        }

        private void ReplaceStatInstance(SerializedProperty property, string className)
        {
            if (!property.IsInArray())
                return;

            int index = property.GetArrayIndex();

            var parentArray = property.GetParentArray();

            if (parentArray == null || index < 0)
                return;

            AbstractStat currentStat = property.managedReferenceValue as AbstractStat;

            AbstractStat newStat = CreateNewStat(className, currentStat);

            var element = parentArray.GetArrayElementAtIndex(index);

            element.managedReferenceValue = newStat;

            property.serializedObject.ApplyModifiedProperties();
        }

        private AbstractStat CreateNewStat(string className, AbstractStat copyFrom)
        {
            AbstractStat newStat = className switch
            {
                nameof(PrimaryStat) => new PrimaryStat(),
                nameof(VitalStat) => new VitalStat(),
                nameof(LevelStat) => new LevelStat(),
                _ => null
            };

            if (newStat != null && copyFrom != null)
            {
                newStat.BaseValue = copyFrom.BaseValue;
                newStat.StatDefinition = copyFrom.StatDefinition;
            }

            return newStat;
        }
    }
}