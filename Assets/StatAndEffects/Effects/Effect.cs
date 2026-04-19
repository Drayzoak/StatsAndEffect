using System;
using StatAndEffects;
namespace StatAndEffects.Effects
{
    [Serializable]
    public abstract class Effect
    {
        public abstract void Initialize(IEntityStats stats);
        public abstract float Process(float value);

        public virtual Effect Clone()
        {
            return (Effect)this.MemberwiseClone();
        }

    }

}