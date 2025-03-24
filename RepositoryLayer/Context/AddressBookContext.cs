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
            base .OnModelCreating(modelBuilder);

            modelBuilder.Entity<AddressBookEntity>()
                .HasOne(e => e.User)  // Each AddressBookEntity has one User
                .WithMany(u => u.Contacts)  // One User can have many Contacts
                .HasForeignKey(e => e.UserId)  // Foreign key in AddressBookEntity
                .OnDelete(DeleteBehavior.Cascade); // Define delete behavior
        }

    }
}
