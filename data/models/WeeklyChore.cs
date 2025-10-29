using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Tools;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    /// <summary>
    /// Represents a weekly chore assignment for a person.
    /// Inherits common assignment properties from <see cref="PersonChore"/>.
    /// The <see cref="DayOfWeek"/> property indicates which weekday the chore occurs on.
    /// </summary>
    public class WeeklyChore : PersonChore
    {
        /// <summary>
        /// Name of the day of the week when the chore should be performed (e.g. "Monday").
        /// Expected values correspond to <see cref="System.DayOfWeek"/> names.
        /// </summary>
        public string DayOfWeek{ get; set; }
    }
}