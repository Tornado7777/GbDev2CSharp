using Microsoft.EntityFrameworkCore;

namespace CloneHabr.Data
{
    public class ClonehabrDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountSession> AccountSessions { get; set; }
        public ClonehabrDbContext(DbContextOptions options) : base(options)
        {

        }
    }

}
