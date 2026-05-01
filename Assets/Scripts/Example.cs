using System;
using System.Collections.Generic;
using StatAndEffects.Builder;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;
using UnityEngine;
namespace DefaultNamespace
{
    public class Example : MonoBehaviour
    {
         public VitalStat health =
             new VitalStat();
        
[SerializeReference]
public VitalStat mana
    = new VitalStat(50, 30, 0, "Mana", 2,
        layerCreationContext : new StatBuilder()
            .AddLayer(StatLayer.Base)
            .AddOperation(StatModifierType.Flat, 24)
            .AddLayer(StatLayer.Gear)
            .AddDefaultOperation(32).Build());

        [SerializeReference]
        public VitalStat sanity
            = new VitalStat(baseValue: 50, currentValue: 40, minValue: 0, statDefinition: "sanity", digitAccuracy: 2,
                layerCreationContext: StatCreationRegistry.Default(24));

        [SerializeReference]
        public PrimaryStat strength = new PrimaryStat();

        [SerializeReference]
        public PrimaryStat dexterity = 
            new PrimaryStat(5,"dexterity",2,
                layerCreationContext : new StatBuilder()
                    .AddLayer(StatLayer.Base)
                    .AddOperation(StatModifierType.Flat, 24).Build());

        [SerializeReference]
        public PrimaryStat constitution = new PrimaryStat(baseValue: 10, statDefinition: "constitution", digitAccuracy: 1, layerCreationContext: StatCreationRegistry.Default(24));
        
        [SerializeReference]
        public LevelStat playerLevel = new LevelStat();

        [SerializeReference]
        public LevelStat characterLevel = new LevelStat(level: 2, maxLevel: 10, baseXp: 100, growthFactor: 1.2f, statDefinition: "characterLevel", digitAccuracy: 1, 
            layerCreationContext: StatCreationRegistry.Default(24), 
            XpLayerCreationContext: StatCreationRegistry.Default(24));

        public List<StatModifier> BaseModifiers = new List<StatModifier>()
        {
            new StatModifier(StatModifierType.Flat,10,StatLayer.Base),
            new StatModifier(StatModifierType.Additive,20,StatLayer.Base),
            new StatModifier(StatModifierType.Multiplicative,30,StatLayer.Base),
        };
        
    }
}