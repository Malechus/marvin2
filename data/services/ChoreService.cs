using System.Data.Common;
using marvin2.Factories;
using marvin2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace marvin2.Services
{
    /// <summary>
    /// Provides read-only access to chore-related data from the application's database.
    /// Encapsulates creation of a <see cref="ChoreContext"/> using configuration and
    /// exposes methods to retrieve daily, weekly, monthly chores, people, and person assignments.
    /// </summary>
    public class ChoreService
    {
        private readonly ChoreContext _context;
        private readonly IConfigurationRoot _config;
        
        /// <summary>
        /// Creates a new instance of <see cref="ChoreService"/> and configures the EF <see cref="ChoreContext"/>
        /// using the provided <see cref="IConfigurationRoot"/>. The connection string is read from
        /// <c>Database:ConnectionString</c> in configuration.
        /// </summary>
        /// <param name="configurationRoot">Configuration root used to locate database connection settings.</param>
        public ChoreService(IConfigurationRoot configurationRoot)
        {
            _config = configurationRoot;
            DbContextOptionsBuilder<ChoreContext> builder = new DbContextOptionsBuilder<ChoreContext>();
            builder.UseMySql(_config["Database:ConnectionString"], ServerVersion.AutoDetect(_config["Database:ConnectionString"]));
            _context = new ChoreContext(builder.Options);
        }
        
        /// <summary>
        /// Returns all configured daily chores from the database.
        /// </summary>
        /// <returns>List of <see cref="DailyChore"/> entries.</returns>
        public List<DailyChore> GetDailyChores()
        {
            return getDailyChores();
        }
        
        /// <summary>
        /// Internal implementation that queries the DailyChores DbSet.
        /// </summary>
        /// <returns>List of <see cref="DailyChore"/> entries.</returns>
        private List<DailyChore> getDailyChores()
        {
            List<DailyChore> dailyChores = _context.DailyChores
                .ToList();

            return dailyChores;
        }
        
        /// <summary>
        /// Returns weekly chores that occur on the specified weekday.
        /// </summary>
        /// <param name="dayOfWeek">Name of the weekday to filter by (case-insensitive).</param>
        /// <returns>List of <see cref="WeeklyChore"/> entries occurring on the given day.</returns>
        public List<WeeklyChore> GetWeeklyChores(string dayOfWeek)
        {
            return getWeeklyChores(dayOfWeek);
        }
        
        /// <summary>
        /// Internal implementation that queries WeeklyChores filtered by weekday.
        /// </summary>
        /// <param name="dayOfWeek">Name of the weekday to filter by (case-insensitive).</param>
        /// <returns>List of <see cref="WeeklyChore"/> entries.</returns>
        private List<WeeklyChore> getWeeklyChores(string dayOfWeek)
        {
            List<WeeklyChore> weeklyChores = _context.WeeklyChores
                .Where(c => c.DayOfWeek.ToLower() == dayOfWeek.ToLower())
                .ToList();

            return weeklyChores;
        }
        
        /// <summary>
        /// Returns monthly chores that occur on the specified day of the month.
        /// </summary>
        /// <param name="dayOfMonth">Numeric day of the month to filter by (1..31).</param>
        /// <returns>List of <see cref="MonthlyChore"/> entries or null if none.</returns>
        public List<MonthlyChore>? GetMonthlyChores(int dayOfMonth)
        {
            return getMonthlyChores(dayOfMonth);
        }
        
        /// <summary>
        /// Internal implementation that queries MonthlyChores filtered by day-of-month.
        /// </summary>
        /// <param name="dayOfMonth">Numeric day of the month to filter by (1..31).</param>
        /// <returns>List of <see cref="MonthlyChore"/> entries or null if none.</returns>
        private List<MonthlyChore>? getMonthlyChores(int dayOfMonth)
        {
            List<MonthlyChore>? monthlyChores = _context.MonthlyChores
                .Where(mc => mc.DayOfMonth == dayOfMonth)
                .ToList();

            return monthlyChores;
        }
        
        /// <summary>
        /// Returns all people stored in the database.
        /// </summary>
        /// <returns>List of <see cref="Person"/> entries.</returns>
        public List<Person> GetPeople()
        {
            return getPeople();
        }
        
        /// <summary>
        /// Internal implementation that queries the People DbSet.
        /// </summary>
        /// <returns>List of <see cref="Person"/> entries.</returns>
        private List<Person> getPeople()
        {
            List<Person> people = _context.People.ToList();
            return people;
        }
        
        /// <summary>
        /// Finds a person by name (case-insensitive).
        /// </summary>
        /// <param name="name">Display name of the person to find.</param>
        /// <returns>The matching <see cref="Person"/> or <c>null</c> if not found.</returns>
        public Person? GetPerson(string name)
        {
            return getPerson(name);
        }
        
        /// <summary>
        /// Internal implementation that queries People filtered by name.
        /// </summary>
        /// <param name="name">Display name of the person to find.</param>
        /// <returns>The matching <see cref="Person"/> or <c>null</c> if not found.</returns>
        private Person? getPerson(string name)
        {
            Person? person = _context.People
                .Where(p => p.Name.ToLower() == name.ToLower())
                .FirstOrDefault();

            return person;
        }
        
        /// <summary>
        /// Returns the person-chore associations for the given person, including related navigation properties.
        /// </summary>
        /// <param name="person">The person whose assignments should be retrieved.</param>
        /// <returns>List of <see cref="PersonChore"/> entries with <see cref="Person"/> and <see cref="Chore"/> included.</returns>
        public List<PersonChore> GetPersonChores(Person person)
        {
            return getPersonChores(person);
        }
        
        /// <summary>
        /// Internal implementation that queries PersonChores for the provided person and eagerly loads related entities.
        /// </summary>
        /// <param name="person">The person whose assignments should be retrieved.</param>
        /// <returns>List of <see cref="PersonChore"/> entries with navigation properties populated.</returns>
        private List<PersonChore> getPersonChores(Person person)
        {
            List<PersonChore> list = _context.PersonChores
                .Include(pc => pc.Chore)
                .Include(pc => pc.Person)
                .Where(c => c.Person.Id == person.Id)
                .ToList();

            return list;
        }
    }
}