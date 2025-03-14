using Microsoft.EntityFrameworkCore;
using ModelLayer.DTO;

namespace RepositoryLayer.Context
{
    public class AddressBookContext : DbContext
    {
        public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options) { }

        public DbSet<UserEntity> User { get; set; }

        public DbSet<AddressBookEntryEntity> AddressBookEntries { get; set; }
        


    }
}
