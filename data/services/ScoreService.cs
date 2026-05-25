using marvin2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace marvin2.Services
{
    /// <summary>
    /// Provides functionality for managing and updating player scores in the database.
    /// Handles score creation for new players and incrementation for existing players.
    /// </summary>
    public class ScoreService
    {
        private readonly ChoreContext _context;
        private readonly IConfigurationRoot _config;

        /// <summary>
        /// Creates a new instance of <see cref="ScoreService"/> and configures the EF <see cref="ChoreContext"/>
        /// using the provided <see cref="IConfigurationRoot"/>. The connection string is read from
        /// <c>Database:ConnectionString</c> in configuration.
        /// </summary>
        /// <param name="configurationRoot">Configuration root used to locate database connection settings.</param>
        public ScoreService(IConfigurationRoot configurationRoot)
        {
            _config = configurationRoot;
            DbContextOptionsBuilder<ChoreContext> builder = new DbContextOptionsBuilder<ChoreContext>();
            builder.UseMySql(_config["Database:ConnectionString"], ServerVersion.AutoDetect(_config["Database:ConnectionString"]));
            _context = new ChoreContext(builder.Options);
        }

        /// <summary>
        /// Increments the score for a given person by 1.
        /// If no score record exists for the person, creates a new record with score 1.
        /// </summary>
        /// <param name="personId">The ID of the person whose score should be incremented.</param>
        /// <returns>The updated score value after incrementing.</returns>
        public int IncrementScore(int personId)
        {
            return incrementScore(personId);
        }

        /// <summary>
        /// Internal implementation that increments or creates a score record for a person.
        /// </summary>
        /// <param name="personId">The ID of the person whose score should be incremented.</param>
        /// <returns>The updated score value after incrementing.</returns>
        private int incrementScore(int personId)
        {
            var score = _context.PersonScores
                .FirstOrDefault(ps => ps.PersonId == personId);

            if (score == null)
            {
                score = new PersonScore
                {
                    PersonId = personId,
                    Score = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.PersonScores.Add(score);
            }
            else
            {
                score.Score++;
                score.UpdatedAt = DateTime.UtcNow;
                _context.PersonScores.Update(score);
            }

            _context.SaveChanges();
            return score.Score;
        }
    }
}
