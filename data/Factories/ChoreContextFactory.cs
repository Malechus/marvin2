using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using marvin2.Models;

namespace marvin2.Factories
{
    public class ChoreContextFactory : IDesignTimeDbContextFactory<ChoreContext>
    {
        public ChoreContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.{args[0]}.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = config["Database:ConnectionString"];

            var optionsBuilder = new DbContextOptionsBuilder<ChoreContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new ChoreContext(optionsBuilder.Options);
        }
    }
}