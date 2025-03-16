using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using BussinessLayer.Service;
using BussinessLayer.Interface;
using ModelLayer.Model;
using ModelLayer.DTO;
namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;

        public AddressBookController(IAddressBookBL addressBookBL)
        {
            _addressBookBL = addressBookBL;
        }

        // Fetching all contacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressBookEntity>>> GetAllContacts()
        {
            return Ok(await _addressBookBL.GetAllContactsAsync());
        }

        // get a contact by Id
        [HttpGet("Get Contact")]
        public async Task<ActionResult<AddressBookEntity>> GetContactById(int id)
        {
            var contact = await _addressBookBL.GetContactByIdAsync(id);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        //for adding New Contacts
        [HttpPost]
        [Route("AddContact")]
        public async Task<ActionResult<AddressBookEntity>> AddContact(AddressBookEntity contact)
        {
            var newContact = await _addressBookBL.AddContactAsync(contact);
            return CreatedAtAction(nameof(GetContactById), new { id = newContact.Id }, newContact);
        }

        //for updating contacts
        [HttpPut("UpdateContact")]
        public async Task<IActionResult> UpdateContact(int id, AddressBookEntity updatedContact)
        {
            var contact = await _addressBookBL.UpdateContactAsync(id, updatedContact);
            if (contact == null) return NotFound();
            return NoContent();
        }

        // for deleting Contacts
        [HttpDelete("DeleteContact")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var isDeleted = await _addressBookBL.DeleteContactAsync(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
    }
}
