using System;
using StatAndEffects.Stat;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
namespace StatAndEffects.Effects
{
    [Serializable]
    public class CriticalEffect : Effect
    {
        public StatDefinition critChanceDefinition;
        public StatDefinition critDamageDefinition;
        
        private PrimaryStat critIcalDamage;
        private PrimaryStat criticalChance;

        public override void Initialize(IEntityStats stats)
        {
            this.critIcalDamage = stats.GetStat<PrimaryStat>(this.critDamageDefinition);
            this.criticalChance = stats.GetStat<PrimaryStat>(this.critChanceDefinition);
        }
        public override float Process(float value)
        {
            if (Random.Range(0,100) <= this.criticalChance.Value)
            {
                return value * this.critIcalDamage.Value;
            }
            return value;
        }
    }
}