using System;
using System.Collections.Generic;
using Common.Extensions;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Serialization;

namespace StatAndEffects.Stat
{
    
    [Serializable]
    public abstract partial class AbstractStat 
    {
    #region Statcache
        
        [SerializeField]
        private int m_DigitAccuracy;
        [CreateProperty]
        public int DigitAccuracy
        {
            get => m_DigitAccuracy;
        }
        
        [SerializeField]
        private int m_MaxModCapacity;
        [CreateProperty]
        public int MaxModCapacity
        {
            get => m_MaxModCapacity;
        }
        
        [HideInInspector][SerializeField]
        private bool m_IsDirty;
        public bool IsDirty
        {
            get => m_IsDirty;
            set
            {
                m_IsDirty = value;
                if (m_IsDirty)
                {
                    this.NotifyPropertyChanged();
                    OnModifiersChanged();
                }
            }
        }

        [SerializeField]
        private LayeredModifierCollection _modifiersCollection;
        public LayeredModifierCollection ModifiersCollection  => _modifiersCollection;

    #endregion 
        
    #region Events
        public event Action ModifiedValueChanged;
        protected virtual void OnModifiedValueChanged() => ModifiedValueChanged?.Invoke();
        private void OnModifiersChanged() => ModifiedValueChanged?.Invoke();
    #endregion
        
    #region StatData
        [Header("StatValues")]
        [SerializeField,DontCreateProperty]
        protected StatDefinition m_StatDefinition;
        
        [SerializeField, DontCreateProperty]
        protected float m_Value;
        [SerializeField, DontCreateProperty]
        protected float m_BaseValue;
        [SerializeField, DontCreateProperty]
        protected float m_ModifiedValue;
        [SerializeField, DontCreateProperty]
        protected float m_EffectValue;

        [CreateProperty]
        public StatDefinition StatDefinition
        {
            get => this.m_StatDefinition; 
            set
            {
                if (Equals(this.m_StatDefinition, value)) return;
                this.m_StatDefinition = value;
                this.NotifyPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float Value{
            get
            {
                if (IsDirty) CalculateModifiedValue(m_DigitAccuracy);
                
                return m_Value;
            }
            private set
            {
                if (Mathf.Approximately(this.m_Value, value)) return;
                m_Value = value;
                this.NotifyPropertyChanged();
            }
        }
        
        [CreateProperty]
        public float BaseValue
        {
            get => m_BaseValue;
            set
            {
                if(Mathf.Approximately(m_BaseValue,value)) return ;
                
                m_BaseValue = value;
                
                this.NotifyPropertyChanged();
                IsDirty = true;
            }
        }
        
        
        [CreateProperty]
        public float ModifiedValue
        {
            get
            {
                if (IsDirty) CalculateModifiedValue(m_DigitAccuracy);
                return m_ModifiedValue;
            }
            private set
            {
                if (Mathf.Approximately(m_ModifiedValue, value)) return;
                
                m_ModifiedValue = value;
                this.NotifyPropertyChanged();
                OnModifiedValueChanged();
            }
        }
        
        
        
    #endregion

    #region Constant
        protected const int DEFAULT_LIST_CAPACITY = 16;
        protected const int DEFAULT_DIGIT_ACCURACY = 2;
        private const int MAXIMUM_ROUND_DIGITS = 6;
    #endregion

    #region operator
        public static implicit operator float(AbstractStat stat)
        {
            return stat.Value;
        }
    #endregion
        protected AbstractStat(
            float baseValue,
            string statDefinition = "NN",
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY,
            int modsMaxCapacity = DEFAULT_LIST_CAPACITY,
            LayerCreationContext layerCreationContext = null
        )
        {
            m_DigitAccuracy = digitAccuracy;
            m_MaxModCapacity = modsMaxCapacity;
            _layerCreationContext = layerCreationContext;

            if (!string.IsNullOrEmpty(statDefinition))
            {
                m_StatDefinition = StatAbilitiesManager.TryToGetValue(statDefinition);
            }
            InitializeLayerCollection();
            BaseValue = baseValue;

        }

        protected AbstractStat()
        {
            m_DigitAccuracy = DEFAULT_DIGIT_ACCURACY;
            m_MaxModCapacity = DEFAULT_LIST_CAPACITY;
            this.InitializeLayerCollection();
        }
        
        private void InitializeLayerCollection()
        {
            Debug.Log($"Initializing layer collection {this.IsValidLayerContext()}");
            if (IsValidLayerContext())
            {
                _modifiersCollection = new LayeredModifierCollection(_layerCreationContext);
            }
            else
            {
                _modifiersCollection = new LayeredModifierCollection(m_MaxModCapacity);
            }
        }

        private float CalculateModifiedValue(int digitAccuracy)
        {
            digitAccuracy = Mathf.Clamp(digitAccuracy, 0, MAXIMUM_ROUND_DIGITS);
            
            float finalValue =  this._modifiersCollection.Evaluate(BaseValue);
            finalValue = (float)Math.Round(finalValue, digitAccuracy);

            Value = finalValue;
            ModifiedValue = finalValue - BaseValue ;

            IsDirty = false;

            return ModifiedValue;
        }

        public bool ContainsModifier(StatModifier statModifier) =>
            this._modifiersCollection.ContainsModifier(statModifier);
        
        public bool ContainsModifier(StatModifier statModifier , StatLayer statLayer) =>
            this._modifiersCollection.ContainsModifier(statModifier, statLayer);

        public List<StatModifier> GetModifiers() => 
            this._modifiersCollection.GetAllModifiersCopy();
        
        public List<StatModifier> GetModifiersOf(StatModifierType statModifierType) => 
            this._modifiersCollection.GetModifiersCopy(statModifierType);

        public List<StatModifier> GetModifiersOf(StatLayer statLayer) => 
            this._modifiersCollection.GetModifiersCopy(statLayer);
        
        public List<StatModifier> GetModifierOf(StatLayer statLayer,StatModifierType statModifierType) =>
            this._modifiersCollection.GetModifiersCopy(statLayer, statModifierType);
        
        public string GetStatName() =>
            this.StatDefinition ? this.StatDefinition : "Stat";
        
    }
}