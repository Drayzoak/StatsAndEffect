
# StatsAndEffect

A flexible and extensible stat system for Unity built around layered modifier evaluation. It supports flat, additive, and multiplicative modifiers, custom modifier types, and efficient runtime updates with minimal allocations. Designed for RPGs and complex gameplay systems.
## Stat Types

The system provides three different stat implementations, each designed for specific gameplay use cases:

### 🔹 PrimaryStat
Used for basic attributes such as Attack, Defense, Strength, or Dexterity.  
These are simple stats that rely purely on modifier evaluation without additional internal logic.

### 🔹 VitalStat
Designed for dynamic, consumable-based stats like Health, Stamina, or Hunger.  
These stats typically involve current/max values and regeneration or depletion behavior.

### 🔹 LevelStat
Handles progression-based stats such as Levels and Experience (XP).  
Includes built-in logic for XP accumulation, level-ups, and scaling requirements.
## Usage/Examples
### Stats
Simple Stat Creation.
```csharp
[SerializeReference] // Important: Required if you want the stat to be visible/editable in the Inspector And If is Public
public VitalStat health = new VitalStat();
        
[SerializeReference]
public PrimaryStat strength = new PrimaryStat();


[SerializeReference]
public LevelStat playerLevel = new LevelStat();
        
```

With Custom Values & Default LayerCreationContex with Modifier     Capacity in StatRegistry
```csharp
[SerializeReference]
public PrimaryStat constitution 
    = new PrimaryStat(baseValue: 10, statDefinition: "constitution", digitAccuracy: 1, 
        layerCreationContext: StatCreationRegistry.Default(24));

[SerializeReference]
public VitalStat sanity
    = new VitalStat(baseValue: 50, currentValue: 40, minValue: 0, statDefinition: "sanity", digitAccuracy: 2,
        layerCreationContext: StatCreationRegistry.Default(24));

[SerializeReference]
public LevelStat characterLevel = 
    new LevelStat(level: 2, maxLevel: 10, baseXp: 100, growthFactor: 1.2f, statDefinition: "characterLevel", digitAccuracy: 1, 
        layerCreationContext: StatCreationRegistry.Default(24), 
        XpLayerCreationContext: StatCreationRegistry.Default(24));
```

With Custom Created LayerCreationContex Using StatBuilder

```csharp
[SerializeReference]
public VitalStat mana
    = new VitalStat(50, 30, 0, "Mana", 2,
        layerCreationContext : new StatBuilder()
            .AddLayer(StatLayer.Base)
            .AddOperation(StatModifierType.Flat, 24)
            .AddLayer(StatLayer.Gear)
            .AddDefaultOperation(32).Build());

[SerializeReference]
public PrimaryStat dexterity = 
    new PrimaryStat(5,"dexterity",2,
        layerCreationContext : new StatBuilder()
            .AddLayer(StatLayer.Base)
            .AddOperation(StatModifierType.Flat, 24).Build());

```

### EntityStats
```csharp
using System;
using StatAndEffects;
using StatAndEffects.Effects;
using StatAndEffects.Stat;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public EntityStats stats;
    
    public StatDefinition defenceDefinition;

    private void Start()
    {
    
        //Get Stat
        var health = this.stats.GetStat<VitalStat>("Health"); //using name (we can Use DisplayName/ShortName Here)  
        var attack = this.stats.GetStat<PrimaryStat>(2); // bu statdefination id
        var defence = this.stats.GetStat<PrimaryStat>(this.defenceDefinition); // by statdefination itself
        
        //creating luck stat and add it to entitystats
        var luck = new PrimaryStat(9,"Luck"); //here we need LuckDefinition scriptable object create in assets
        this.stats.AddStat(luck); 
        this.stats.RemoveStat(luck);
    }
}
```
#### Effects

