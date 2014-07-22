namespace Xyun.Xylona.Common.Comparers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper from http://brendan.enrick.com/post/linq-your-collections-with-iequalitycomparer-and-lambda-expressions.aspx.
    /// </summary>
    /// <typeparam name="T">
    /// The type being compared.
    /// </typeparam>
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        #region Fields

        /// <summary>
        /// The lambda comparer.
        /// </summary>
        private readonly Func<T, T, bool> _lambdaComparer;

        /// <summary>
        /// The lambda hash.
        /// </summary>
        private readonly Func<T, int> _lambdaHash;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaComparer{T}"/> class.
        /// </summary>
        /// <param name="lambdaComparer">
        /// The lambda comparer.
        /// </param>
        public LambdaComparer(Func<T, T, bool> lambdaComparer)
            : this(lambdaComparer, o => 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaComparer{T}"/> class.
        /// </summary>
        /// <param name="lambdaComparer">
        /// The lambda comparer.
        /// </param>
        /// <param name="lambdaHash">
        /// The lambda hash.
        /// </param>
        public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
        {
            if (lambdaComparer == null)
            {
                throw new ArgumentNullException("lambdaComparer");
            }

            if (lambdaHash == null)
            {
                throw new ArgumentNullException("lambdaHash");
            }

            this._lambdaComparer = lambdaComparer;
            this._lambdaHash = lambdaHash;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(T x, T y)
        {
            return this._lambdaComparer(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public int GetHashCode(T obj)
        {
            return this._lambdaHash(obj);
        }

        #endregion
    }
}