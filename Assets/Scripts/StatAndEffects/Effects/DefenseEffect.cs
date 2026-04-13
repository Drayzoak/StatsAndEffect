using System;
using StatAndEffects.Stat;
namespace StatAndEffects.Effects
{
    [Serializable]
    public class DefenseEffect : Effect
    {
        public StatDefinition statDefinition;
        private PrimaryStat defense;
        
        public override void Initialize(IEntityStats stats)
        {
            this.defense = stats.GetStat<PrimaryStat>(this.statDefinition);
        }

        public override float Process(float value)
        {
            float def = this.defense.Value;
            return value * (100f / (100f + def));
        }
    }

}