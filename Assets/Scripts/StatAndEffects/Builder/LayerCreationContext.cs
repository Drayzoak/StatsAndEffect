using System;
using System.Collections.Generic;
using SAPUnityEditorTools;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEngine;
namespace StatAndEffects.Builder
{
    [Serializable]
    public class LayerCreationContext : ISerializationCallbackReceiver
    {
        
        public SerializableDictionary<StatLayer, OperationCreationContext> layers 
            = new SerializableDictionary<StatLayer, OperationCreationContext>();
        

        public void OnAfterDeserialize()
        {
            layers?.OnAfterDeserialize(); // FORCE restore
        }

        public void OnBeforeSerialize()
        {
            layers?.OnBeforeSerialize();
        }
        
        public void AddLayer(StatLayer statLayer)
        {
            this.layers.TryAdd(statLayer, new OperationCreationContext());
        }
    }
    
}