```csharp
using System;
using StatAndEffects;
using StatAndEffects.Effects;
using StatAndEffects.Stat;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public EntityStats stats;

    public InstantEffectPipeline calculateDamage;
    public InstantEffectPipeline applyDamage;
    public EffectPipeline damage;
    
    
    private void Awake()
    {
        if(!this.stats.Container.TryGetEffect("CalculateDamage", out this.calculateDamage))
            Debug.LogError(this.calculateDamage + " is not a valid effect.");
        
        if(!this.stats.Container.TryGetEffect("Damage", out this.damage))
            Debug.LogError(this.damage + " is not a valid effect.");
    }

    public float CalculateDamage()
    {
        this.calculateDamage.ExecuteEffect(60);
        return this.calculateDamage.result;
    }

    public void ApplyDamage(float damage)
    {
        this.damage.ExecuteEffect(damage);
    }
}
```
```csharp
    public EntityStats stats;
    
    void Start()
    {
        stats = GetComponent<EntityStats>();    
        float damage = this.stats.Container.ExecuteEffect("CalculateDamage");
        this.stats.Container.ExecuteEffect("Damage", damage);
        this.stats.Container.ExecuteEffect("DamageOvertime", 2);
    }
```
## EntityStats

EntityStats is the core container of the StatAndEffects system.
It acts as a central place where all Stats and Effect Pipelines are stored, managed, and executed.

You can think of it as a template + runtime container for an entity (player, enemy, NPC).

### ⚙️ Responsibilities


📊 Storing all stats (PrimaryStat, VitalStat, LevelStat, etc.)

⚡ Managing effect pipelines (damage, healing, buffs, etc.)

🔍 Fast lookup of stats by:
ID
Name
Type

🚀 Executing effects using pipelines

🏗️ Structure
```csharp
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
```
EntityStats → MonoBehaviour (attach to GameObject)

EntityStatsContainer → Actual data storage & logic

### 📦 EntityStatsContainer

This is where all the data lives:

```csharp
[SerializeReference]
public List<AbstractStat> StatsList = new List<AbstractStat>();

Internally it maintains:

Dictionary<int, AbstractStat> → Fast ID lookup
Dictionary<string, AbstractStat> → Name lookup
EntityStats stats = GetComponent<EntityStats>();
```

## 🧩 StatDefinition

StatDefinition is a ScriptableObject that represents the identity and metadata of a stat within the system.

It acts as a central reference point used by stats, modifiers, and effects to ensure consistency and avoid hardcoded values.

Provide a unique identifier (Id) for each stat  
Store display and UI-related data  
Serve as a lookup key for retrieving stats from EntityStats  
Separate data (definition) from logic (stat behavior)  
```csharp
using Unity.Properties;
using UnityEngine;

namespace StatAndEffects.Stat
{
    [CreateAssetMenu(menuName = "StatAndEffects/Stat Definition")]
    public class StatDefinition : ScriptableObject
    {
        public int Id;
        
        [SerializeField, DontCreateProperty]
        private string displayName;

        [SerializeField, DontCreateProperty]
        private string shortName; 

        [SerializeField, DontCreateProperty]
        private string description;

        [SerializeField, DontCreateProperty]
        private Sprite icon;

        [CreateProperty]
        public string DisplayName => displayName;
        [CreateProperty]
        public string ShortName => shortName;
        [CreateProperty]
        public string Description => description;
        [CreateProperty]
        public Sprite Icon => icon;

        public static implicit operator string(StatDefinition stat)
        {
            return $"{stat.DisplayName} ({stat.ShortName})";
        }
    }

}
```

#### Important
Change assetLocation in  StatAbilitiesManager to Folder in which you are creating/created StatDefinition.
## StatLayer


The system uses a **layer-based modifier architecture** to organize how different sources affect a stat.  
Each modifier belongs to a specific `StatLayer`, allowing clear separation of logic and predictable evaluation.

```csharp
public enum StatLayer
{
    Base,
    Gear,
    Buff,
    Skill,
    Meta
}
```

