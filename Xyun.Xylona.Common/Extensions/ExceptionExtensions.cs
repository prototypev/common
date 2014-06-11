using System;
using System.Diagnostics.Contracts;

namespace Xyun.Xylona.Common.Extensions
{
    /// <summary>
    ///     Extensions to System.Exception.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Gets the inner most exception.
        /// </summary>
        /// <param name="ex">
        ///     The original exception.
        /// </param>
        /// <returns>
        ///     The inner most exception.
        /// </returns>
        public static Exception GetInnermostException(this Exception ex)
        {
            Contract.Requires(ex != null);

            return ex.InnerException == null
                       ? ex
                       : ex.InnerException.GetInnermostException();
        }
    }
}