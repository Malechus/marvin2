using Microsoft.EntityFrameworkCore;
using marvin2.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using MySqlConnector;

namespace marvin2.Models
{
    public class ChoreContext : DbContext
    {
        private IConfigurationRoot _config;
        
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
        
        public DbSet<WeeklyChore> WeeklyChores { get; set; }
        public DbSet<DailyChore> DailyChores { get; set; }
        public DbSet<MonthlyChore> MonthlyChores { get; set; }
        public DbSet<Person> People { get; set; }


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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeeklyChore>().HasBaseType<Chore>();
            
            modelBuilder.Entity<DailyChore>().HasBaseType<Chore>();
            
            modelBuilder.Entity<MonthlyChore>().HasBaseType<Chore>();

            modelBuilder.Entity<Chore>().UseTphMappingStrategy();
        }
    }
}