namespace Xyun.Xylona.Common.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    ///     Extensions to System.Collections.List.
    /// </summary>
    public static class ListExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Determines whether the specified list is equal to the current list.
        /// </summary>
        /// <param name="source">
        /// The source list.
        /// </param>
        /// <param name="compareTo">
        /// The list to compare.
        /// </param>
        /// <returns>
        /// <c>true</c> if the lists are equal; otherwise <c>false</c>.
        /// </returns>
        public static bool ListEquals(this IList source, IList compareTo)
        {
            Contract.Requires(source != null);
            Contract.Requires(compareTo != null);

            if (source.Count != compareTo.Count)
            {
                return false;
            }

            for (int i = 0; i < source.Count; i++)
            {
                if (!Equals(source[i], compareTo[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes items from a list where the condition matches the provided predicate.
        /// </summary>
        /// <typeparam name="TItem">
        /// The type of the item.
        /// </typeparam>
        /// <param name="list">
        /// The list.
        /// </param>
        /// <param name="predicate">
        /// The predicate.
        /// </param>
        public static void RemoveWhere<TItem>(this IList<TItem> list, Func<TItem, bool> predicate)
        {
            Contract.Requires(list != null);
            Contract.Requires(predicate != null);

            TItem[] itemsToRemove = list.Where(predicate)
                                        .ToArray();
            foreach (TItem item in itemsToRemove)
            {
                list.Remove(item);
            }
        }

        #endregion
    }
}