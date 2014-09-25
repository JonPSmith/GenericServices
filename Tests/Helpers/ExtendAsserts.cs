#region licence
// The MIT License (MIT)
// 
// Filename: ExtendAsserts.cs
// Date Created: 2014/05/19
// 
// Copyright (c) 2014 Jon Smith (www.selectiveanalytics.com & www.thereformedprogrammer.net)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion
using System;
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

        internal static void IsA<T>(this object actualValue, string errorMessage = null) where T : class
        {
            Assert.True(actualValue.GetType() == typeof(T), "expected type {0}, but was of type {1}", typeof(T).Name, actualValue.GetType().Name);
        }
    }
}
