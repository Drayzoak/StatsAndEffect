using System;
using System.Collections.Generic;
using StatAndEffects.Stat;
namespace StatAndEffects.Modifiers
{
    [Serializable]
    public abstract class ModifierOperationBase : IModifierOperation
    {
        protected List<StatModifier> StatModifiers;
        public int Capacity { get; set; }
        public int Count => this.StatModifiers.Count;

        protected ModifierOperationBase(int capacity)
        {
            this.Capacity = capacity;
            this.StatModifiers = new List<StatModifier>(this.Capacity);
        }

        protected ModifierOperationBase() : this(8) { }

        public bool TryAddModifier(StatModifier statModifier)
        {
            if(this.HasCapacity() && !this.StatModifiers.Exists(t => t.Equals(statModifier) ))
            {
                this.StatModifiers.Add(statModifier);
                return true;
            }
            return false;
        }
        
        public bool TryRemoveModifier(StatModifier statModifier) => this.StatModifiers.Remove(statModifier);

        public List<StatModifier> GetAllModifiers()
        {
            if (this.StatModifiers.Count == 0) return new List<StatModifier>();
            return this.StatModifiers;
        }
        public bool ContainsModifier(StatModifier statModifier)
        {
            for (int i = 0; i < this.StatModifiers.Count; i++)
            {
                if(statModifier.Equals(this.StatModifiers[i])) return true;
            }
            return false;
        }
        public StatModifier GetModifierAt(int index) => this.StatModifiers[index];
        public void RemoveAllModifiers() => this.StatModifiers.Clear();

        public abstract float CalculateModifier(float baseValue, float modifiedValue);

        public bool HasCapacity()
        {
            if (this.StatModifiers.Capacity != this.Capacity) this.StatModifiers.Capacity = this.Capacity;
            if (this.StatModifiers.Count < this.Capacity) return true;
            return false;
        }
    }
}