using System;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using UnityEngine;
using Unity.Properties;

namespace StatAndEffects.Stat
{
    [Serializable]
    public class LevelStat : AbstractStat
    {
        [SerializeField, DontCreateProperty] private int m_Level = 1;
        [SerializeField, DontCreateProperty] private float m_CurrentXp = 0f;

        [SerializeField] private float m_BaseXp = 100f;
        [SerializeField] private float m_Growth = 1.2f;

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
                m_Level = value;
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

        #endregion

        #region Constructors

        
        public LevelStat() : base()
        {
        }

        
        public LevelStat(int level, float baseXp, float growth,
            string statDefinition = "NN",
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY,
            int modsMaxCapacity = DEFAULT_LIST_CAPACITY,
            LayerCreationContext layerCreationContext = null)
            : base(CalculateBaseXp(level, baseXp, growth),statDefinition, digitAccuracy, modsMaxCapacity, layerCreationContext)
        {
            Initialize(level, baseXp, growth);
        }

        #endregion

        #region Initialization

        public void Initialize(int level, float baseXp, float growth)
        {
            m_Level = level;
            m_BaseXp = baseXp;
            m_Growth = growth;
            m_CurrentXp = 0f;

            BaseValue = CalculateBaseXp(level, baseXp, growth);
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

            BaseValue = CalculateBaseXp(m_Level, m_BaseXp, m_Growth);
            IsDirty = true;

            OnLevelUp?.Invoke(m_Level);
        }

        private void RecalculateBaseXp()
        {
            BaseValue = CalculateBaseXp(m_Level, m_BaseXp, m_Growth);
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