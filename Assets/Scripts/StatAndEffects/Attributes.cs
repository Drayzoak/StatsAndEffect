using System.Collections.Generic;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEngine;
using UnityEngine.Serialization;
using ZLinq;
namespace StatAndEffects
{
    public sealed class Attributes : MonoBehaviour 
    {
        public EntityStats entityStats;
        [FormerlySerializedAs("abstractStats")]
        [SerializeReference]
        public List<AttributeStat> attributeStats = new List<AttributeStat>();
        
        [ContextMenu("SetUp Stats")]
        public void SetUpStats()
        {
            Debug.Log("===== SETUP STATS START =====");

            if (this.entityStats == null)
            {
                Debug.LogError("entityStats is NULL");
                return;
            }

            if (this.entityStats.Container == null)
            {
                Debug.LogError("entityStats.Container is NULL");
                return;
            }

            this.entityStats.Container.Initialize();
            Debug.Log("Container initialized");

            if (this.attributeStats == null)
            {
                Debug.LogError("abstractStats list is NULL");
                return;
            }

            int totalAdded = 0;

            foreach (AttributeStat attributeStat in this.attributeStats.AsValueEnumerable())
            {
                if (attributeStat == null)
                {
                    Debug.LogWarning("AttributeStat is NULL, skipping...");
                    continue;
                }

                Debug.Log($"Processing AttributeStat: {attributeStat.StatDefinition?.name}");

                if (attributeStat.AttributeModifiers == null)
                {
                    Debug.LogWarning("AttributeModifiers is NULL");
                    continue;
                }

                int localCount = 0;

                foreach (KeyValuePair<StatDefinition, StatModifier> attributeModifier in attributeStat.AttributeModifiers)
                {
                    if (attributeModifier.Key == null)
                    {
                        Debug.LogWarning("StatDefinition key is NULL, skipping...");
                        continue;
                    }

                    if (attributeModifier.Value == null)
                    {
                        Debug.LogWarning($"Modifier for {attributeModifier.Key.name} is NULL, skipping...");
                        continue;
                    }

                    AbstractStat stat = this.entityStats.GetStat<AbstractStat>(attributeModifier.Key);

                    if (stat == null)
                    {
                        Debug.LogError($"Stat NOT FOUND in container: {attributeModifier.Key.name}");
                        continue;
                    }

                    bool added = stat.ModifiersCollection.TryAddModifier(attributeModifier.Value);
                    stat.MarkDirty();

                    Debug.Log(
                        added
                            ? $"✅ Added Modifier → {attributeModifier.Key.name} | Value: {attributeModifier.Value.Value}"
                            : $"❌ Duplicate/Failed → {attributeModifier.Key.name}"
                    );

                    if (added)
                    {
                        localCount++;
                        totalAdded++;
                    }
                }

                Debug.Log($"--> Added {localCount} modifiers for {attributeStat.StatDefinition?.name}");
            }

            Debug.Log($"===== SETUP COMPLETE | Total Added: {totalAdded} =====");
        }
    }
}