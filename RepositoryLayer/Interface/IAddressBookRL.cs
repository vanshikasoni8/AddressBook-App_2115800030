using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        
        Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync();
        Task<AddressBookEntity?> GetContactByIdAsync(int id);
        Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact);
        Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact);
        Task<bool> DeleteContactAsync(int id);
    }
}
