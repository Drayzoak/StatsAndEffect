using System;
using System.Collections.Generic;
using System.Linq;
using StatAndEffects.Stat;
using UnityEngine;

namespace StatAndEffects.Modifiers
{
    public static class Registry
    {
        public static Dictionary<StatModifierType, ModifierOperationBase> CreateDefaultOperations(int capacity)
        {
            Dictionary<StatModifierType,ModifierOperationBase> operations = new ();
            operations.Add(StatModifierType.Flat, new FlatModifierOperation(capacity));
            operations.Add(StatModifierType.Multiplicative, new MultiplicativeModifierOperation(capacity));
            operations.Add(StatModifierType.Additive, new AdditiveModifierOperation(capacity));
            
            return operations;
        }

        public static Dictionary<StatLayer, ModifierCollection> CreateDefaultLayers(int capacity)
        {
            Dictionary<StatLayer, ModifierCollection> layers = new ();
            layers.Add(StatLayer.Base, new ModifierCollection(capacity));
            layers.Add(StatLayer.Gear, new ModifierCollection(capacity));
            return layers;
        }

        public static ModifierOperationBase CreateModifierCollection(StatModifierType statModifierType, int capacity)
        {
            return statModifierType switch
            {
                StatModifierType.Flat => new FlatModifierOperation(capacity),
                StatModifierType.Additive => new AdditiveModifierOperation(capacity),
                StatModifierType.Multiplicative => new MultiplicativeModifierOperation(capacity),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
