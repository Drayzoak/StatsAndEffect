using System;

namespace Common.Extensions
{
    public static class EnumExtension
    {
        public static int Length<T>() where T : Enum
        {
            return Enum.GetNames(typeof(T)).Length;
        }
    }
}