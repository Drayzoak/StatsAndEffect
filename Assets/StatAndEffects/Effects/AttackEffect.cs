using System;
using StatAndEffects.Stat;
namespace StatAndEffects.Effects
{
    [Serializable]
    public class AttackEffect : Effect
    {
        public StatDefinition statDefinition;
        private PrimaryStat attack;

        public override void Initialize(IEntityStats stats)
        {
            this.attack = stats.GetStat<PrimaryStat>(this.statDefinition);
        }

        public AttackEffect(PrimaryStat stat)
        {
            this.attack = stat;
        }

        public override float Process(float value)
        {
            return value * this.attack.Value;
        }
    }

}