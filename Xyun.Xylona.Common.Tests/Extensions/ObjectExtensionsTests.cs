namespace Xyun.Xylona.Common.Tests.Extensions
{
    using System.Collections.Generic;

    using NUnit.Framework;

    using Xyun.Xylona.Common.Extensions;

    /// <summary>
    ///     Tests for the Object extensions.
    /// </summary>
    public abstract class ObjectExtensionsTests
    {
        /// <summary>
        ///     Tests the Merge() extension method.
        /// </summary>
        [TestFixture]
        public class MergeTests : ObjectExtensionsTests
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Tests with valid objects.
            /// </summary>
            [Test]
            public void TestWithValidObjects()
            {
                var obj1 = new
                               {
                                   foo1 = "bar1"
                               };
                var obj2 = new
                               {
                                   foo2 = "bar2"
                               };

                dynamic merged = obj1.Merge(obj2);

                Assert.IsNotNull(merged);
                Assert.AreEqual("bar1", merged.foo1);
                Assert.AreEqual("bar2", merged.foo2);
            }

            #endregion
        }

        /// <summary>
        ///     Tests the PropertyValuesAreEqual() extension method.
        /// </summary>
        [TestFixture]
        public class PropertyValuesAreEqualTests : ObjectExtensionsTests
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Test with two equal objects.
            /// </summary>
            [Test]
            public void TestTwoEqualObjects()
            {
                object obj1 = new
                                  {
                                      p1 = "foo", 
                                      p2 = "bar", 
                                      p3 = new List<string>
                                               {
                                                   "abc", 
                                                   "def"
                                               }
                                  };
                object obj2 = new
                                  {
                                      p1 = "foo", 
                                      p2 = "bar", 
                                      p3 = new List<string>
                                               {
                                                   "abc", 
                                                   "def"
                                               }
                                  };

                Assert.IsTrue(obj1.PropertyValuesAreEqual(obj2));
            }

            /// <summary>
            ///     Tests with 2 unequal lists.
            /// </summary>
            [Test]
            public void TestTwoUnequalObjects()
            {
                object obj1 = new
                                  {
                                      p1 = "foo", 
                                      p2 = "bar", 
                                      p3 = new List<string>
                                               {
                                                   "abc", 
                                                   "def"
                                               }
                                  };
                object obj2 = new
                                  {
                                      p1 = "foo", 
                                      p2 = "bar", 
                                      p3 = new List<string>
                                               {
                                                   "abc"
                                               }
                                  };

                Assert.IsFalse(obj1.PropertyValuesAreEqual(obj2));
            }

            #endregion
        }
    }
}