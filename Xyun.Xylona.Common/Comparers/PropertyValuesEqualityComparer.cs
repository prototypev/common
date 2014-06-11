using System.Collections.Generic;
using Xyun.Xylona.Common.Extensions;

namespace Xyun.Xylona.Common.Comparers
{
    /// <summary>
    ///     A property value equality comparer.
    /// </summary>
    /// <typeparam name="T">The object type.</typeparam>
    public class PropertyValuesEqualityComparer<T> : IEqualityComparer<T>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        ///     true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(T x, T y)
        {
            return x.PropertyValuesAreEqual(y);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}