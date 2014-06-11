using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Reflection;

namespace Xyun.Xylona.Common.Extensions
{
    /// <summary>
    ///     Extensions to System.Object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Determines whether the specified object's properties' values are equal to the current object's properties' values.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="compareTo">The object to compare.</param>
        /// <param name="flags">The binding flags used to determine which properties to compare. Defaults to public instance properties, with hierarchy flattened.</param>
        /// <returns>
        ///     <c>true</c> if the object's properties' values are equal; otherwise <c>false</c>.
        /// </returns>
        public static bool PropertyValuesAreEqual(this object source, object compareTo, BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public)
        {
            Contract.Requires(source != null);
            Contract.Requires(compareTo != null);

            PropertyInfo[] properties = source.GetType()
                                              .GetProperties(flags);

            foreach (PropertyInfo property in properties)
            {
                object sourceValue = property.GetValue(source, null);
                object compareToValue = property.GetValue(compareTo, null);

                IList sourceList = sourceValue as IList;
                IList actualList = compareToValue as IList;
                if (sourceList != null && actualList != null)
                {
                    if (!actualList.ListEquals(sourceList))
                    {
                        return false;
                    }
                }
                else if (!Equals(sourceValue, compareToValue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Merges 2 anonymous objects.
        /// </summary>
        /// <param name="obj1">The first object.</param>
        /// <param name="obj2">The second object.</param>
        /// <returns>The merged object.</returns>
        public static dynamic Merge(this object obj1, object obj2)
        {
            Contract.Requires(obj1 != null);
            Contract.Requires(obj2 != null);

            Type type1 = obj1.GetType();
            Contract.Requires(type1.IsAnonymousType(), "The first object is not an anonymous type.");

            Type type2 = obj2.GetType();
            Contract.Requires(type2.IsAnonymousType(), "The second object is not an anonymous type.");

            dynamic expando = new ExpandoObject();
            IDictionary<string, object> result = expando as IDictionary<string, object>;
            foreach (PropertyInfo propertyInfo in type1.GetProperties())
            {
                result[propertyInfo.Name] = propertyInfo.GetValue(obj1, null);
            }

            foreach (PropertyInfo propertyInfo in type2.GetProperties())
            {
                result[propertyInfo.Name] = propertyInfo.GetValue(obj2, null);
            }

            return result;
        }
    }
}