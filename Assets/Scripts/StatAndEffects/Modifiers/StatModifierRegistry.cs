using System.Collections.Generic;
namespace StatAndEffects.Modifiers
{
    public static class StatModifierRegistry
    {
        private static readonly Dictionary<string, StatModifier> _map = new();

        public static void Clear()
        {
            _map.Clear();
        }

        public static void Register(this StatModifier modifier)
        {
            if (modifier == null)
                return;

            _map.TryAdd(modifier.Guid, modifier);
        }

        public static StatModifier Resolve(this StatModifier modifier)
        {
            if (modifier == null)
                return null;

            if (_map.TryGetValue(modifier.Guid, out var existing))
                return existing;

            _map.Add(modifier.Guid, modifier);
            return modifier;
        }
    }
}