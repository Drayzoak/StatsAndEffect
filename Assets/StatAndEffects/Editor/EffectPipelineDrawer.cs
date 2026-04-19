using Common;
using StatAndEffects.Effects;
using UnityEditor;
namespace StatAndEffects.Editor
{
    [CustomPropertyDrawer(typeof(EffectPipeline), true)]
    public class EffectPipelineDrawer:ManagedReferenceDrawer
    {
        
    }
}