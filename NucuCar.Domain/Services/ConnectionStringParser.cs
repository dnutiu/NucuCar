using System;
using System.Collections.Generic;
using DotNetty.Common;

namespace NucuCar.Domain.Services
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
            // TODO: Write tests for this.
            var items = connectionString.Split(";");
            var parsedConnectionString = new Dictionary<string, string>();
            foreach (var item in items)
            {
                var keyValue = item.Split("=");
                if (keyValue.Length != 2)
                {
                    throw new ArgumentException(
                        $"Invalid argument for connection string, expected KEY=VALUE got {item}");
                }

                parsedConnectionString[keyValue[0]] = parsedConnectionString[keyValue[1]];
            }

            return parsedConnectionString;
        }
    }
}