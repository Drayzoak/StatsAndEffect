using System.Collections;
using System.Collections.Generic;
using StatAndEffects.Stat;
namespace StatAndEffects
{
    public interface IEntityStats
    {
        
        public AbstractStat this[int index] { get;}
        public AbstractStat this[string name] { get;}
        
        public void AddStat(AbstractStat stat);
        
        public bool RemoveStat(AbstractStat stat);

        public TStat GetStat<TStat>(int id) where TStat : AbstractStat;

        public TStat GetStat<TStat>(StatDefinition statDefinition) where TStat : AbstractStat;
        public TStat GetStat<TStat>(string name) where TStat : AbstractStat;
        public AbstractStat GetStatAtIndex(int index);
        public IList GetStatsByType<T>() where T : AbstractStat;
        public IEnumerable<T> GetStatsOf<T>() where T : AbstractStat;
        
        public bool ContainsStat(string name);

        public bool ContainsStat(int id);
        
        public void ClearStats();
        

        public IEnumerator<AbstractStat> GetEnumerator();
    }
}