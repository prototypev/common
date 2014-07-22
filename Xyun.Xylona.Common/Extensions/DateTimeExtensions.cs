namespace Xyun.Xylona.Common.Extensions
{
    using System;

    /// <summary>
    ///     Extensions to DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Calculates the number of anniversaries between two dates.
        /// </summary>
        /// <param name="anniversaryDate">
        /// The anniversary date.
        /// </param>
        /// <param name="asAtDate">
        /// As at date.
        /// </param>
        /// <returns>
        /// The number of anniversaries between two dates.
        /// </returns>
        public static int NumberOfAnniversariesAsAt(this DateTime anniversaryDate, DateTime asAtDate)
        {
            int years = asAtDate.Year - anniversaryDate.Year;

            if (asAtDate.Month < anniversaryDate.Month || (asAtDate.Month == anniversaryDate.Month && asAtDate.Day < anniversaryDate.Day))
            {
                // subtract one year if the anniversary has not yet happened in the As At year
                years--;
            }

            return Math.Max(0, years);
        }

        /// <summary>
        /// Shifts the specified date to the day of the week within the same week.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <param name="dayOfWeek">
        /// The day of week to shift to.
        /// </param>
        /// <returns>
        /// The shifted date.
        /// </returns>
        public static DateTime ShiftToDayOfWeek(this DateTime date, DayOfWeek dayOfWeek)
        {
            int diff = date.DayOfWeek - dayOfWeek;
            return date.AddDays(-1 * diff);
        }

        #endregion
    }
}