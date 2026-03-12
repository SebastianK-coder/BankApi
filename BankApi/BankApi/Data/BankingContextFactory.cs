using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankApi.Data
{
    public class BankingContextFactory : IDesignTimeDbContextFactory<BankingDbContext>
    {
        public BankingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankingDbContext>();

            return new BankingDbContext(optionsBuilder.Options);
        }
    }
}
