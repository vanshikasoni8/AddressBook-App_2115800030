using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AddressBookApp.Controllers;
using BussinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Testing
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUserBL> _mockUserService;
        private AuthController _authController;

        [SetUp]
        public void Setup()
        {
            _mockUserService = new Mock<IUserBL>();
            _authController = new AuthController(_mockUserService.Object);
        }

        [Test]
        public async Task Register_ShouldReturnCreatedAtAction_WhenUserIsRegistered()
        {
            // Arrange
            var userDTO = new UserDTO { Email = "test@example.com", Password = "password", Name = "Test User" };
            var userEntity = new UserEntity { Id = 1, Email = userDTO.Email, Name = userDTO.Name };
            _mockUserService.Setup(x => x.Register(userDTO)).ReturnsAsync(userEntity);

            // Act
            var result = await _authController.Register(userDTO);
            var createdResult = result.Result as CreatedAtActionResult;

            // Assert
            Assert.NotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual(userEntity, createdResult.Value);
        }


        [Test]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var userDTO = new UserDTO { Email = "test@example.com", Password = "password" };
            var token = "sample-jwt-token";

            _mockUserService.Setup(x => x.Login(It.IsAny<UserDTO>())).ReturnsAsync(token);

            // Act
            var result = await _authController.Login(userDTO);

            // Assert
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            // Convert response to JObject
            var jsonResponse = JObject.FromObject(okResult.Value);
            Assert.AreEqual(token, jsonResponse["Token"].ToString());
        }

        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var userDTO = new UserDTO { Email = "test@example.com", Password = "wrongpassword" };
            _mockUserService.Setup(x => x.Login(userDTO)).ReturnsAsync((string)null);

            // Act
            var result = await _authController.Login(userDTO);
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(unauthorizedResult);
            Assert.AreEqual(401, unauthorizedResult.StatusCode);
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "test@example.com" };
            _mockUserService.Setup(x => x.ForgotPassword(forgotPasswordDTO)).ReturnsAsync(true);

            // Act
            var result = await _authController.ForgotPassword(forgotPasswordDTO) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task ForgotPassword_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var forgotPasswordDTO = new ForgotPasswordDTO { Email = "nonexistent@example.com" };
            _mockUserService.Setup(x => x.ForgotPassword(forgotPasswordDTO)).ReturnsAsync(false);

            // Act
            var result = await _authController.ForgotPassword(forgotPasswordDTO) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnOk_WhenResetIsSuccessful()
        {
            // Arrange
            var resetPasswordDTO = new ResetPasswordDTO { Token = "valid-token", NewPassword = "newpassword" };
            _mockUserService.Setup(x => x.ResetPassword(resetPasswordDTO)).ReturnsAsync(true);

            // Act
            var result = await _authController.ResetPassword(resetPasswordDTO) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenResetFails()
        {
            // Arrange
            var resetPasswordDTO = new ResetPasswordDTO { Token = "invalid-token", NewPassword = "newpassword" };
            _mockUserService.Setup(x => x.ResetPassword(resetPasswordDTO)).ReturnsAsync(false);

            // Act
            var result = await _authController.ResetPassword(resetPasswordDTO) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }
    }
}