Adding a new modifier layer is simple:

Just add a new value to the StatLayer enum in StatAndEffect/Stat/StatLayer.cs folder.

### Adding Defalt layer

juat Add new layer and ModifierCollection in Dictionary.
```csharp

public static Dictionary<StatLayer, ModifierCollection> CreateDefaultLayers(int capacity)
{
    Dictionary<StatLayer, ModifierCollection> layers = new ();
    layers.Add(StatLayer.Base, new ModifierCollection(capacity));
    layers.Add(StatLayer.Gear, new ModifierCollection(capacity));
    return layers;
}
```
## Modifier
Modifiers define how a stat is affected.Each modifier consists of:

* Type → How the value is applied (Flat, Additive, Multiplicative)
* Value → The strength of the modifier
* Layer → The source of the modifier (Base, Gear, Buff, etc.)
```csharp
// Single modifier (e.g., from gear)
public StatModifier fromGearModifier = 
    new StatModifier(StatModifierType.Flat, 1, StatLayer.Gear);

// Multiple base modifiers
public List<StatModifier> baseModifiers = new List<StatModifier>()
{
    new StatModifier(StatModifierType.Flat, 10, StatLayer.Base),
    new StatModifier(StatModifierType.Additive, 20, StatLayer.Base),
    new StatModifier(StatModifierType.Multiplicative, 30, StatLayer.Base),
};       
```
### Modifier Operation
The system have three types of modifier operations implementation:

* Flat
* Additive 
* Multiplicative

These are implemented based on the design from the referenced repository:
👉 https://github.com/meredoth/Stat-System

The behavior and application order follow a similar approach.
#### Operation Details
* Flat :
Adds a fixed value directly to the base stat.
Example: +10
* Additive
Adds a percentage based on the base value (before final calculation).
Example: +20% of base
* Multiplicative
Multiplies the final value after other modifiers are applied.
Example: ×1.3

### Add New Operation 
First, we create the ModifierOperationsBaseAbsoluteReduction class that inherits from the ModifierOperationsBase class, and we implement the CalculateModifiersValue method that is appropriate for our new type:

```csharp
public class ModifierOperationsBaseAbsoluteReduction : ModifierOperationsBase
{
   public ModifierOperationsBaseAbsoluteReduction(int capacity) : base(capacity) { }
   public ModifierOperationsBaseAbsoluteReduction() { }

   public override float CalculateModifiersValue(float baseValue, float currentValue)
   {
      var biggestModifier = 0f;

      for (var i = 0; i < Modifiers.Count; i++)
         biggestModifier = Mathf.Max(biggestModifier, Modifiers[i]);

      var modifierValue = biggestModifier == 0f ? 0f : baseValue * (1 - biggestModifier) - currentValue;

      return modifierValue;
   }
}
```

then Add StatModifierType AbsoluteRedution in StatAndEffects/Modifiers/StatModifierType.cs

```csharp
namespace StatAndEffects.Modifiers
{
    public enum StatModifierType 
    {
        Flat = 100,
        Additive = 200,
        Multiplicative = 300,
        AbsoluteRedution = 400
    }
}

```

Then At last Just Add ModifierOperationsBaseAbsoluteReduction in CreateModifierCollection (StatAndEffects/Modifiers/Registry.cs) switch statement
```csharp 
public static ModifierOperationBase CreateModifierCollection(StatModifierType statModifierType, int capacity)
{
    return statModifierType switch
    {
        StatModifierType.Flat => new FlatModifierOperation(capacity),
        StatModifierType.Additive => new AdditiveModifierOperation(capacity),
        StatModifierType.Multiplicative => new MultiplicativeModifierOperation(capacity),
        StatModifierType.AbsoluteRedution => new ModifierOperationsBaseAbsoluteReduction(capacity);
        _ => throw new ArgumentOutOfRangeException()
    };
}

```
If we want As Default Creation of ModifierOperationsBaseAbsoluteReduction then also add here in CreateDefaultOperations Method
```csharp
public static Dictionary<StatModifierType, ModifierOperationBase> CreateDefaultOperations(int capacity)
{
    Dictionary<StatModifierType,ModifierOperationBase> operations = new ();
    operations.Add(StatModifierType.Flat, new FlatModifierOperation(capacity));
    operations.Add(StatModifierType.Multiplicative, new MultiplicativeModifierOperation(capacity));
    operations.Add(StatModifierType.Additive, new AdditiveModifierOperation(capacity));
    
    return operations;
}

```
## Effects

