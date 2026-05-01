using System.Collections.Generic;
using StatAndEffects.Manager;
using UnityEngine;
#if UNITY_EDITOR
using GameLogging;
using UnityEditor;
#endif
using StatAndEffects.Stat;

namespace StatAndEffects
{
    public static class StatAbilitiesManager
    {
        private static StatDatabase _database;

        public static StatDatabase Database { get => _database; }
        public static IReadOnlyList<StatDefinition> BaseStats
            => _database?.Stats;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeRuntime()
        {
            LoadDatabase();
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void InitializeEditor()
        {
            LoadDatabase();
        }
#endif

        public static void LoadDatabase()
        {
#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:StatDatabase");

            if (guids.Length == 0)
            {
                GameLog.Error("Manager","No StatDatabase found!");
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _database = AssetDatabase.LoadAssetAtPath<StatDatabase>(path);
            guids = AssetDatabase.FindAssets("t:StatDefinition " , new[]
            {
                _database.DatabasePath
            });

            _database.Clear();
            foreach (string guid in guids)
            {
                path = AssetDatabase.GUIDToAssetPath(guid);
                StatDefinition statDefinition = AssetDatabase.LoadAssetAtPath<StatDefinition>(path);
                if (statDefinition == null)
                    continue;
                _database.AddStat(statDefinition);
            }
            EditorUtility.SetDirty(_database);
            AssetDatabase.SaveAssets();
            
#else
            _database = Resources.Load<StatDatabase>("StatDatabase");
#endif
        }

        public static StatDefinition TryToGetValue(string name)
        {
            return _database?.GetByName(name);
        }

        public static StatDefinition GetById(int id)
        {
            return _database?.GetById(id);
        }

        
        public static void AddStat(StatDefinition stat)
        {
            _database.AddStat(stat);
        }
    }
}