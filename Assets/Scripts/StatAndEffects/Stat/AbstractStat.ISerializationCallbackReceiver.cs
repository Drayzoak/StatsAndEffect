using System;
using System.Collections.Generic;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using UnityEngine;

namespace StatAndEffects.Stat
{
    public abstract partial class AbstractStat : ISerializationCallbackReceiver
    {
        
        [SerializeReference]
        private LayerCreationContext _layerCreationContext;
        [SerializeReference]
        private List<StatModifier> serializedStatModifiers = new();

        private bool _serializationDirty = true;

        public void MarkSerializationDirty()
        {
            _serializationDirty = true;
        }
        
        public void OnBeforeSerialize()
        {
            if (_modifiersCollection == null)
                return;

            serializedStatModifiers.Clear();

            var modifiers = GetModifiers();

            for (int i = 0; i < modifiers.Count; i++)
            {
                serializedStatModifiers.Add(modifiers[i]);
            }
        }

        public void OnAfterDeserialize()
        {
            if (this._layerCreationContext != null)
            {
                this._layerCreationContext.layers.EnsureInitialized();
            }
            this.InitializeLayerCollection();

            if (serializedStatModifiers == null || serializedStatModifiers.Count == 0)
                return;
            
            this._modifiersCollection.TryAddModifiers(serializedStatModifiers);
        }
        
        private bool IsValidLayerContext()
        {
            return _layerCreationContext != null && _layerCreationContext.layers != null && _layerCreationContext.layers.Count > 0;
        }
    }
}