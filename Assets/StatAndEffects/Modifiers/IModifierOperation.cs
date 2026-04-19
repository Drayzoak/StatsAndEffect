using System.Collections.Generic;
namespace StatAndEffects.Modifiers
{
    public interface IModifierOperation
    {
        float CalculateModifier(float baseValue, float modifiedValue);
        bool TryAddModifier(StatModifier statModifier);
        bool TryRemoveModifier(StatModifier statModifier);
        void RemoveAllModifiers();
        List<StatModifier> GetAllModifiers();
        
        bool ContainsModifier(StatModifier statModifier);
        StatModifier GetModifierAt(int index);

        bool HasCapacity();
        public int Capacity { get; set; }
        public int Count { get; }
    }

}