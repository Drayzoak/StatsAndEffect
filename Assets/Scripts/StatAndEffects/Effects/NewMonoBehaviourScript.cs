using StatAndEffects;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public EntityStats stats;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats = GetComponent<EntityStats>();    
        float damage = this.stats.Container.ExecuteEffect("CalculateDamage");
        this.stats.Container.ExecuteEffect("Damage", damage);
        this.stats.Container.ExecuteEffect("DamageOvertime", 2);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
