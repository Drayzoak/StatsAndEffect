using System;
using StatAndEffects.Stat;
namespace StatAndEffects.Effects
{
    [Serializable]
    public class ApplyVitalStat : Effect
    {
        public StatDefinition statDefinition;
        private VitalStat vital;

        public ApplyVitalStat(VitalStat vital)
        {
            this.vital = vital;
        }
        
        public override void Initialize(IEntityStats stats)
        {
            this.vital  = stats.GetStat<VitalStat>(this.statDefinition);
        }
        
        public override float Process(float value)
        {
            this.vital.ModifyValue(value);
            return value;
        }
    }
    
    [Serializable]
    public class CalculateDamage : Effect
    {
        public StatDefinition attackDefinition;
        public StatDefinition criticalChanceDefinition;
        public StatDefinition criticalDamageDefinition;

        private PrimaryStat attack;
        private PrimaryStat criticalChance;
        private PrimaryStat criticalDamage;

        public CalculateDamage(
            PrimaryStat attack,
            PrimaryStat critChance,
            PrimaryStat critDamage)
        {
            this.attack = attack;
            this.criticalChance = critChance;
            this.criticalDamage = critDamage;
        }

        public override void Initialize(IEntityStats stats)
        {
            this.attack = stats.GetStat<PrimaryStat>(this.attackDefinition);
            this.criticalChance = stats.GetStat<PrimaryStat>(this.criticalChanceDefinition);
            this.criticalDamage = stats.GetStat<PrimaryStat>(this.criticalDamageDefinition);
        }

        public override float Process(float value)
        {
            float result = value * attack.Value;
            
            if (UnityEngine.Random.Range(0f, 100f) <= criticalChance.Value)
            {
                result *= criticalDamage.Value;
            }

            return result;
        }
    }

}