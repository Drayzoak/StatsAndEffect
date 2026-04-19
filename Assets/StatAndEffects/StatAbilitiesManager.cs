using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using StatAndEffects.Stat;
using UnityEngine;

namespace StatAndEffects
{
    public static class StatAbilitiesManager  
    {
        private static string assetLocation = "Assets/Private/ScriptableObject";
        private static readonly List<StatDefinition> BaseStats = new List<StatDefinition>();

        public static void RegisterBaseStat(StatDefinition statDefinition)
        {
            if (BaseStats.Contains(statDefinition))
            {
                Debug.LogWarning("Base stat already registered");
            }
            BaseStats.Add(statDefinition);
            Debug.Log("Registered Base Stat: " + statDefinition);
        }

        public static StatDefinition TryToGetValue(string param)
        {
            return BaseStats.FirstOrDefault(t => 
                string.Equals(t.DisplayName, param, StringComparison.OrdinalIgnoreCase));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            BaseStats.Clear();
            AssetFinder.FindAssets<StatDefinition>(assetLocation)
                .ForEach(RegisterBaseStat);
        }
    }
}