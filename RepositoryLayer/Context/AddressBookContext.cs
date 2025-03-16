using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace RepositoryLayer.Context
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options)
        {
           
        }

        public DbSet<UserEntity> User { get; set; }

        public DbSet<AddressBookEntity> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
