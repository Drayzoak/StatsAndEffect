namespace StatAndEffects.Stat
{
    /// <summary>
    /// Defines the different layers used in the stat modifier system.
    /// Each layer represents a source of modifiers that contribute
    /// to the final calculated stat value.
    ///
    /// The order and combination of these layers are handled internally
    /// by the LayeredModifierCollection.
    /// </summary>
    public enum StatLayer
    {
        Base,   // Core stat value (default/base stats)
        Gear,   // Equipment-based modifiers
        Buff,   // Temporary effects (potions, status effects)
        Skill,  // Skill or ability-based modifiers
        Meta    // Global or special modifiers (difficulty, perks, etc.)
    }
}