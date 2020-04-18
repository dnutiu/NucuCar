using System;
using System.Collections.Generic;

namespace NucuCar.Common.Utilities
{
    /// <summary>
    /// ConnectionStringParser is an utility service to parse and validate connection strings.
    /// </summary>
    public static class ConnectionStringParser
    {
        /// <summary>
        /// Parse parses and validates a provided connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to parse</param>
        /// <returns>A dictionary object containing the values of the connection string.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dictionary<string, string> Parse(string connectionString)
        {
            if (connectionString.Equals(string.Empty))
            {
                throw new ArgumentException("ConnectionString can't be empty!");
            }
            
            var items = connectionString.Split(";");
            var parsedConnectionString = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var keyValue = item.Split("=", 2);
                if (keyValue.Length < 2)
                {
                    throw new ArgumentException(
                        $"Invalid argument for connection string, expected KEY=VALUE got {item}");
                }

                parsedConnectionString.Add(keyValue[0], keyValue[1]);
            }

            return parsedConnectionString;
        }
    }
}