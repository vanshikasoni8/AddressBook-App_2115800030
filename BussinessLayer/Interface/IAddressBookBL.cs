using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BussinessLayer.Interface
{
    public interface IAddressBookBL
    {
        //Task<IEnumerable<AddressBookEntity>> GetAllContacts();
        //Task<AddressBookEntity?> GetContactById(int id);
        //Task<AddressBookEntity> AddContact(AddressBookEntity contact);
        //Task<AddressBookEntity?> UpdateContact(int id, AddressBookEntity contact);
        //Task<bool> DeleteContact(int id);
        Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync();
        Task<AddressBookEntity?> GetContactByIdAsync(int id);
        Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact);
        Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact);
        Task<bool> DeleteContactAsync(int id);
    }
}
