namespace Xyun.Xylona.Common.Extensions
{
    using System.Text.RegularExpressions;

    /// <summary>
    ///     Extensions to System.String.
    /// </summary>
    public static class StringExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Splits a string according to camel casing.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The new string split by camel casing.
        /// </returns>
        public static string SplitCamelCase(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                       ? input
                       : Regex.Replace(input, "(?<!(^|[A-Z]))(?=[A-Z])|(?<!^)(?=[A-Z][a-z])", " $1", RegexOptions.Compiled)
                              .Trim();
        }

        #endregion
    }
}