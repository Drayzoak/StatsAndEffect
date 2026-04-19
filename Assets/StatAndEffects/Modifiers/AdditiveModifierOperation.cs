namespace StatAndEffects.Modifiers
{
    public sealed class AdditiveModifierOperation : ModifierOperationBase
    {
        public AdditiveModifierOperation(int capacity) : base(capacity) { }

        public override float CalculateModifier(float baseValue, float modifiedValue)
        {
            float additiveModifierValue  = 0f;

            foreach (var modifier in StatModifiers)
            {
                additiveModifierValue += modifier;
            }

            return baseValue * additiveModifierValue;
        }
    }
}