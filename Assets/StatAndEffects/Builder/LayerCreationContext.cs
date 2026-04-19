using System;
using System.Collections.Generic;
using SAPUnityEditorTools;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEngine;
using ZLinq;

namespace StatAndEffects.Builder
{
    [Serializable]
    public class LayerCreationContext : ISerializationCallbackReceiver
    {
        public SerializableDictionary<StatLayer, OperationCreationContext> layers 
            = new SerializableDictionary<StatLayer, OperationCreationContext>();

        public SerializableDictionary<StatModifierType, int> operationsData 
            = new SerializableDictionary<StatModifierType, int>();

        public void OnAfterDeserialize()
        {
            layers?.OnAfterDeserialize();
            operationsData?.OnAfterDeserialize();
            this.InitializeData();
        }

        public void OnBeforeSerialize()
        {
            layers?.OnBeforeSerialize();
            operationsData?.OnBeforeSerialize();
        }

        public void AddLayer(StatLayer statLayer)
        {
            layers.TryAdd(statLayer, new OperationCreationContext());
        }

        public void InitializeData()
        {
            if (layers == null || layers.Count == 0)
                return;
            
            operationsData = new SerializableDictionary<StatModifierType, int>();

            foreach (var layerPair in layers.AsValueEnumerable())
            {
                var ctx = layerPair.Value;
                if (ctx?.operations == null)
                    continue;

                foreach (var pair in ctx.operations.AsValueEnumerable())
                {
                    if (!operationsData.TryAdd(pair.Key, pair.Value))
                    {
                        operationsData[pair.Key] += pair.Value;
                    }
                }
            }
        }
    }
}