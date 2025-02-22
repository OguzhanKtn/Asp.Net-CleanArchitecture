using CleanArchitecture_2025.Domain.Abstraction;
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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<Entity>();
            
            foreach(var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTimeOffset.Now;
                        break;
                    case EntityState.Modified:
                        if(entry.Entity.IsDeleted == true)
                        {
                            entry.Entity.DeletedAt = DateTimeOffset.Now;
                            break;
                        }
                        else
                        {
                            entry.Entity.UpdatedAt = DateTimeOffset.Now;
                            break;
                        }
                    case EntityState.Deleted:
                        throw new ArgumentException("Hard delete is not allowed");
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
