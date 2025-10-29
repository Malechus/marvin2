using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using marvin2.Models;

namespace marvin2.Factories
{
    /// <summary>
    /// Design-time factory for creating <see cref="ChoreContext"/> instances.
    /// Used by EF Core tools (for example, migrations) when the application's
    /// DbContext cannot be constructed from the application's service provider.
    /// </summary>
    public class ChoreContextFactory : IDesignTimeDbContextFactory<ChoreContext>
    {
        /// <summary>
        /// Creates a new <see cref="ChoreContext"/> configured using an environment-specific
        /// appsettings JSON file. The first element of <paramref name="args"/> is expected
        /// to be the environment name (e.g. "Development" or "Production") which will be
        /// appended to "appsettings.{env}.json".
        /// </summary>
        /// <param name="args">
        /// Command-line arguments forwarded by the EF tools. The factory expects
        /// args[0] to contain the environment name used to locate the appropriate
        /// settings file.
        /// </param>
        /// <returns>
        /// A configured <see cref="ChoreContext"/> instance ready for design-time operations.
        /// </returns>
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