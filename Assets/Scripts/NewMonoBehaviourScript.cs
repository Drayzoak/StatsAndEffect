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
    
    
    public StatDefinition defenceDefinition;
    
    private void Awake()
    {
        if(!this.stats.Container.TryGetEffect("CalculateDamage", out this.calculateDamage))
            Debug.LogError(this.calculateDamage + " is not a valid effect.");
        
        if(!this.stats.Container.TryGetEffect("Damage", out this.damage))
            Debug.LogError(this.damage + " is not a valid effect.");
    }

    private void Start()
    {
        var health = this.stats.GetStat<VitalStat>("Health");
        var attack = this.stats.GetStat<PrimaryStat>(2);
        var defence = this.stats.GetStat<PrimaryStat>(this.defenceDefinition);
        
        var luck = new PrimaryStat(9,"Luck");
        this.stats.AddStat(luck);
        this.stats.RemoveStat(luck);
        

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