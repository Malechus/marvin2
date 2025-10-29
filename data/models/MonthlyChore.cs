using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Tools;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    /// <summary>
    /// Represents a monthly chore assignment for a person.
    /// Inherits common assignment properties from <see cref="PersonChore"/>.
    /// A monthly chore can be scheduled either by a numeric day of month (1-31)
    /// or by a day-of-week + week-of-month combination (for example, the second Tuesday).
    /// </summary>
    public class MonthlyChore : PersonChore
    {
        /// <summary>
        /// Numeric day of the month the chore should occur (1..31).
        /// When set, <see cref="Day"/> and <see cref="Week"/> must be null.
        /// </summary>
        public int? DayOfMonth { get; set; }

        /// <summary>
        /// Day-of-week enumeration used when scheduling by weekday + week-of-month.
        /// </summary>
        public enum DayOfWeek {Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

        /// <summary>
        /// The day-of-week used for weekday-based monthly scheduling.
        /// When set, <see cref="Week"/> must also be set and <see cref="DayOfMonth"/> must be null.
        /// </summary>
        public DayOfWeek? Day {get;set;}

        /// <summary>
        /// Specifies which week of the month the chore occurs on (first, second, etc.).
        /// Used together with <see cref="Day"/> for weekday-based scheduling.
        /// </summary>
        public enum WeekOfMonth { First, Second, Third, Fourth }

        /// <summary>
        /// The week-of-month used for weekday-based monthly scheduling.
        /// </summary>
        public WeekOfMonth? Week { get; set; }
        
        /// <summary>
        /// Validates that the monthly chore is configured correctly.
        /// Returns true when either:
        /// - <see cref="DayOfMonth"/> is set to a value between 1 and 31 and <see cref="Day"/> and <see cref="Week"/> are null, or
        /// - <see cref="DayOfMonth"/> is null and both <see cref="Day"/> and <see cref="Week"/> are set.
        /// Otherwise returns false.
        /// </summary>
        /// <returns><c>true</c> if the configuration is valid; otherwise <c>false</c>.</returns>
        public bool IsValid()
        {
            if(DayOfMonth > 0 && DayOfMonth < 32 && Day is null && Week is null)
            {
                return true;
            }
            else if(DayOfMonth is null && Day is not null && Week is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}