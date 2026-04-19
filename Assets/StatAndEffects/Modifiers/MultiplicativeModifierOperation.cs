namespace StatAndEffects.Modifiers
{
    public sealed class MultiplicativeModifierOperation : ModifierOperationBase
    {
        public MultiplicativeModifierOperation(int capacity) : base(capacity) { }

        public override float CalculateModifier(float baseValue, float modifiedValue)
        {
            float multiplicativeModifierValue = modifiedValue;

            foreach (var modifier in StatModifiers)
            {
                multiplicativeModifierValue += multiplicativeModifierValue * modifier;
            }

            return multiplicativeModifierValue - modifiedValue;
        }
    }
}