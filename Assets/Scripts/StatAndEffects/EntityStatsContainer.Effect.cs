using System;
using System.Collections.Generic;
using StatAndEffects.Effects;
using UnityEngine;
using UnityEngine.Serialization;
namespace StatAndEffects
{
    public sealed partial class EntityStatsContainer
    {
        [SerializeReference]
        public List<EffectPipeline> effects = new List<EffectPipeline>();
        
        private Dictionary<string, EffectPipeline> effectsLookup = new Dictionary<string, EffectPipeline>();
        private bool effectsBuilt = false;
        public void Initialize()
        {
            this.EnsureStatLookup();
            
        }

        private void EnsureEffectLookup()
        {
            if (this.effectsBuilt)
                return;
            
            this.RebuildEffects();
        }

        void RebuildEffects()
        {
            this.effectsLookup.Clear();

            foreach (EffectPipeline pipeline in this.effects)
            {
                pipeline.Initialize(this);
                this.effectsLookup.Add(pipeline.name, pipeline);
            }
            this.effectsBuilt = true;
        }

        public float ExecuteEffect(string effectName, float value = 1)
        {
            this.EnsureEffectLookup();

            if (this.effectsLookup.TryGetValue(effectName, out EffectPipeline pipeline))
            {
                if (pipeline is InstantEffectPipeline instantEffect)
                {
                    instantEffect.ExecuteEffect(value);
                    Debug.Log($"Effect {effectName}: {instantEffect.result}");
                    return instantEffect.result;
                }
                pipeline.ExecuteEffect(value);
            }
            return 0;
        }
    }
    
}