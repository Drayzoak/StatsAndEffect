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
            effectsLookup.Clear();

            foreach (var pipeline in effects)
            {
                if (pipeline == null || string.IsNullOrEmpty(pipeline.name))
                    continue;

                pipeline.Initialize(this);

                if (!effectsLookup.ContainsKey(pipeline.name))
                    effectsLookup.Add(pipeline.name, pipeline);
            }

            effectsBuilt = true;
        }

        public float ExecuteEffect(string effectName, float value = 1)
        {
            EnsureEffectLookup();

            if (!effectsLookup.TryGetValue(effectName, out var pipeline))
                return 0;

            if (pipeline is InstantEffectPipeline instant)
            {
                instant.ExecuteEffect(value);
                return instant.result;
            }

            pipeline.ExecuteEffect(value);
            return 0;
        }
        
        public bool AddEffect(EffectPipeline effect)
        {
            if (effect == null || string.IsNullOrEmpty(effect.name))
                return false;

            EnsureEffectLookup();

            if (effectsLookup.ContainsKey(effect.name))
                return false; // prevent duplicate

            effects.Add(effect);

            effect.Initialize(this);
            effectsLookup.Add(effect.name, effect);

            return true;
        }
        
        public bool RemoveEffect(string effectName)
        {
            EnsureEffectLookup();

            if (!effectsLookup.TryGetValue(effectName, out var effect))
                return false;

            effects.Remove(effect);
            effectsLookup.Remove(effectName);

            return true;
        }
        
        public bool RemoveEffect(EffectPipeline effect)
        {
            if (effect == null)
                return false;

            if (!effects.Remove(effect))
                return false;

            effectsLookup.Remove(effect.name);
            return true;
        }
        
        public bool TryGetEffect<T>(string name, out T effect) where T : EffectPipeline
        {
            EnsureEffectLookup();

            if (effectsLookup.TryGetValue(name, out var baseEffect) && baseEffect is T typedEffect)
            {
                effect = typedEffect;
                return true;
            }

            effect = null;
            return false;
        }
        
        public void ClearEffects()
        {
            effects.Clear();
            effectsLookup.Clear();
            effectsBuilt = false;
        }
    }
}