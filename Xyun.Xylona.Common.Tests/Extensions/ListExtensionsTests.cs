using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Xyun.Xylona.Common.Extensions;

namespace Xyun.Xylona.Common.Tests.Extensions
{
    /// <summary>
    ///     Tests for the List extensions.
    /// </summary>
    public abstract class ListExtensionsTests
    {
        /// <summary>
        ///     Tests the ListEquals() extension method.
        /// </summary>
        [TestFixture]
        public class ListEqualsTests : ListExtensionsTests
        {
            /// <summary>
            ///     Test with two equal lists.
            /// </summary>
            [Test]
            public void TestTwoEqualLists()
            {
                IList list1 = new List<string>
                    {
                        "a",
                        "b",
                        "c"
                    };
                IList list2 = new List<string>
                    {
                        "a",
                        "b",
                        "c"
                    };

                Assert.IsTrue(list1.ListEquals(list2));
            }

            /// <summary>
            ///     Tests with 2 unequal lists.
            /// </summary>
            [Test]
            public void TestTwoUnequalLists()
            {
                IList list1 = new List<string>
                    {
                        "a",
                        "b",
                        "c"
                    };
                IList list2 = new List<string>
                    {
                        "b",
                        "c"
                    };

                Assert.IsFalse(list1.ListEquals(list2));
            }
        }

        /// <summary>
        ///     Tests the RemoveWhere() extension method.
        /// </summary>
        [TestFixture]
        public class RemoveWhereTests : ListExtensionsTests
        {
            /// <summary>
            ///     Tests with an invalid predicate.
            /// </summary>
            [Test]
            public void TestWithInvalidPredicate()
            {
                IList<string> list = new List<string>
                    {
                        "a",
                        "b",
                        "c"
                    };
                list.RemoveWhere(s => s == "d");

                Assert.AreEqual(3, list.Count);
                Assert.AreEqual("a", list[0]);
                Assert.AreEqual("b", list[1]);
                Assert.AreEqual("c", list[2]);
            }

            /// <summary>
            ///     Tests with a valid predicate.
            /// </summary>
            [Test]
            public void TestWithValidPredicate()
            {
                IList<string> list = new List<string>
                    {
                        "a",
                        "b",
                        "c"
                    };
                list.RemoveWhere(s => s == "b");

                Assert.AreEqual(2, list.Count);
                Assert.AreEqual("a", list[0]);
                Assert.AreEqual("c", list[1]);
            }
        }
    }
}