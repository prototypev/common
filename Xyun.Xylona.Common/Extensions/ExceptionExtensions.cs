namespace Xyun.Xylona.Common.Extensions
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    ///     Extensions to System.Exception.
    /// </summary>
    public static class ExceptionExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the inner most exception.
        /// </summary>
        /// <param name="ex">
        /// The original exception.
        /// </param>
        /// <returns>
        /// The inner most exception.
        /// </returns>
        public static Exception GetInnermostException(this Exception ex)
        {
            Contract.Requires(ex != null);

            return ex.InnerException == null
                       ? ex
                       : ex.InnerException.GetInnermostException();
        }

        #endregion
    }
}