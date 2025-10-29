using Microsoft.EntityFrameworkCore;
using marvin2.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MySqlConnector;

namespace marvin2.Models
{
    /// <summary>
    /// EF Core DbContext for chore-related entities.
    /// Encapsulates the database sets for chores, people, and the person-chore assignments,
    /// and configures the MySQL connection and the inheritance mapping strategy for the
    /// PersonChore hierarchy.
    /// </summary>
    public class ChoreContext : DbContext
    {
        private IConfigurationRoot _config;
        
        /// <summary>
        /// Constructs a new <see cref="ChoreContext"/> instance using the provided options.
        /// If configuration has not been loaded, this constructor will build a default
        /// configuration reading appsettings.json from the application's base directory.
        /// </summary>
        /// <param name="options">DbContext options forwarded to the base <see cref="DbContext"/>.</param>
        public ChoreContext(DbContextOptions<ChoreContext> options)
            : base(options)
        {
            if(_config == null)
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();
                    
            }
        }
        
        /// <summary>
        /// DbSet for weekly chore assignment entries.
        /// </summary>
        public DbSet<WeeklyChore> WeeklyChores { get; set; }
        /// <summary>
        /// DbSet for daily chore assignment entries.
        /// </summary>
        public DbSet<DailyChore> DailyChores { get; set; }
        /// <summary>
        /// DbSet for monthly chore assignment entries.
        /// </summary>
        public DbSet<MonthlyChore> MonthlyChores { get; set; }
        /// <summary>
        /// DbSet for people stored in the database.
        /// </summary>
        public DbSet<Person> People { get; set; }
        /// <summary>
        /// DbSet for person-chore associations (base type for the assignment hierarchy).
        /// </summary>
        public DbSet<PersonChore> PersonChores{ get; set; }


        /// <summary>
        /// Configures the DbContext to use a MySQL connection if the options builder
        /// has not already been configured. Connection details are read from the
        /// loaded configuration (<c>_config</c>).
        /// </summary>
        /// <param name="optionsBuilder">The options builder to configure.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
                {
                    Server = _config["Database:Server"],
                    Port = 3306,
                    UserID = _config["Database:UserID"],
                    Password = _config["Database:Password"],
                    Database = _config["Database:Database"]
                };

                optionsBuilder.UseMySql(builder.ConnectionString, ServerVersion.AutoDetect(builder.ConnectionString));
            }
        }

        /// <summary>
        /// Configures the EF Core model for the chore domain.
        /// Maps WeeklyChore, DailyChore, and MonthlyChore as derived types of
        /// <see cref="PersonChore"/> and instructs EF Core to use table-per-hierarchy (TPH)
        /// mapping for the assignment hierarchy.
        /// </summary>
        /// <param name="modelBuilder">Model builder used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeeklyChore>().HasBaseType<PersonChore>();
            
            modelBuilder.Entity<DailyChore>().HasBaseType<PersonChore>();
            
            modelBuilder.Entity<MonthlyChore>().HasBaseType<PersonChore>();

            modelBuilder.Entity<PersonChore>().UseTphMappingStrategy();
        }
    }
}