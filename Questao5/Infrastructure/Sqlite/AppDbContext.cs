using Microsoft.EntityFrameworkCore;
using Questao5.Domain.Entities;

namespace Questao5.Infrastructure.Sqlite
{
    public class AppDbContext : DbContext
    {
        public DbSet<ContaCorrente> ContaCorrente { get; set; }
        public DbSet<Idempotencia> Idempotencia { get; set; }
        public DbSet<Movimento> Movimento { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.sqlite");
        }
    }
}