using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;

        public AddressBookBL(IAddressBookRL addressBookRL)
        {
            _addressBookRL = addressBookRL;
        }

        // Fetch all contacts
        public async Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync()
        {
            return await _addressBookRL.GetAllContactsAsync();
        }

        // Get a contact by ID
        public async Task<AddressBookEntity?> GetContactByIdAsync(int id)
        {
            return await _addressBookRL.GetContactByIdAsync(id);
        }

        // Add a new contact
        public async Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact)
        {
            return await _addressBookRL.AddContactAsync(contact);
        }

        // Update an existing contact
        public async Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity   updatedContact)
        {
            return await _addressBookRL.UpdateContactAsync(id, updatedContact);
        }

        // Delete a contact
        public async Task<bool> DeleteContactAsync(int id)
        {
            return await _addressBookRL.DeleteContactAsync(id);
        }
    }
}
