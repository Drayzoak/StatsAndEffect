using System;
using System.Collections;
using System.Collections.Generic;
using StatAndEffects.Stat;
using UnityEngine;
namespace StatAndEffects
{
    [Serializable]
    public sealed partial class EntityStatsContainer : IEntityStats
    {
        [SerializeReference]
        public List<AbstractStat> StatsList = new List<AbstractStat>();

        
        private Dictionary<int, AbstractStat> statById = new();
        private Dictionary<string, AbstractStat> statByName = new();

        private bool lookupBuilt = false;

        public AbstractStat this[int index] => this.StatsList[index];
        public AbstractStat this[string statName] => this.GetStat<AbstractStat>(statName);

        void EnsureStatLookup()
        {
            if (this.lookupBuilt)
                return;

            this.RebuildLookup();
        }

        void RebuildLookup()
        {
            this.statById.Clear();
            this.statByName.Clear();

            for (int i = 0; i < this.StatsList.Count; i++)
            {
                var stat = this.StatsList[i];
                if (stat?.StatDefinition == null)
                    continue;

                var def = stat.StatDefinition;

                this.statById[def.Id] = stat;
                this.statByName[def.DisplayName] = stat;
                this.statByName[def.ShortName] = stat;
            }

            this.lookupBuilt = true;
        }


        public void AddStat(AbstractStat stat)
        {
            if (stat == null)
            {
                StatLog.LogStatError("AddStat failed: stat is null.");
                return;
            }

            this.EnsureStatLookup();

            var def = stat.StatDefinition;

            if (this.statById.ContainsKey(def.Id))
            {
                StatLog.LogStatWarning($"Stat '{def.DisplayName}' already exists.");
                return;
            }

            this.StatsList.Add(stat);

            this.statById[def.Id] = stat;
            this.statByName[def.DisplayName] = stat;
            this.statByName[def.ShortName] = stat;

            StatLog.LogStatInfo($"Stat added: '{def.DisplayName}'");
        }

        public bool RemoveStat(AbstractStat stat)
        {
            if (stat == null)
            {
                StatLog.LogStatError("RemoveStat failed: stat is null.");
                return false;
            }

            this.EnsureStatLookup();

            if (!this.StatsList.Remove(stat))
            {
                StatLog.LogStatWarning("RemoveStat failed: stat not found.");
                return false;
            }

            var def = stat.StatDefinition;

            this.statById.Remove(def.Id);
            this.statByName.Remove(def.DisplayName);
            this.statByName.Remove(def.ShortName);

            StatLog.LogStatInfo($"Stat removed: {def.DisplayName}");

            return true;
        }
        
        public TStat GetStat<TStat>(int id) where TStat : AbstractStat
        {
            this.EnsureStatLookup();

            if (this.statById.TryGetValue(id, out var stat))
                return stat as TStat;

            return null;
        }

        public TStat GetStat<TStat>(StatDefinition statDefinition) where TStat : AbstractStat
        {
            return this.GetStat<TStat>(statDefinition.Id);
        }
        
        public TStat GetStat<TStat>(string name) where TStat : AbstractStat
        {
            if (string.IsNullOrEmpty(name))
            {
                StatLog.LogStatError("GetStat failed: name is null or empty.");
                return null;
            }

            this.EnsureStatLookup();

            if (this.statByName.TryGetValue(name, out var stat))
                return stat as TStat;

            StatLog.LogStatWarning($"Stat '{name}' not found.");
            return null;
        }

        public bool ContainsStat(int id)
        {
            this.EnsureStatLookup();
            return this.statById.ContainsKey(id);
        }

        public bool ContainsStat(string name)
        {
            this.EnsureStatLookup();
            return this.statByName.ContainsKey(name);
        }
        
        public void ClearStats()
        {
            this.StatsList.Clear();
            this.statById.Clear();
            this.statByName.Clear();
            this.lookupBuilt = true;

            StatLog.LogStatInfo("All stats cleared.");
        }

        public AbstractStat GetStatAtIndex(int index)
        {
            if (index < 0 || index >= this.StatsList.Count)
            {
                StatLog.LogStatError($"GetStatAtIndex failed: index {index} out of range.");
                return null;
            }

            return this.StatsList[index];
        }
        public IList GetStatsByType<T>() where T : AbstractStat
        {
            List<T> result = new List<T>();

            for (int i = 0; i < this.StatsList.Count; i++)
            {
                if (this.StatsList[i] is T typed)
                    result.Add(typed);
            }

            return result;
        }
        public IEnumerable<T> GetStatsOf<T>() where T : AbstractStat
        {
            for (int i = 0; i < this.StatsList.Count; i++)
            {
                if (this.StatsList[i] is T typed)
                    yield return typed;
            }
        }
        public IEnumerator<AbstractStat> GetEnumerator()
        {
            return this.StatsList.GetEnumerator();
        }

    }
}