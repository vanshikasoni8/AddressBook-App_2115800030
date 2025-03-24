using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using BussinessLayer.Service;
using BussinessLayer.Interface;
using ModelLayer.Model;
using ModelLayer.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
        //[Authorize(Roles ="User,Admin")]
        [HttpGet("Get Contact")]
        public async Task<ActionResult<AddressBookEntity>> GetContactById(int id)
        {
            var contact = await _addressBookBL.GetContactByIdAsync(id);
            if (contact == null) return NotFound();
            return Ok(contact);
        }

        private int GetUserIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                foreach (var claim in identity.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }

                var userIdClaim = identity.Claims.FirstOrDefault(c =>
                                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                Console.WriteLine("YEH H: ", userIdClaim);

                if (userIdClaim != null)
                {
                    Console.WriteLine($"Extracted Claim Value: {userIdClaim.Value}");

                    if (int.TryParse(userIdClaim.Value, out int userId))
                    {
                        return userId;
                    }
                    else
                    {
                        throw new FormatException($"User ID in token is not a valid integer: {userIdClaim.Value}");
                    }

                    //return userIdClaim.;
                }
            }
            throw new UnauthorizedAccessException("User ID not found in token.");
        }




        //for adding New Contacts
        [HttpPost]
        [Route("AddContact")]
        public async Task<ActionResult<AddressBookEntity>> AddContact(AddressBookDTO contact)
        {
            var userId = GetUserIdFromToken();
            AddressBookEntity entry = new AddressBookEntity()
            {
                Name = contact.Name,
                Phone = contact.Phone,
                Email = contact.Email,
                Address = contact.Address,
                UserId = userId
            };
            var newContact = await _addressBookBL.AddContactAsync(entry);
            return Ok(newContact);
        }

        //for updating contacts
        [HttpPut("UpdateContact")]
        public async Task<IActionResult> UpdateContact(int id, AddressBookEntity updatedContact)
        {
            var contact = await _addressBookBL.UpdateContactAsync(id, updatedContact);
            if (contact == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("DeleteContact")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var isDeleted = await _addressBookBL.DeleteContactAsync(id);
            if (!isDeleted) return NotFound();
            return NoContent();
        }
    }
}
