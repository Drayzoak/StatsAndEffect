using System;
using System.Collections.Generic;
using StatAndEffects.Builder;
using StatAndEffects.Stat;
using UnityEngine;
using ZLinq;
namespace StatAndEffects.Modifiers
{
    
    public class LayeredModifierCollection
    {
        private readonly Dictionary<StatLayer,ModifierCollection> _layers;
        private readonly Dictionary<StatModifierType, List<ModifierOperationBase>> _operations;
        
        private bool _isDirty = true;
        private float _cachedValue;
        private float _lastBaseValue;

        private readonly List<StatModifier> _tempCache;

        public ModifierCollection this[StatLayer layer] => this._layers[layer];

        public LayeredModifierCollection(int capacity = 16)
        {
            this._layers = Registry.CreateDefaultLayers(capacity);
            this._operations = new();
            
            foreach (ModifierCollection value in this._layers.Values.AsValueEnumerable()) 
            {
                foreach (KeyValuePair<StatModifierType, ModifierOperationBase> operation in value.Enumerable)
                {
                    if (!this._operations.ContainsKey(operation.Key))
                        this._operations.Add(operation.Key, new List<ModifierOperationBase>());
                    this._operations[operation.Key].Add(operation.Value);
                }
            }
            this._tempCache = new(capacity*2);
        }

        public LayeredModifierCollection(LayerCreationContext layerCreationContext)
        {
            if (layerCreationContext == null || layerCreationContext.layers == null)
            {
                throw new ArgumentNullException(nameof(layerCreationContext), "LayerCreationContext is null or invalid");
            }

            int layerCount = layerCreationContext.layers.Count;

            _layers = new Dictionary<StatLayer, ModifierCollection>(layerCount);
            _operations = new Dictionary<StatModifierType, List<ModifierOperationBase>>(8);
            
            foreach (var pair in layerCreationContext.layers)
            {
                if (pair.Value == null)
                    continue;

                ModifierCollection modifierCollection = new ModifierCollection(pair.Value);
                _layers.Add(pair.Key, modifierCollection);
                
                foreach (KeyValuePair<StatModifierType, ModifierOperationBase> operation in modifierCollection.Enumerable)
                {
                    if (!this._operations.ContainsKey(operation.Key))
                        this._operations.Add(operation.Key, new List<ModifierOperationBase>(8));
                    this._operations[operation.Key].Add(operation.Value);
                }
            }
            
        }


        public void MarkDirty() => this._isDirty = true;

        public float Evaluate(float baseValue)
        {
            if (!this._isDirty && Mathf.Approximately(baseValue, this._lastBaseValue))
                return this._cachedValue;

            this._lastBaseValue = baseValue;

            float value = baseValue;

            foreach (var operations in this._operations.Values.AsValueEnumerable())
            {
                foreach (var operation in operations.AsValueEnumerable())
                {
                    value += operation.CalculateModifier(baseValue,value);
                }
            }

            this._cachedValue = value;
            this._isDirty = false;

            return value;
        }


        public bool TryAddModifier(StatModifier modifier)
        {
            if (this._layers[modifier.Layer].TryAddModifier(modifier))
            {
                this._isDirty = true;
                return true;
            }

            return false;
        }

        public bool TryRemoveModifier(StatModifier modifier)
        {
            if (this._layers[modifier.Layer].TryRemoveModifier(modifier))
            {
                this._isDirty = true;
                return true;
            }

            return false;
        }

        public void ClearModifiers()
        {
            foreach (var modifierCollection in this._layers.Values.AsValueEnumerable())
            {
                modifierCollection.Clear();
            }

            this._isDirty = true;
        }

        public (bool allAdded, List<StatModifier> failedModifiers) TryAddModifiers(IEnumerable<StatModifier> modifiers)
        {
            this._tempCache.Clear();

            foreach (var m in modifiers.AsValueEnumerable())
            {
                if (!this.TryAddModifier(m))
                    this._tempCache.Add(m);
            }

            this._isDirty = true;
            return (this._tempCache.Count == 0, this._tempCache);
        }

        public (bool allRemoved, List<StatModifier> failedModifiers) TryRemoveModifiers(IEnumerable<StatModifier> modifiers)
        {
            this._tempCache.Clear();

            foreach (var m in modifiers.AsValueEnumerable())
            {
                if (!this.TryRemoveModifier(m))
                    this._tempCache.Add(m);
            }

            this._isDirty = true;
            return (this._tempCache.Count == 0, this._tempCache);
        }

        internal List<StatModifier> GetModifiers(StatLayer layer, StatModifierType type)
        {
            return this._layers[layer].GetModifiersOf(type);
        }

        internal List<StatModifier> GetModifiers(StatLayer layer)
        {
            return this._layers[layer].GetModifiers();
        }

        public void ForEachModifier(Action<StatModifier> action)
        {
            foreach (var modifierCollection in this._layers.Values.AsValueEnumerable())
            {
                modifierCollection.ForEachModifier(action);
            }
        }

        public void ForEachModifier(StatModifierType type, Action<StatModifier> action)
        {
            foreach (var modifierCollection in this._layers.Values.AsValueEnumerable())
            {
                modifierCollection.ForEachModifier(type,action);
            }
        }

        public void ForEachModifier(StatLayer layer, Action<StatModifier> action)
        {
            this._layers[layer].ForEachModifier(action);
        }

        public void ForEachModifier(StatLayer layer, StatModifierType type, Action<StatModifier> action)
        {
            this._layers[layer].ForEachModifier(type, action);
        }

        public List<StatModifier> GetAllModifiersCopy()
        {
            var result = new List<StatModifier>();
            
            foreach (var modifierCollection in this._layers.Values.AsValueEnumerable())
            {
                foreach (var statModifier in modifierCollection.GetModifiers().AsValueEnumerable())
                {
                    result.Add(statModifier);
                }
            }
            return result;
        }

        public List<StatModifier> GetModifiersCopy(StatModifierType statModifierType)
        {
            var result = new List<StatModifier>();

            foreach (var modifierCollection in this._layers.Values.AsValueEnumerable())
            {
                foreach (var statModifier in modifierCollection.GetModifiersOf(statModifierType).AsValueEnumerable())
                {
                    result.Add(statModifier);
                }
            }
            return result;
        }
        
        public List<StatModifier> GetModifiersCopy(StatLayer layer)
        {
            var source = this._layers[layer].GetModifiers();
            return new List<StatModifier>(source);
        }

        public List<StatModifier> GetModifiersCopy(StatLayer layer, StatModifierType type)
        {
            var source = this._layers[layer].GetModifiersOf(type);
            return new List<StatModifier>(source);
        }

        public bool ContainsModifier(StatModifier modifier)
        {
            foreach (var modCollection in this._layers.Values.AsValueEnumerable())
            {
                if(modCollection.ContainsModifier(modifier))
                    return true;
            }
            return false;
        }
        
        public bool ContainsModifier(StatModifier statModifier , StatLayer statLayer) 
            => this._layers[statLayer].ContainsModifier(statModifier);
        
        public Dictionary<StatLayer, ModifierCollection>.Enumerator GetEnumerator() 
            => this._layers.GetEnumerator();
        

    }
}