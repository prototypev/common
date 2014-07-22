namespace Xyun.Xylona.Common.Tests.Utilities
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Xyun.Xylona.Common.Utilities;

    /// <summary>
    /// Tests for DeepCloner.
    /// </summary>
    [TestFixture]
    public class DeepClonerTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// Tests deep cloning an object with circular references.
        /// </summary>
        [Test]
        public void TestDeepCloningObjectWithCircularReferences()
        {
            TestClass parent = new TestClass
                                   {
                                       Name = "Parent"
                                   };
            parent.AddChild(new TestClass
                                {
                                    Name = "Child #1"
                                });
            parent.AddChild(new TestClass
                                {
                                    Name = "Child #2"
                                });

            TestClass clone = DeepCloner.DeepClone(parent);

            Assert.AreNotSame(parent, clone);
            Assert.AreEqual(parent.Name, clone.Name);
            Assert.IsNull(clone.Parent);
            Assert.AreNotSame(parent.Children, clone.Children);

            TestClass[] originalChildren = parent.Children.ToArray();
            TestClass[] cloneChildren = clone.Children.ToArray();
            Assert.AreEqual(originalChildren.Length, cloneChildren.Length);

            for (int i = 0; i < originalChildren.Length; i++)
            {
                Assert.AreNotSame(originalChildren[i], cloneChildren[i]);
                Assert.AreEqual(originalChildren[i].Name, cloneChildren[i].Name);
                Assert.AreSame(clone, cloneChildren[i].Parent);
            }
        }

        #endregion

        /// <summary>
        /// Test class for deep cloning.
        /// </summary>
        private class TestClass
        {
            #region Fields

            /// <summary>
            /// Backing field for Children.
            /// </summary>
            private readonly IList<TestClass> _children = new List<TestClass>();

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the children.
            /// </summary>
            /// <value>
            /// The children.
            /// </value>
            public IEnumerable<TestClass> Children
            {
                get
                {
                    return this._children;
                }
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public string Name { get; set; }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <value>
            /// The parent.
            /// </value>
            public TestClass Parent { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// Adds the child.
            /// </summary>
            /// <param name="child">
            /// The child.
            /// </param>
            public void AddChild(TestClass child)
            {
                child.Parent = this;
                this._children.Add(child);
            }

            #endregion
        }
    }
}