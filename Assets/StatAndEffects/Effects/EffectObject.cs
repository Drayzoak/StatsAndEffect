using System;
using System.Collections.Generic;
using StatAndEffects.Stat;
using UnityEngine;
namespace StatAndEffects.Effects
{
    [Serializable]
    [CreateAssetMenu(fileName = "EffectObject", menuName = "StatAndEffects/Effect")]
    public class EffectObject : ScriptableObject
    {
        [SerializeReference]
        public List<Effect> processors = new List<Effect>();
        
        public List<Effect> LoadEffects(IEntityStats stats)
        {
            List<Effect> effects = new List<Effect>();
            foreach (Effect effect in this.processors)
            {
                effects.Add(effect.Clone());
            }
            return effects;
        }
    }
}