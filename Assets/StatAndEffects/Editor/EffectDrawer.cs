using Common;
using StatAndEffects.Effects;
using UnityEditor;
namespace StatAndEffects.Editor
{
    [CustomPropertyDrawer(typeof(Effect), true)]
    public class EffectDrawer: ManagedReferenceDrawer
    {
        
    }
}