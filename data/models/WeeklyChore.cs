using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Tools;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    public class WeeklyChore : Chore
    {
        public string DayOfWeek{ get; set; }
    }
}