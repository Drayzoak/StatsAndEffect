using System;
using Common;
using StatAndEffects.Modifiers;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
namespace StatAndEffects.Editor
{
    public class StatModifierElement : VisualElement
    {
        private FloatField _floatField;
        private EnumField _enumField;
        
        public Action<StatModifier, StatModifierType> OnTypeChanged;
        public Action OnValueChanged;
        
        public StatModifierElement()
        {
            this._floatField = new FloatField()
            {
                style = { width = new StyleLength(new Length(50, LengthUnit.Percent)) }
            };
            this._enumField = new EnumField()
            {
                style = { width = new StyleLength(new Length(50, LengthUnit.Percent)) }
            };
            this._enumField.Init(StatModifierType.Flat);
            this.Add(this._floatField);
            this.Add(this._enumField);
            this.style.flexDirection = FlexDirection.Row;
        }

        public void SetStatModifier(StatModifier statModifier)
        {
            if (statModifier == null) return;
            dataSource = statModifier;
            
            this._floatField.SetValueWithoutNotify(statModifier.Value);
            this._enumField.SetValueWithoutNotify(statModifier.Type);
            
            DataBindingHelper.BindToSource(this._floatField, new PropertyPath(nameof(StatModifier.Value)));
            this._floatField.RegisterValueChangedCallback(evt => this.OnValueChanged?.Invoke());
            
            this._enumField.RegisterValueChangedCallback(this.OnModifierTypeChanged);
            
        }
        
        private void OnModifierTypeChanged(ChangeEvent<Enum> evt)
        {
            if (evt.previousValue == evt.newValue) return;
            this.OnTypeChanged?.Invoke((StatModifier)dataSource, (StatModifierType)evt.newValue);
        }

        public void UnBind()
        {
            dataSource = null;
            this._enumField.UnregisterValueChangedCallback(this.OnModifierTypeChanged);
            this._floatField.ClearBindings();
        }
    }
}