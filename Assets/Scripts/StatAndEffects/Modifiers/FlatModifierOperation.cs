namespace StatAndEffects.Modifiers
{
    public sealed class FlatModifierOperation : ModifierOperationBase
    {
        internal FlatModifierOperation(int capacity) : base(capacity) { }

        public override float CalculateModifier(float baseValue, float modifiedValue)
        {
            float flatModifiersValue  = 0f;

            foreach (var modifier in StatModifiers)
            {
                flatModifiersValue  += modifier;
            }

            return flatModifiersValue ;
        }
    }

}