The Effect System is responsible for applying gameplay logic such as damage, healing, buffs, and calculations using a modular and extensible pipeline-based architecture.

Effects are small, reusable processing units that modify a value step-by-step.

Reads required stats from an entity 

Processes an input value.

Returns the modified result

#### Base Effect

All effects inherit from:
```csharp
public abstract class Effect
{
    public abstract void Initialize(IEntityStats stats);
    public abstract float Process(float value);
}
```
* Initialize → Cache required stats
* Process → Apply logic to the input value

Current Implemented Effect

* 🗡️ AttackEffect :
Multiplies input value using attack stat.

* 🛡️ DefenseEffect : 
Reduces incoming value using defense formula.

* 💥 CriticalEffect :
Applies critical damage based on chance.

* ❤️ ApplyVitalStat : 
Applies value directly to a vital stat (e.g., health).

#### Adding New CalculateDamage Effect 
 which Calculate Damage 

 ```csharp
[Serializable]
public class CalculateDamage : Effect
{
    // StatDefinition is Required For If we are creating pipeline in Editor
    public StatDefinition attackDefinition;
    public StatDefinition criticalChanceDefinition;
    public StatDefinition criticalDamageDefinition;

    private PrimaryStat attack;
    private PrimaryStat criticalChance;
    private PrimaryStat criticalDamage;

    //When we want to create it form script then Assign 
    public CalculateDamage(
        PrimaryStat attack,
        PrimaryStat critChance,
        PrimaryStat critDamage)
    {
        this.attack = attack;
        this.criticalChance = critChance;
        this.criticalDamage = critDamage;
    }

    public override void Initialize(IEntityStats stats)
    {
        this.attack = stats.GetStat<PrimaryStat>(this.attackDefinition);
        this.criticalChance = stats.GetStat<PrimaryStat>(this.criticalChanceDefinition);
        this.criticalDamage = stats.GetStat<PrimaryStat>(this.criticalDamageDefinition);
    }

    public override float Process(float value)
    {
        float result = value * attack.Value;
        
        if (UnityEngine.Random.Range(0f, 100f) <= criticalChance.Value)
        {
            result *= criticalDamage.Value;
        }

        return result;
    }
}
```
### Effect Pipeline

Pipelines control how effects are executed.

Base Pipeline
public abstract class EffectPipeline

Features:

Holds a list of effects
Initializes them with entity stats
Processes values sequentially

* ⚡ InstantEffectPipeline
Executes effects immediately
Stores result

* ⏳ EffectOverTimePipeline
Executes effects over time
Uses tick system
Supports:
Tick interval
Max ticks

#### Usage
    1. Create A Scriptable Object "Effect Object" from Create>StatsAndEffect>EffectObject
    2. Then Create Effect Chain in Obejct 
    3. Add a EntityStats to Object
    4. Assign Object To EffectPipeline

```csharp 
    public EntityStats stats;
    
    void Start()
    {
        stats = GetComponent<EntityStats>();    
        float damage = this.stats.Container.ExecuteEffect("CalculateDamage");
        this.stats.Container.ExecuteEffect("Damage", damage);
        this.stats.Container.ExecuteEffect("DamageOvertime", 2);
    }

```
* IMPORTANT

Use EntityStats For Effect Aa Effect is still in development need some improvement
