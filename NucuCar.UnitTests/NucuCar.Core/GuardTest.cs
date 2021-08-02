using System;
using NucuCar.Core.Utilities;
using Xunit;

namespace NucuCar.UnitTests.NucuCar.Core
{
    public class GuardTest
    {
        /// <summary>
        /// Ensure that an exception is thrown when the argument is null or whitespace,
        /// and no exception is raised when the argument is non-null.
        /// </summary>
        [Fact]
        private void Test_GuardArgumentNotNullOrWhitespace()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Guard.ArgumentNotNullOrWhiteSpace("null", null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                Guard.ArgumentNotNullOrWhiteSpace("whitespace", "");
            });
            Guard.ArgumentNotNullOrWhiteSpace("string", "string");
        }

        /// <summary>
        /// Ensure that an exception is thrown when argument is null. Otherwise no exception should be thrown.
        /// </summary>
        [Fact]
        private void Test_GuardArgumentNotNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Guard.ArgumentNotNull("null", null);
            });
            Guard.ArgumentNotNull("object", new string("asd"));
        }
    }
}