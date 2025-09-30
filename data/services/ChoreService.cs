

using System.Data.Common;
using marvin2.Factories;
using marvin2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace marvin2.Services
{
    public class ChoreService
    {
        private readonly ChoreContext _context;
        private readonly IConfigurationRoot _config;
        
        public ChoreService(IConfigurationRoot configurationRoot)
        {
            _config = configurationRoot;
            DbContextOptionsBuilder<ChoreContext> builder = new DbContextOptionsBuilder<ChoreContext>();
            builder.UseMySql(_config["Database:ConnectionString"], ServerVersion.AutoDetect(_config["Database:ConnectionString"]));
            _context = new ChoreContext(builder.Options);
        }
        
        public List<DailyChore> GetDailyChores()
        {
            return getDailyChores();
        }
        
        private List<DailyChore> getDailyChores()
        {
            List<DailyChore> dailyChores = _context.DailyChores
                .Where(dc => dc.IsActive == true)
                .ToList();

            return dailyChores;
        }
        
        public List<WeeklyChore> GetWeeklyChores(string dayOfWeek)
        {
            return getWeeklyChores(dayOfWeek);
        }
        
        private List<WeeklyChore> getWeeklyChores(string dayOfWeek)
        {
            List<WeeklyChore> weeklyChores = _context.WeeklyChores
                .Where(c => c.DayOfWeek.ToLower() == dayOfWeek.ToLower())
                .ToList();

            return weeklyChores;
        }
        
        public List<MonthlyChore>? GetMonthlyChores(int dayOfMonth)
        {
            return getMonthlyChores(dayOfMonth);
        }
        
        private List<MonthlyChore>? getMonthlyChores(int dayOfMonth)
        {
            List<MonthlyChore>? monthlyChores = _context.MonthlyChores
                .Where(mc => mc.DayOfMonth == dayOfMonth)
                .ToList();

            return monthlyChores;
        }
        
        public List<Person> GetPeople()
        {
            return getPeople();
        }
        
        private List<Person> getPeople()
        {
            List<Person> people = _context.People.ToList();
            return people;
        }
    }
}