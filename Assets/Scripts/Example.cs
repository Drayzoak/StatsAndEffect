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
        [SerializeReference]
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
            = new VitalStat(50,40,0,
                layerCreationContext : StatCreationRegistry.Default(24));

        [SerializeReference]
        public PrimaryStat strength = new PrimaryStat();

        [SerializeReference]
        public PrimaryStat dexterity = 
            new PrimaryStat(5,"dexterity",2,
                layerCreationContext : new StatBuilder()
                    .AddLayer(StatLayer.Base)
                    .AddOperation(StatModifierType.Flat, 24).Build());
        
        [SerializeReference]
        public PrimaryStat constitution = new PrimaryStat(10,layerCreationContext: StatCreationRegistry.Default(24));
        
        [SerializeField]
        public LevelStat playerLevel = new LevelStat();
        
        [SerializeField]
        public LevelStat characterLevel = new LevelStat(0,100,1.2f);

        public List<StatModifier> statModifiers = new List<StatModifier>()
        {
            new StatModifier(StatModifierType.Flat,10,StatLayer.Base),
            new StatModifier(StatModifierType.Additive,20,StatLayer.Base),
            new StatModifier(StatModifierType.Multiplicative,30,StatLayer.Base),
        };
        
        private void Awake()
        {
            (bool allAdded, List<StatModifier> failedModifiers) addModifiers =this.health.ModifiersCollection.TryAddModifiers(statModifiers);
            
            (bool allRemoved, List<StatModifier> failedModifiers) removeModifiers = this.health.ModifiersCollection.TryRemoveModifiers(statModifiers);
        }
    }
}