namespace Xyun.Xylona.Common.Tests.Extensions
{
    using NUnit.Framework;

    using Xyun.Xylona.Common.Extensions;

    /// <summary>
    ///     Tests for the String extensions.
    /// </summary>
    public abstract class StringExtensionsTests
    {
        /// <summary>
        ///     Tests the SplitCamelCase() extension method.
        /// </summary>
        [TestFixture]
        public class SplitCamelCaseTests : StringExtensionsTests
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Tests with valid strings.
            /// </summary>
            [Test]
            public void TestWithValidStrings()
            {
                Assert.AreEqual("Foo Bar", "FooBar".SplitCamelCase());
                Assert.AreEqual("ALLCAPS", "ALLCAPS".SplitCamelCase());
                Assert.AreEqual("IP Address", "IPAddress".SplitCamelCase());
            }

            #endregion
        }
    }
}