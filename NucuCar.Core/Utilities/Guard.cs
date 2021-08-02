using System;

namespace NucuCar.Core.Utilities
{
    /// <summary>  
    ///  Helper class used for checking arguments and raise exception if the checks don't pass.
    /// </summary> 
    public static class Guard
    {
        /// <summary>
        /// Checks if the argument string is null or whitespace and raises exception on check fail.
        /// </summary>
        /// <param name="argumentName">The argument name that will be logged in the exception message.</param>
        /// <param name="argument">The argument to check if it's null or whitespace.</param>
        /// <exception cref="ArgumentNullException">Raised if the argument is null or whitespace.</exception>
        public static void ArgumentNotNullOrWhiteSpace(string argumentName, string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new ArgumentNullException($"The argument {argumentName} is null or whitespace!");
            }
        }

        /// <summary>
        /// Checks if the argument is null and raises exception on check fail.
        /// </summary>
        /// <param name="argumentName">The argument name that will be logged in the exception message.</param>
        /// <param name="argument">The argument to check if it's null.</param>
        /// <exception cref="ArgumentNullException">Raised if the argument is null.</exception>
        public static void ArgumentNotNull(string argumentName, object argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException($"The argument {argumentName} is null or whitespace!");
            }
        }
    }
}