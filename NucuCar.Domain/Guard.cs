using System;
using Microsoft.Extensions.Logging;

namespace NucuCar.Domain
{
    public static class Guard
    {
        internal static void ArgumentNotNullOrWhiteSpace(string argumentName, string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException($"The argument {argumentName} is null or whitespace!");
            }
        }

        public static void ArgumentNotNull(string argumentName, object argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException($"The argument {argumentName} is null or whitespace!");
            }
        }
    }
}