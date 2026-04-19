using System;
using System.Collections;
using System.Collections.Generic;
using StatAndEffects.Stat;
using UnityEngine;
namespace StatAndEffects
{
    public sealed class EntityStats : MonoBehaviour, IEntityStats
    {
        [SerializeReference]
        public EntityStatsContainer Container = new EntityStatsContainer();

        public AbstractStat this[int index] => this.Container[index];
        public AbstractStat this[string statName] => this.Container[statName];
   
        public void AddStat(AbstractStat stat) => this.Container.AddStat(stat);
        public bool RemoveStat(AbstractStat stat) => this.Container.RemoveStat(stat);
        
        public TStat GetStat<TStat>(int id) where TStat : AbstractStat => this.Container.GetStat<TStat>(id);
        public TStat GetStat<TStat>(StatDefinition statDefinition) where TStat : AbstractStat => this.Container.GetStat<TStat>(statDefinition);
        public TStat GetStat<TStat>(string name) where TStat : AbstractStat => this.Container.GetStat<TStat>(name);
        public AbstractStat GetStatAtIndex(int index) => this.Container.GetStatAtIndex(index);
        public IList GetStatsByType<T>() where T : AbstractStat => this.Container.GetStatsByType<T>();
        public IEnumerable<T> GetStatsOf<T>() where T : AbstractStat => this.Container.GetStatsOf<T>();
        
        public bool ContainsStat(string name) => this.Container.ContainsStat(name);
        public bool ContainsStat(int id) => this.Container.ContainsStat(id);
        
        public void ClearStats() => this.Container.ClearStats();
        public IEnumerator<AbstractStat> GetEnumerator() => this.Container.GetEnumerator();

        public void Awake() => this.Container.Initialize();
        
    }

}