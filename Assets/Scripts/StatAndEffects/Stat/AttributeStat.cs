using System;
using System.Collections.Generic;
using SAPUnityEditorTools;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using Unity.Properties;
using UnityEngine;

namespace StatAndEffects.Stat
{
    [Serializable]
    public class AttributeStat : AbstractStat, ISerializationCallbackReceiver
    {
        
        [SerializeReference,DontCreateProperty]
        private SerializableDictionary<StatDefinition, StatModifier> _attributeModifiers
            = new SerializableDictionary<StatDefinition, StatModifier>();

        public IReadOnlyDictionary<StatDefinition, StatModifier> AttributeModifiers => _attributeModifiers.AsReadOnly();

        
        [CreateProperty]
        public new AttributeStatDefinition StatDefinition
        {
            get => base.StatDefinition as AttributeStatDefinition;
            set
            {
                if (value == null)
                {
                    base.StatDefinition = null;
                    StatLog.LogStatError("StatDefinition cannot be null");
                    return;
                }

                base.StatDefinition = value;
                RebuildModifiers(); 
            }
        }

        public AttributeStat(
            float baseValue,
            string statDefinition = "NN",
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY,
            int modsMaxCapacity = DEFAULT_LIST_CAPACITY,
            LayerCreationContext layerCreationContext = null
        ) : base(baseValue, statDefinition, digitAccuracy, modsMaxCapacity, layerCreationContext)
        {
            Subscribe();
        }

        public AttributeStat() : base()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            BaseValueChanged -= CalculateModifiers;
            BaseValueChanged += CalculateModifiers;
        }

        private void RebuildModifiers()
        {
            if (StatDefinition == null)
                return;

            foreach (var data in StatDefinition.attributesDatas)
            {
                if(data.statDefinition == null)
                    continue;
                if(this._attributeModifiers.ContainsKey(data.statDefinition))
                    continue;
                
                float value = data.Calculate(Value);

                var modifier = new StatModifier(
                    data.statModifierType,
                    value,
                    StatLayer.Attribute
                );
                
                modifier.Register();
                _attributeModifiers[data.statDefinition] = modifier;
            }
        }

        private void CalculateModifiers()
        {
            if (StatDefinition == null || _attributeModifiers.Count == 0)
                return;

            foreach (var data in StatDefinition.attributesDatas)
            {
                if (_attributeModifiers.TryGetValue(data.statDefinition, out var modifier))
                {
                    modifier.Value = data.Calculate(Value);
                }
            }
        }

   
        public void Refresh()
        {
            RebuildModifiers();
            CalculateModifiers();
        }

   
        public void OnBeforeSerialize()
        {
            this._attributeModifiers.OnBeforeSerialize();
        }

        public void OnAfterDeserialize()
        {
            this._attributeModifiers.OnAfterDeserialize();
            
            for (int i = 0; i < this._attributeModifiers.Count; i++)
            {
                StatModifier mod = this._attributeModifiers.GetValueAt(i).Resolve();
                this._attributeModifiers.SetValueAt(i, mod);
            }
            
            Subscribe();

            RebuildModifiers();
            CalculateModifiers();
        }
    }
}