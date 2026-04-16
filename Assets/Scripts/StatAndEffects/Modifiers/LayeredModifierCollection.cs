using System;
using System.Collections.Generic;
using StatAndEffects.Builder;
using StatAndEffects.Stat;
using UnityEngine;
using ZLinq;
namespace StatAndEffects.Modifiers
{
    
    public sealed partial class LayeredModifierCollection 
    {
        private Dictionary<StatLayer,ModifierCollection> _layers;
        private Dictionary<StatModifierType, List<ModifierOperationBase>> _operations;
        
        private bool _isDirty = true;
        private float _cachedValue;
        private float _lastBaseValue;
        private List<StatModifier> _tempCache;
        
        [SerializeField]
        private int _capacity;
        private bool _initialized;
        
        public ModifierCollection this[StatLayer layer] => this._layers[layer];

        public LayeredModifierCollection(int capacity = 16)
        {
            this._capacity = capacity;
            this.Initialize();
        }

        public LayeredModifierCollection(LayerCreationContext layerCreationContext)
        {
            this._layerCreationContext = layerCreationContext;
            this.Initialize();
        }


        private void Initialize()
        {
            if (_initialized)
                return;

            if (_layerCreationContext != null &&
                _layerCreationContext.layers != null &&
                _layerCreationContext.layers.Count > 0)
            {
                CreateFromContext();
            }
            else
            {
                CreateDefault();
            }

            _initialized = true;
        }
        
        private void CreateDefault()
        {
            _layers = Registry.CreateDefaultLayers(this._capacity);
            
            _operations = new Dictionary<StatModifierType, List<ModifierOperationBase>>();

            foreach (var value in _layers.Values.AsValueEnumerable())
            {
                foreach (var operation in value.AsValueEnumerable())
                {
                    if (!_operations.TryGetValue(operation.Key, out var list))
                    {
                        list = new List<ModifierOperationBase>();
                        _operations.Add(operation.Key, list);
                    }

                    list.Add(operation.Value);
                }
            }

            _tempCache = new List<StatModifier>(32);
        }
        
        private void CreateFromContext()
        {
            _layerCreationContext.layers.EnsureInitialized();

            int layerCount = _layerCreationContext.layers.Count;

            _layers = new Dictionary<StatLayer, ModifierCollection>(layerCount);
            _operations = new Dictionary<StatModifierType, List<ModifierOperationBase>>(this._layerCreationContext.operationsData.Count);

            foreach (KeyValuePair<StatLayer, OperationCreationContext> pair in _layerCreationContext.layers.AsValueEnumerable())
            {
                OperationCreationContext operationContext = pair.Value;
                if (pair.Value == null)
                    continue;

                var modifierCollection = new ModifierCollection(operationContext);
                _layers.Add(pair.Key, modifierCollection);

                foreach (var operation in modifierCollection)
                {
                    if (!_operations.TryGetValue(operation.Key, out var list))
                    {
                        list = new List<ModifierOperationBase>(operationContext.operations[operation.Key]);
                        _operations.Add(operation.Key, list);
                    }

                    list.Add(operation.Value);
                }
            }

            _tempCache = new List<StatModifier>(32);
        }
        public void MarkDirty() => this._isDirty = true;

        public float Evaluate(float baseValue)
        {
            this.Initialize();
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