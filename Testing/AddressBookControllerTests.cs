using BussinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using Moq;
using NUnit.Framework;
using WebApplication1.Controllers;

namespace Testing
{
    [TestFixture]
    public class AddressBookControllerTests
    {
        private Mock<IAddressBookBL> _mockAddressBookBL;
        private AddressBookController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAddressBookBL = new Mock<IAddressBookBL>();
            _controller = new AddressBookController(_mockAddressBookBL.Object);
        }

        [Test]
        public async Task GetAllContacts_ReturnsOk_WithListOfContacts()
        {
            // Arrange
            var contacts = new List<AddressBookEntity>
            {
                new AddressBookEntity { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new AddressBookEntity { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            };

            _mockAddressBookBL.Setup(bl => bl.GetAllContactsAsync()).ReturnsAsync(contacts);

            // Act
            var result = await _controller.GetAllContacts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(contacts, okResult.Value);
        }

        [Test]
        public async Task GetContactById_ExistingId_ReturnsOk()
        {
            var contact = new AddressBookEntity { Id = 1, Name = "John Doe", Email = "john@example.com" };
            _mockAddressBookBL.Setup(bl => bl.GetContactByIdAsync(1)).ReturnsAsync(contact);

            var result = await _controller.GetContactById(1);

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(contact, okResult.Value);
        }

        [Test]
        public async Task GetContactById_NonExistingId_ReturnsNotFound()
        {
            _mockAddressBookBL.Setup(bl => bl.GetContactByIdAsync(99)).ReturnsAsync((AddressBookEntity)null);

            var result = await _controller.GetContactById(99);

            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task AddContact_ValidContact_ReturnsCreatedAtAction()
        {
            var contact = new AddressBookEntity { Id = 1, Name = "John Doe", Email = "john@example.com" };
            _mockAddressBookBL.Setup(bl => bl.AddContactAsync(It.IsAny<AddressBookEntity>())).ReturnsAsync(contact);

            var result = await _controller.AddContact(contact);

            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);
            var createdAtResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtResult);
            Assert.AreEqual(contact, createdAtResult.Value);
        }

        [Test]
        public async Task UpdateContact_ExistingId_ReturnsNoContent()
        {
            var updatedContact = new AddressBookEntity { Id = 1, Name = "John Updated", Email = "john@example.com" };
            _mockAddressBookBL.Setup(bl => bl.UpdateContactAsync(1, updatedContact)).ReturnsAsync(updatedContact);

            var result = await _controller.UpdateContact(1, updatedContact);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateContact_NonExistingId_ReturnsNotFound()
        {
            _mockAddressBookBL.Setup(bl => bl.UpdateContactAsync(99, It.IsAny<AddressBookEntity>())).ReturnsAsync((AddressBookEntity)null);

            var result = await _controller.UpdateContact(99, new AddressBookEntity());

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task DeleteContact_ExistingId_ReturnsNoContent()
        {
            _mockAddressBookBL.Setup(bl => bl.DeleteContactAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteContact(1);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task DeleteContact_NonExistingId_ReturnsNotFound()
        {
            _mockAddressBookBL.Setup(bl => bl.DeleteContactAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeleteContact(99);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
