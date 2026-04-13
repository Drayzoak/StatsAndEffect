using System;
using StatAndEffects.Stat;
namespace StatAndEffects.Effects
{
    [Serializable]
    public class ApplyVitalStat : Effect
    {
        public StatDefinition statDefinition;
        private VitalStat vital;
        
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

}