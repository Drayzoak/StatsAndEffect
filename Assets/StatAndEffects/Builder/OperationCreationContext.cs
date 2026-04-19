using System;
using System.Collections.Generic;

using SAPUnityEditorTools;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEngine;

namespace StatAndEffects.Builder
{
    [Serializable]
    public class OperationCreationContext : ISerializationCallbackReceiver
    {
        public SerializableDictionary<StatModifierType,int> operations 
            = new SerializableDictionary<StatModifierType,int>();
        public void AddOperation(StatModifierType type, int value)
        {
            if (operations.ContainsKey(type))
            {
                operations[type] = value; // update
            }
            else
            {
                operations.TryAdd(type, value);
            }
        }

        public bool TryGetOperation(StatModifierType type, out int value)
        {
            return operations.TryGetValue(type, out value);
        }
        
        public void OnAfterDeserialize()
        {
            operations?.OnAfterDeserialize(); // FORCE restore
        }

        public void OnBeforeSerialize()
        {
            operations?.OnBeforeSerialize();
        }
        
        
    }
}