using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
namespace StatAndEffects.Builder
{
    public static class StatCreationRegistry
    {
        public static LayerCreationContext Default(int capacity) => 
            new StatBuilder()
                .AddLayer(StatLayer.Base)
                .AddDefaultOperation(capacity)
                .AddLayer(StatLayer.Gear)
                .AddOperation(StatModifierType.Flat,capacity)
                .AddOperation(StatModifierType.Additive,capacity)
                .AddOperation(StatModifierType.Multiplicative,capacity)
                .AddLayer(StatLayer.Buff)
                .AddDefaultOperation(capacity)
                .Build();
        
    }
}