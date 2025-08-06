using System;
using marvin2.Models;

namespace marvin2.Services
{
    public interface IChoreService
    {
        public List<DailyChore> GetDailyChores();

        public List<WeeklyChore> GetWeeklyChores(string dayOfWeek);

        public List<MonthlyChore> GetMonthlyChores(int dayOfMonth);

        public List<Person> GetPeople();
    }
}