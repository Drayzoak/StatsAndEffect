using System.Collections.Generic;
using System.Linq;
using Common;
using StatAndEffects.Stat;
using UnityEngine;
namespace StatAndEffects.Manager
{
    [CreateAssetMenu(menuName = "StatAndEffects/Stat Database")]
    public class StatDatabase : ScriptableObject
    {
        [Header("📁 Asset Paths")]
        [SerializeField] private string databasePath;

        [SerializeField] private string statDefinitionPath;
        [SerializeField] private string attributeStatDefinitionPath;

        [Header("📊 Data")]
        [SerializeField] private List<StatDefinition> stats = new();

        public string DatabasePath => databasePath;
        public string StatDefinitionPath => statDefinitionPath;
        public string AttributeStatDefinitionPath => attributeStatDefinitionPath;

        public IReadOnlyList<StatDefinition> Stats => this.stats;
        // =========================
        // 🔹 LOOKUPS
        // =========================

        public StatDefinition GetByName(string name)
        {
            return this.stats.FirstOrDefault(s =>
                string.Equals(s.DisplayName, name, System.StringComparison.OrdinalIgnoreCase));
        }

        public StatDefinition GetById(int id)
        {
            return this.stats.FirstOrDefault(s => s.Id == id);
        }

        public void Clear()
        {
            stats.Clear();
        }

        public void AddStat(StatDefinition stat)
        {
            this.stats.Add(stat);
        }
    }
}