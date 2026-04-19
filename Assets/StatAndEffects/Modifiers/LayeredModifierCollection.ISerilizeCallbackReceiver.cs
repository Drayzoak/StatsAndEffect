using System.Collections.Generic;
using StatAndEffects.Builder;
using UnityEngine;
using ZLinq;
namespace StatAndEffects.Modifiers
{
    public sealed partial class LayeredModifierCollection : ISerializationCallbackReceiver
    {
        [SerializeReference]
        private LayerCreationContext _layerCreationContext;

        [SerializeReference]
        private List<StatModifier> _serializedModifiers = new();

        public void OnBeforeSerialize()
        {
            if (!_initialized)
                return;

            _serializedModifiers.Clear();

            foreach (var layer in _layers.Values.AsValueEnumerable())
            {
                var mods = layer.GetModifiers();

                for (int i = 0; i < mods.Count; i++)
                {
                    _serializedModifiers.Add(mods[i]);
                }
            }
        }
        
        public void OnAfterDeserialize()
        {
            _initialized = false;

            Initialize();

            if (_serializedModifiers != null && _serializedModifiers.Count > 0)
            {
                TryAddModifiers(_serializedModifiers);
            }

            _isDirty = true;
        }
    }
}