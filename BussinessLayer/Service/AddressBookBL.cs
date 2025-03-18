using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BussinessLayer.Interface;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using StackExchange.Redis;

namespace BussinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;
        private readonly IDatabase _cache;
        private readonly TimeSpan _cacheExpiration;

        public AddressBookBL(IAddressBookRL addressBookRL, IConnectionMultiplexer redis)
        {
            _addressBookRL = addressBookRL;
            _cache = redis.GetDatabase();
            _cacheExpiration = TimeSpan.FromMinutes(10); // Cache expiry time
        }

        // Fetch all contacts with caching
        public async Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync()
        {
            string cacheKey = "contacts:all";

            // Check if data exists in Redis
            var cachedData = await _cache.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<IEnumerable<AddressBookEntity>>(cachedData);
            }

            // Fetch from DB if not cached
            var contacts = await _addressBookRL.GetAllContactsAsync();

            // Store in Redis cache
            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contacts), _cacheExpiration);

            return contacts;
        }

        // Get a contact by ID with caching
        public async Task<AddressBookEntity?> GetContactByIdAsync(int id)
        {
            string cacheKey = $"contact:{id}";

            // Try getting from cache
            var cachedData = await _cache.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<AddressBookEntity>(cachedData);
            }

            // Fetch from DB if not in cache
            var contact = await _addressBookRL.GetContactByIdAsync(id);
            if (contact != null)
            {
                await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contact), _cacheExpiration);
            }

            return contact;
        }

        // Add a new contact and update cache
        public async Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact)
        {
            var newContact = await _addressBookRL.AddContactAsync(contact);

            // Invalidate all contacts cache
            await _cache.KeyDeleteAsync("contacts:all");

            return newContact;
        }

        // Update an existing contact and refresh cache
        public async Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact)
        {
            var contact = await _addressBookRL.UpdateContactAsync(id, updatedContact);

            if (contact != null)
            {
                // Update individual contact cache
                await _cache.StringSetAsync($"contact:{id}", JsonSerializer.Serialize(contact), _cacheExpiration);
                // Invalidate all contacts cache
                await _cache.KeyDeleteAsync("contacts:all");
            }

            return contact;
        }

        // Delete a contact and remove from cache
        public async Task<bool> DeleteContactAsync(int id)
        {
            var isDeleted = await _addressBookRL.DeleteContactAsync(id);

            if (isDeleted)
            {
                // Remove from Redis cache
                await _cache.KeyDeleteAsync($"contact:{id}");
                await _cache.KeyDeleteAsync("contacts:all");
            }

            return isDeleted;
        }
    }
}
