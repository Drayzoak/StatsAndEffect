using System;
namespace StatAndEffects.Effects
{
    [Serializable]
    public class InstantEffectPipeline : EffectPipeline
    {
        public float result;
        public override void ExecuteEffect(float value = 1)
        {

            this.result = ProcessInternal(value);
        }
    }
}