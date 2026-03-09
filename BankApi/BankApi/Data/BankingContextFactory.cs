using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankApi.Data
{
    public class BankingContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
    {
        public BankingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();
            optionsBuilder.UseSqlite("Data Source=banking.db"); // <- Twój connection string

            return new BankingDbContext(optionsBuilder.Options);
        }
    }
}
