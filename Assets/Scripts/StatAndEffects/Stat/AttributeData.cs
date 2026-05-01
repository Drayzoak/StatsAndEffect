using System;
using Common;
using StatAndEffects.Modifiers;
using Unity.Properties;
using UnityEngine;
namespace StatAndEffects.Stat
{
    [Serializable]
    public class AttributeData
    {
        public StatDefinition statDefinition;
        public StatModifierType statModifierType;
        [SerializeField,DontCreateProperty]
        private string formula;

        [SerializeField]
        private bool IsDirty;
        [CreateProperty]
        public string Formula
        {
            get { return formula; }
            set
            {
                formula = value; 
                this.IsDirty = true;
            }
        }
        
        private Func<float, float> func;
        public Func<float, float> Calculate
        {
            get
            {
                if (this.func == null || this.IsDirty)
                {
                    this.func = FormulaBuilder.Build(this.formula);
                    this.IsDirty = false;
                }
                return this.func;
            }
        }

    }
}