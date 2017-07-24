using Microsoft.EntityFrameworkCore;

namespace Fiver.Mvc.Testing.EF
{
    public class Database : DbContext
    {
        public Database(
            DbContextOptions<Database> options) : base(options) { }

        public DbSet<MovieEntity> Movies { get; set; }
    }
}
