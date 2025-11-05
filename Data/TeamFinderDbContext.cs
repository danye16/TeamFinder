using Microsoft.EntityFrameworkCore;

namespace TeamFinder.Api.Data
{
    public class TeamFinderDbContext : DbContext
    {
        public TeamFinderDbContext(DbContextOptions<TeamFinderDbContext> options) : base(options)
        {
        }

        // DbSets se agregarán aquí más adelante
    }
}