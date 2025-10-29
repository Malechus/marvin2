using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Tools;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    /// <summary>
    /// Represents a daily chore assignment for a person.
    /// Inherits common assignment properties from <see cref="PersonChore"/> and
    /// adds a priority level that can be used to order or filter daily tasks.
    /// </summary>
    public class DailyChore : PersonChore
    {
        /// <summary>
        /// Priority levels applicable to daily chores.
        /// </summary>
        public enum Priority 
        { 
            /// <summary>Low priority — task can be deferred if needed.</summary>
            Low, 
            /// <summary>Medium priority — normal importance.</summary>
            Medium, 
            /// <summary>High priority — should be completed as soon as possible.</summary>
            High 
        }

        /// <summary>
        /// The priority assigned to this daily chore.
        /// </summary>
        public Priority priority { get; set; }
    }
}