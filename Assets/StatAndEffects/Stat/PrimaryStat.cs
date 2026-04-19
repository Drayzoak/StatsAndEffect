using System;
using StatAndEffects.Builder;
namespace StatAndEffects.Stat
{
    [Serializable]
    public sealed class PrimaryStat : AbstractStat
    {
        public PrimaryStat(
            float baseValue, 
            string statDefinition = "" ,
            int digitAccuracy = DEFAULT_DIGIT_ACCURACY, 
            int modsMaxCapacity = DEFAULT_DIGIT_ACCURACY,
            LayerCreationContext layerCreationContext = null
        ) : base(baseValue, statDefinition, digitAccuracy, modsMaxCapacity, layerCreationContext)
        {
            
        }
        
        
        public PrimaryStat() : base()
        {
            
        }
    }
}