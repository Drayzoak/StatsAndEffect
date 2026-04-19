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