using System;
using System.Collections.Generic;
using UnityEngine;

namespace StatAndEffects.Effects
{
    [Serializable]
    public abstract class EffectPipeline
    {
        public string name;
        public bool negate;
        public EffectObject effectObject;
        
        private List<Effect> processors = new List<Effect>();
        
        public void Add(Effect p) => this.processors.Add(p);

        public virtual void Initialize(IEntityStats stats)
        {
            if (!this.effectObject)
            {
                StatLog.LogStatWarning("Effect pipeline is missing an effect object");
                return;
            }
            if (this.effectObject.processors.Count == 0)
            {
                StatLog.LogStatWarning($"Effect pipeline with {this.effectObject.name} has no processors");
                return;
            }

            this.processors = this.effectObject.LoadEffects(stats);

            foreach (Effect effect in this.processors)
            {
                effect.Initialize(stats);
            }

        }

        protected float ProcessInternal(float value)
        {
            if (this.negate)
            {
                value = -value;
            }

            for (int i = 0; i < this.processors.Count; i++)
            {
                value = this.processors[i].Process(value);
            }

            return value;
        }

        public abstract void ExecuteEffect(float value = 0);
    }

}