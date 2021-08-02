using System;
using System.Collections.Generic;
using NucuCar.Core.Utilities;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Core
{
    public class ConnectionStringParserTest
    {
        [Fact]
        private void Test_ConnectionStringParser_Valid()
        {
            const string connectionString = "Test=1;Test2=2";
            var parsedString = ConnectionStringParser.Parse(connectionString);

            Assert.Equal("1", parsedString.GetValueOrDefault("Test"));
            Assert.Equal("2", parsedString.GetValueOrDefault("Test2"));
        }
        
        [Fact]
        private void Test_ConnectionStringParser_EmptyValue()
        {
            const string connectionString = "Test=1;Test2=";
            var parsedString = ConnectionStringParser.Parse(connectionString);

            Assert.Equal("1", parsedString.GetValueOrDefault("Test"));
            Assert.Equal(string.Empty, parsedString.GetValueOrDefault("Test2"));
        }
        
        [Fact]
        private void Test_ConnectionStringParser_EmptyValue2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ConnectionStringParser.Parse(string.Empty);
            });
        }
        
        [Fact]
        private void Test_ConnectionStringParser_Invalid()
        {
            const string connectionString = "Test=1;Test2=;d";
            Assert.Throws<ArgumentException>(() =>
            {
                ConnectionStringParser.Parse(connectionString);
            });
        }
        
        [Fact]
        private void Test_ConnectionStringParser_ValueWithMultipleEquals()
        {
            const string connectionString = "Test=1;Test2=base64=";
            var parsedString = ConnectionStringParser.Parse(connectionString);

            Assert.Equal("1", parsedString.GetValueOrDefault("Test"));
            Assert.Equal("base64=", parsedString.GetValueOrDefault("Test2"));
        }
        
        
    }
}