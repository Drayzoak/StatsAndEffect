
using System;
using Common.Extensions;
using StatAndEffects.Editor.Stat;
using UnityEngine.UIElements;
using StatAndEffects.Stat;
using UnityEditor;

namespace StatAndEffects.Editor
{
    [CustomPropertyDrawer(typeof(AbstractStat), true)]
    public class AbstractStatDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            
            if (property.managedReferenceValue == null)
            {
                Type fieldType = null;
                
                if (!string.IsNullOrEmpty(property.managedReferenceFullTypename))
                {
                    fieldType = Type.GetType(property.managedReferenceFullTypename);
                }
                
                if (fieldType == null)
                {
                    fieldType = property.GetManagedReferenceFieldType();
                }
                
                if (fieldType == null || fieldType.IsAbstract || !typeof(AbstractStat).IsAssignableFrom(fieldType))
                {
                    fieldType = typeof(PrimaryStat); 
                }

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

            if (!isListStat)
                statElement.Name = property.name;
            statElement.SetTypeChangeEnabled(isListStat);

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