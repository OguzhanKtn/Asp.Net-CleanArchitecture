using CleanArchitecture_2025.Domain.Employees;
using GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture_2025.Infrastructure.Context
{
    internal sealed class ApplicationDbContext : DbContext,IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employee>? Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
