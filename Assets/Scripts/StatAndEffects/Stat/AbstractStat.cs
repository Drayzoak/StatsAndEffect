using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace StatAndEffects.Stat
{
    [Serializable]
    public abstract partial class AbstractStat 
        : IDataSourceViewHashProvider, INotifyBindablePropertyChanged
    {
    #region CONFIG
        protected const int DEFAULT_LIST_CAPACITY = 16;
        protected const int DEFAULT_DIGIT_ACCURACY = 2;
        private const int MAXIMUM_ROUND_DIGITS = 6;
    #endregion
        
    #region Fields
        [SerializeField] private int m_DigitAccuracy = DEFAULT_DIGIT_ACCURACY;
        [SerializeField] private int m_MaxModCapacity = DEFAULT_LIST_CAPACITY;

        [SerializeField] protected StatDefinition m_StatDefinition;

        [SerializeField] protected float m_BaseValue;
        [SerializeField] protected float m_FinalValue;
        [SerializeField] protected float m_ModifiedValue;

        [SerializeField] private bool m_IsDirty = true;

        [SerializeReference] 
        private LayeredModifierCollection _modifiersCollection;
    #endregion
    
    #region Event
        public event Action BaseValueChanged;
        public event Action ModifiedValueChanged;
    #endregion
        
    #region Properties
        [CreateProperty]
        public int DigitAccuracy => m_DigitAccuracy;

        [CreateProperty]
        public int MaxModCapacity => m_MaxModCapacity;

        public bool IsDirty
        {
            get => m_IsDirty;
            set
            {
                if (m_IsDirty == value) return;

                m_IsDirty = value;

                if (m_IsDirty)
                {
                    Calculate();
                    NotifyPropertyChanged();
                }
            }
        }
        public LayeredModifierCollection ModifiersCollection => _modifiersCollection;

        [CreateProperty]
        public StatDefinition StatDefinition
        {
            get => m_StatDefinition;
            set
            {
                if (Equals(m_StatDefinition, value)) return;

                m_StatDefinition = value;
                MarkDirty();
                NotifyPropertyChanged();
            }
        }

        [CreateProperty]
        public float BaseValue
        {
            get => m_BaseValue;
            set
            {
                if (Mathf.Approximately(m_BaseValue, value)) return;

                m_BaseValue = Round(value);
                MarkDirty();
                
                NotifyPropertyChanged();
                BaseValueChanged?.Invoke();
            }
        }

        /// <summary>
        /// Final evaluated value (Base + Modifiers)
        /// </summary>
        [CreateProperty]
        public float Value
        {
            get
            {
                EnsureCalculated();
                return m_FinalValue;
            }
        }

        /// <summary>
        /// Difference from base value
        /// </summary>
        [CreateProperty]
        public float ModifiedValue
        {
            get
            {
                EnsureCalculated();
                return this.m_ModifiedValue;
            }
        }

    #endregion
        
        protected AbstractStat(
            float baseValue,
            string statDefinition = "NN",
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY,
            int modsMaxCapacity = DEFAULT_LIST_CAPACITY,
            LayerCreationContext layerCreationContext = null)
        {
            m_DigitAccuracy = digitAccuracy;
            m_MaxModCapacity = modsMaxCapacity;

            if (!string.IsNullOrEmpty(statDefinition))
                m_StatDefinition = StatAbilitiesManager.TryToGetValue(statDefinition);

            _modifiersCollection = layerCreationContext != null
                ? new LayeredModifierCollection(layerCreationContext)
                : new LayeredModifierCollection(modsMaxCapacity);
            
            
            BaseValue = baseValue;
        }

        protected AbstractStat()
        {
            _modifiersCollection = new LayeredModifierCollection();
        }

        // ================= CORE LOGIC =================

        protected void EnsureCalculated()
        {
            if (!m_IsDirty) return;

            Calculate();
        }

        private void Calculate()
        {
            int accuracy = Mathf.Clamp(m_DigitAccuracy, 0, MAXIMUM_ROUND_DIGITS);

            float final = _modifiersCollection.Evaluate(m_BaseValue);
            final = (float)Math.Round(final, accuracy);

            m_FinalValue = final;
            this.m_ModifiedValue = final - m_BaseValue;

            m_IsDirty = false;

            NotifyPropertyChanged(nameof(Value));
            NotifyPropertyChanged(nameof(ModifiedValue));
            ModifiedValueChanged?.Invoke();
        }

        public void MarkDirty()
        {
            m_IsDirty = true;
            this.EnsureCalculated();
        }

        private float Round(float value)
        {
            return (float)Math.Round(value, m_DigitAccuracy);
        }

        // ================= UTIL =================

        public bool ContainsModifier(StatModifier mod) =>
            _modifiersCollection.ContainsModifier(mod);

        public List<StatModifier> GetModifiers() =>
            _modifiersCollection.GetAllModifiersCopy();

        public string GetStatName() =>
            m_StatDefinition ? m_StatDefinition : "Stat";

        public static implicit operator float(AbstractStat stat) => stat.Value;

    }
}