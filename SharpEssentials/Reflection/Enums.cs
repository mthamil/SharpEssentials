using System;

namespace SharpEssentials.Reflection
{
    /// <summary>
    /// Provides utility methods for working with enums.
    /// </summary>
    public static class Enums
    {
        /// <summary>
        /// Attempts to convert the string representation of the name of an enumerated constant to its equivalent
        /// enumerated object.
        /// </summary>
        public static Option<TEnum> TryParse<TEnum>(string value) where TEnum : struct
        {
            TEnum parsed;
            if (Enum.TryParse(value, out parsed))
                return parsed;

            return Option<TEnum>.None();
        }
    }
}