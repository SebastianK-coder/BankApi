using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using static BankApi.Services.Program;


namespace BankApi.Data
{
    public class BankingDbContext : DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options)
            : base(options)
        {
        }

        public DbSet<KontoBankowe> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KontoBankowe>()
                .HasKey(k => k.NumerKonta);
            modelBuilder.Entity<KontoBankowe>()
                .Property(k => k.Saldo)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Kwota)
                .HasPrecision(18, 2);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = WebApplication.CreateBuilder();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
