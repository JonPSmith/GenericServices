using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;

namespace Tests.Helpers
{
    internal static class ExtendAsserts
    {
        internal static void ShouldEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldStartWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.StartsWith(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldEndWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.EndsWith(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldContain(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.Contains(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldNotEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.True(expectedValue != actualValue, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this float actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this long actualValue, long expectedValue, int tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this double actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this int actualValue, int expectedValue, int tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqual<T>( this T actualValue, T expectedValue, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, IEnumerable<string> errorMessages)
        {
            Assert.AreEqual(expectedValue, actualValue,  string.Join("\n", errorMessages));
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, IEnumerable<ValidationResult> validationResults)
        {
            Assert.AreEqual(expectedValue, actualValue, string.Join("\n", validationResults.Select( x => x.ErrorMessage)));
        }

        internal static void ShouldNotEqual<T>(this T actualValue, T unexpectedValue, string errorMessage = null)
        {
            Assert.AreNotEqual(unexpectedValue, actualValue);
        }

        internal static void ShouldNotEqualNull<T>(this T actualValue, string errorMessage = null) where T : class
        {
            Assert.NotNull( actualValue);
        }
    }
}
