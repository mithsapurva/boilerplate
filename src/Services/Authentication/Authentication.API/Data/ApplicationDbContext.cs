

namespace Authentication.API
{
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SystemAccount> SystemAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder
                .Entity<SystemAccount>()
                .HasData(
                new SystemAccount
                {
                    Id = 1,
                    SystemName = "eClaims",
                    PrivateKey = "MDwwDQYJKoZIhvcNAQEBBQADKwAwKAIhAMLKK5CsSzHynylz4bisvO94cHv9gP6t0yPs/Usf7MCNAgMBAAE="
                }
                );
        }
    }
}
