using System;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Xyun.Xylona.Common.Extensions
{
    /// <summary>
    ///     Extensions to System.Type.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Gets the assembly qualified name of a type with only the full name and assembly name.
        ///     No version or public key token is included.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     The short assembly qualified name.
        /// </returns>
        public static string GetShortAssemblyQualifiedName(this Type type)
        {
            Contract.Requires(type != null);

            return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName()
                                                                .Name);
        }

        /// <summary>
        ///     Gets the type without nullability.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>If nullable, the underlying type; otherwise the type itself.</returns>
        public static Type GetTypeWithoutNullability(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsNullableType()
                       ? new NullableConverter(type).UnderlyingType
                       : type;
        }

        /// <summary>
        ///     Determines whether the specified type is an anonymous type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if the specified type is an anonymous type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAnonymousType(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsDefined(typeof (CompilerGeneratedAttribute), false) && type.FullName.Contains("AnonymousType");
        }

        /// <summary>
        ///     Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableType(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);
        }

        /// <summary>
        ///     Determines whether the type to check is a subclass of the specified raw generic type.
        ///     From: http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        /// </summary>
        /// <param name="toCheck">The type to check.</param>
        /// <param name="generic">The generic type.</param>
        /// <returns>
        ///     <c>true</c> if the type is a subclass; otherwise, <c>flase</c>.
        /// </returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof (object))
            {
                Type cur = toCheck.IsGenericType
                               ? toCheck.GetGenericTypeDefinition()
                               : toCheck;
                if (generic == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }
}