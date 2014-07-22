namespace Xyun.Xylona.Common.Tests.Extensions
{
    using System;

    using NUnit.Framework;

    using Xyun.Xylona.Common.Extensions;

    /// <summary>
    ///     Tests for the Exception extensions.
    /// </summary>
    public abstract class ExceptionExtensionsTests
    {
        /// <summary>
        ///     Tests the GetInnermostException() extension method.
        /// </summary>
        [TestFixture]
        public class GetInnermostExceptionTests : ExceptionExtensionsTests
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Tests with nested exceptions.
            /// </summary>
            [Test]
            public void TestNestedExceptions()
            {
                Exception ex1 = new Exception();
                Assert.AreEqual(ex1, ex1.GetInnermostException());

                Exception ex2 = new Exception(string.Empty, ex1);
                Assert.AreEqual(ex1, ex2.GetInnermostException());

                Exception ex3 = new Exception(string.Empty, ex2);
                Assert.AreEqual(ex1, ex3.GetInnermostException());
            }

            #endregion
        }
    }
}