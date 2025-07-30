using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Tools;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace marvin2.Models
{
    public class MonthlyChore : Chore
    {
        public int? DayOfMonth { get; set; }
        public enum DayOfWeek {Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }
        public DayOfWeek? Day {get;set;}
        public enum WeekOfMonth { First, Second, Third, Fourth }
        public WeekOfMonth? Week { get; set; }
        
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