using System;
using StatAndEffects.Builder;
using Unity.Properties;
using UnityEngine;
namespace StatAndEffects.Stat
{
    [Serializable]
    public class VitalStat : AbstractStat
    {
        [Header("Vital Values")]
        [SerializeField,DontCreateProperty] private float m_MinValue;
        [SerializeField,DontCreateProperty] private float m_CurrentValue;

        #region Properties
        [CreateProperty]
        public float MinValue
        {
            get => this.m_MinValue;
            set
            {
                if (Mathf.Approximately(this.m_MinValue, value)) return;
                this.m_MinValue = value;
                this.ClampCurrent();
                NotifyPropertyChanged();
            }
        }

        [CreateProperty]
        public float CurrentValue
        {
            get => this.m_CurrentValue;
            set
            {
                if (Mathf.Approximately(this.m_CurrentValue, value)) return;
                this.m_CurrentValue = value;
                this.ClampCurrent();
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public VitalStat(
            float baseValue,
            float currentValue,
            float minValue = 0f,
            string statDefinition = "",
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY,
            int modsMaxCapacity = DEFAULT_LIST_CAPACITY,
            LayerCreationContext layerCreationContext = null
        ) : base(baseValue, statDefinition, digitAccuracy, modsMaxCapacity, layerCreationContext)
        {
            this.m_MinValue = minValue;
            this.m_CurrentValue = Mathf.Clamp(currentValue, minValue, baseValue);
        }
        
        public VitalStat() : base()
        {
            
        }
        #endregion

        #region Overrides

        protected override void OnModifiedValueChanged()
        {
            base.OnModifiedValueChanged();
            this.ClampCurrent();
        }

        #endregion

        #region Helpers

        private void ClampCurrent()
        {
            this.m_CurrentValue = Mathf.Clamp(this.m_CurrentValue, this.m_MinValue, Value);
        }

        #endregion

        public void ModifyValue(float value)
        {
            this.CurrentValue += value;
        }
    }

}