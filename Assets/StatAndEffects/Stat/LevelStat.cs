using System;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Serialization;

namespace StatAndEffects.Stat
{
    [Serializable]
    public class LevelStat : AbstractStat
    {
        [SerializeField, DontCreateProperty] private int m_Level = 1;
        [SerializeField, DontCreateProperty] private int m_MaxLevel = 1;
        [SerializeField, DontCreateProperty] private float m_CurrentXp = 0f;

        [SerializeField] private float m_BaseXp = 100f;
        [SerializeField] private float _growthFactor = 1.2f;

        private LayeredModifierCollection m_XpModifiersCollection;
        public LayeredModifierCollection XPModifiersCollection => m_XpModifiersCollection;

        #region Properties

        [CreateProperty]
        public int Level
        {
            get => m_Level;
            set
            {
                if (value == m_Level) return;
                m_Level = Mathf.Clamp(value, 0, m_MaxLevel);
                NotifyPropertyChanged();
                RecalculateBaseXp();
            }
        }

        [CreateProperty]
        public int MaxLevel
        {
            get => m_MaxLevel;
            set
            {
                if (value == m_MaxLevel) return;
                m_MaxLevel = value;
                NotifyPropertyChanged();
                RecalculateBaseXp();
            }
        }

        [CreateProperty]
        public float CurrentXp
        {
            get => m_CurrentXp;
            set
            {
                if (Mathf.Approximately(value, m_CurrentXp)) return;

                m_CurrentXp = Mathf.Clamp(value, 0f, XpRequiredToLevelUp);
                NotifyPropertyChanged();
            }
        }

        [CreateProperty]
        public float XpRequiredToLevelUp => Value;

        [CreateProperty]
        public float BaseXp
        {
            get => m_BaseXp;
            set
            {
                if (Mathf.Approximately(value, this.m_BaseXp)) return;
                m_BaseXp = Mathf.Clamp(value, 0f, XpRequiredToLevelUp);
                NotifyPropertyChanged();
                this.RecalculateBaseXp();
            }
        }

        [CreateProperty]
        public float GrowthFactor
        {
            get => this._growthFactor;
            set
            {
                if (Mathf.Approximately(value, this._growthFactor)) return;
                this._growthFactor = value > 0 ? value : 1f;
                NotifyPropertyChanged();
                this.RecalculateBaseXp();
            }
        }
        #endregion

        #region Constructors

        
        public LevelStat() : base()
        {
            this.m_XpModifiersCollection = new LayeredModifierCollection();
        }

        
        public LevelStat(int level, int maxLevel,float baseXp, float growthFactor,
            string statDefinition = "NN",
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY,
            int modsMaxCapacity = DEFAULT_LIST_CAPACITY,
            LayerCreationContext layerCreationContext = null,
            LayerCreationContext XpLayerCreationContext = null)
            : base(CalculateBaseXp(level, baseXp, growthFactor),statDefinition, digitAccuracy, modsMaxCapacity, layerCreationContext)
        {
            
            if (XpLayerCreationContext == null)
                this.m_XpModifiersCollection = new LayeredModifierCollection();
            else
                this.m_XpModifiersCollection = new LayeredModifierCollection(XpLayerCreationContext);
            
        
            m_Level = level;
            m_MaxLevel = maxLevel;
            m_BaseXp = baseXp;
            this._growthFactor = growthFactor;
            m_CurrentXp = 0f;

            BaseValue = CalculateBaseXp(level, baseXp, growthFactor);
            
        }

        #endregion


        #region Core Logic

        public void AddXp(float amount)
        {
            if (amount <= 0) return;

            m_CurrentXp += amount;

            while (m_CurrentXp >= XpRequiredToLevelUp)
            {
                m_CurrentXp -= XpRequiredToLevelUp;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            m_Level++;

            BaseValue = CalculateBaseXp(m_Level, m_BaseXp, this._growthFactor);
            IsDirty = true;

            OnLevelUp?.Invoke(m_Level);
        }

        private void RecalculateBaseXp()
        {
            BaseValue = CalculateBaseXp(m_Level, m_BaseXp, this._growthFactor);
        }

        private static float CalculateBaseXp(int level, float baseXp, float growth)
        {
            return baseXp * Mathf.Pow(growth, level - 1);
        }

        #endregion

        #region Events

        public event Action<int> OnLevelUp;

        #endregion
    }
}