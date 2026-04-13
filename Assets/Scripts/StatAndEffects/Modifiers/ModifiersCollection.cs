using System;
using System.Collections.Generic;
using Common.Extensions;
using StatAndEffects.Builder;
using ZLinq;
using ZLinq.Linq;

namespace StatAndEffects.Modifiers
{
    public class ModifierCollection
    {
        private readonly SortedList<StatModifierType, ModifierOperationBase> _operations;
        private readonly List<StatModifier> _cache;

        public IEnumerable<ModifierOperationBase> Operations => _operations.Values;
        public IEnumerable<StatModifierType> Getkey => this._operations.Keys;
        public ModifierOperationBase this[StatModifierType type] => this._operations[type];
        public ValueEnumerable<FromEnumerable<KeyValuePair<StatModifierType, ModifierOperationBase>>, KeyValuePair<StatModifierType, ModifierOperationBase>> Enumerable => this._operations.AsValueEnumerable();
        public ModifierCollection(int capacity)
        {
            int opCount = EnumExtension.Length<StatModifierType>();

            this._operations = new(opCount);
            this._cache = new List<StatModifier>(capacity * opCount);

            var defaultOps = Registry.CreateDefaultOperations(capacity);

            foreach (var pair in defaultOps.AsValueEnumerable())
            {
                this._operations.Add(pair.Key, pair.Value);
            }
        }

        public ModifierCollection(OperationCreationContext context)
        {
            this._operations = new(context.operations.Count);
            int tempCapacity = 0;
            foreach (KeyValuePair<StatModifierType, int> pair in context.operations)
            {
                tempCapacity += pair.Value;
                ModifierOperationBase operation = Registry.CreateModifierCollection(pair.Key, pair.Value);
                this._operations.Add(pair.Key, operation);
            }
            this._cache = new(tempCapacity);
        }
        
        public bool TryAddModifier(StatModifier modifier)
        {
            if (!this._operations.TryGetValue(modifier.Type, out var op))
                return false;

            if (!op.HasCapacity())
                return false;

            return op.TryAddModifier(modifier);
        }

        public bool TryRemoveModifier(StatModifier modifier)
        {
            return this._operations.TryGetValue(modifier.Type, out var op) &&
                op.TryRemoveModifier(modifier);
        }

        public void Clear()
        {
            foreach (var op in this._operations.Values.AsValueEnumerable())
            {
                op.RemoveAllModifiers();
            }
        }

        internal List<StatModifier> GetModifiers()
        {
            this._cache.Clear();

            foreach (var op in this._operations.Values.AsValueEnumerable())
            {
                var list = op.GetAllModifiers().AsValueEnumerable();
                foreach (StatModifier statModifier in list)
                {
                    this._cache.Add(statModifier);
                }
            }

            return this._cache;
        }

        internal List<StatModifier> GetModifiersOf(StatModifierType type)
        {
            return this._operations.TryGetValue(type, out var op)
                ? op.GetAllModifiers()
                : EmptyList;
        }

        public void ForEachModifier(System.Action<StatModifier> action)
        {
            foreach (var op in this._operations.Values)
            {
                var list = op.GetAllModifiers().AsValueEnumerable();

                foreach (StatModifier statModifier in list)
                {
                    action(statModifier);
                }
            }
        }

        public void ForEachModifier(StatModifierType type, System.Action<StatModifier> action)
        {
            if (!this._operations.TryGetValue(type, out var op))
                return;

            var list = op.GetAllModifiers().AsValueEnumerable();

            foreach (StatModifier statModifier in list)
            {
                action(statModifier);
            }
        }

        public List<StatModifier> GetModifiersCopy()
        {
            var result = new List<StatModifier>(this._cache.Capacity);

            foreach (var op in this._operations.Values)
            {
                var list = op.GetAllModifiers().AsValueEnumerable();

                foreach (StatModifier statModifier in list)
                {
                    result.Add(statModifier);
                }
            }

            return result;
        }

        public List<StatModifier> GetModifiersCopy(StatModifierType type)
        {
            return this._operations.TryGetValue(type, out var op)
                ? new List<StatModifier>(op.GetAllModifiers())
                : new List<StatModifier>(0);
        }

        public bool ContainsModifier(StatModifier modifier)
        {
            return this._operations.TryGetValue(modifier.Type, out var op) &&
                op.ContainsModifier(modifier);
        }
        private static readonly List<StatModifier> EmptyList = new(0);

    }
}