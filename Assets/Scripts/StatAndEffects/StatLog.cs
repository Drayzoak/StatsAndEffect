using GameLogging;
using StatAndEffects.Modifiers;
using StatAndEffects.Stat;

namespace StatAndEffects
{
    public static class StatLog
    {
        private const string STAT_CATEGORY = "Stat";
        private const string MODIFIER_CATEGORY = "Modifier";
        private static string GetStatInfo(AbstractStat stat)
        {
            return $"{stat.GetStatName()} ({stat.GetType().Name})";
        }

        public static void ModifierAdded(AbstractStat stat, StatModifier modifier)
        {
            GameLog.Info(
                MODIFIER_CATEGORY,
                $"{GetStatInfo(stat)} Added {modifier.Type} ({modifier.Value})"
            );
        }

        public static void ModifierRemoved(AbstractStat stat, StatModifier modifier)
        {
            GameLog.Info(
                MODIFIER_CATEGORY,
                $"{GetStatInfo(stat)} Removed {modifier.Type} ({modifier.Value})"
            );
        }

        public static void ModifiersCleared(AbstractStat stat)
        {
            GameLog.Info(
                MODIFIER_CATEGORY,
                $"{GetStatInfo(stat)} Cleared all modifiers"
            );
        }

        public static void ModifierCapacityReached(AbstractStat stat)
        {
            GameLog.Warn(
                MODIFIER_CATEGORY,
                $"{GetStatInfo(stat)} modifier capacity reached"
            );
        }

        public static void ModifierAddFailed(AbstractStat stat, StatModifier modifier)
        {
            GameLog.Warn(
                MODIFIER_CATEGORY,
                $"{GetStatInfo(stat)} failed to add {modifier.Type} ({modifier.Value})"
            );
        }

        public static void InvalidModifierType(AbstractStat stat, StatModifierType type)
        {
            GameLog.Error(
                STAT_CATEGORY,
                $"{GetStatInfo(stat)} invalid modifier type ({type})"
            );
        }

        public static void OperationMissing(AbstractStat stat, StatModifierType type)
        {
            GameLog.Error(
                STAT_CATEGORY,
                $"{GetStatInfo(stat)} missing operation for modifier type ({type})"
            );
        }
        

        public static void LogStatInfo(string msg)
        {
            GameLog.Info(STAT_CATEGORY,msg);
        }

        public static void LogStatWarning(string msg)
        {
            GameLog.Warn(STAT_CATEGORY, msg);
        }

        public static void LogStatError(string msg)
        {
            GameLog.Error(STAT_CATEGORY, msg);
        }
    }
}