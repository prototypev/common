namespace Xyun.Xylona.Common.Tests.Extensions
{
    using System;

    using NUnit.Framework;

    using Xyun.Xylona.Common.Extensions;

    /// <summary>
    ///     Tests for the DateTime extensions.
    /// </summary>
    public abstract class DateTimeExtensionsTests
    {
        /// <summary>
        ///     Tests the NumberOfAnniversariesAsAt() extension method.
        /// </summary>
        [TestFixture]
        public class NumberOfAnniversariesAsAtTests : DateTimeExtensionsTests
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Tests with valid DateTimes.
            /// </summary>
            [Test]
            public void TestWithValidDateTimes()
            {
                Assert.AreEqual(1, new DateTime(2012, 1, 1).NumberOfAnniversariesAsAt(new DateTime(2013, 11, 20)));
                Assert.AreEqual(0, new DateTime(2012, 12, 1).NumberOfAnniversariesAsAt(new DateTime(2013, 11, 20)));
                Assert.AreEqual(0, DateTime.Today.NumberOfAnniversariesAsAt(DateTime.Today));
                Assert.AreEqual(0, new DateTime(2013, 12, 1).NumberOfAnniversariesAsAt(new DateTime(2013, 11, 20)));
                Assert.AreEqual(0, new DateTime(2014, 12, 1).NumberOfAnniversariesAsAt(new DateTime(2013, 11, 20)));
            }

            #endregion
        }
    }
}