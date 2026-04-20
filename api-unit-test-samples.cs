// ============================================================
// api-unit-test-samples
// Unit testing patterns for ASP.NET Core APIs
// Stack: MSTest, Moq, C#
// ============================================================

using AccessControlApi.Controllers;
using AccessControlApi.Models;
using AccessControlApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AccessControlApi.Tests
{
    [TestClass]
    public class AccessCardsControllerTests
    {
        private Mock<IAccessCardService> _mockService = null!;
        private Mock<ILogger<AccessCardsController>> _mockLogger = null!;
        private AccessCardsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IAccessCardService>();
            _mockLogger = new Mock<ILogger<AccessCardsController>>();
            _controller = new AccessCardsController(_mockService.Object, _mockLogger.Object);
        }

        // ─── GetAll ───────────────────────────────────────────

        [TestMethod]
        public async Task GetAll_ReturnsOk_WithListOfCards()
        {
            // Arrange
            var cards = new List<AccessCard>
            {
                new() { Id = 1, HolderName = "Raju Gupta", CardNumber = "CARD-001", IsActive = true },
                new() { Id = 2, HolderName = "John Doe",   CardNumber = "CARD-002", IsActive = true }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(cards);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var returned = okResult.Value as IEnumerable<AccessCard>;
            Assert.IsNotNull(returned);
            Assert.AreEqual(2, returned.Count());
        }

        [TestMethod]
        public async Task GetAll_ReturnsOk_WithEmptyList_WhenNoCardsExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<AccessCard>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
        }

        // ─── GetById ──────────────────────────────────────────

        [TestMethod]
        public async Task GetById_ReturnsOk_WhenCardExists()
        {
            // Arrange
            var card = new AccessCard { Id = 1, HolderName = "Raju Gupta", CardNumber = "CARD-001" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(card);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

            var returned = okResult.Value as AccessCard;
            Assert.IsNotNull(returned);
            Assert.AreEqual("Raju Gupta", returned.HolderName);
        }

        [TestMethod]
        public async Task GetById_ReturnsNotFound_WhenCardDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((AccessCard?)null);

            // Act
            var result = await _controller.GetById(99);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        // ─── Create ───────────────────────────────────────────

        [TestMethod]
        public async Task Create_ReturnsCreated_WithNewCard()
        {
            // Arrange
            var newCard = new AccessCard { HolderName = "Jane Smith", CardNumber = "CARD-003", Email = "jane@example.com" };
            var createdCard = new AccessCard { Id = 3, HolderName = "Jane Smith", CardNumber = "CARD-003", Email = "jane@example.com" };
            _mockService.Setup(s => s.CreateAsync(It.IsAny<AccessCard>())).ReturnsAsync(createdCard);

            // Act
            var result = await _controller.Create(newCard);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(StatusCodes.Status201Created, createdResult.StatusCode);

            var returned = createdResult.Value as AccessCard;
            Assert.IsNotNull(returned);
            Assert.AreEqual(3, returned.Id);
        }

        [TestMethod]
        public async Task Create_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("HolderName", "Required");
            var card = new AccessCard();

            // Act
            var result = await _controller.Create(card);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        // ─── Update ───────────────────────────────────────────

        [TestMethod]
        public async Task Update_ReturnsNoContent_WhenCardExists()
        {
            // Arrange
            var card = new AccessCard { HolderName = "Updated Name", Email = "updated@example.com" };
            _mockService.Setup(s => s.UpdateAsync(1, It.IsAny<AccessCard>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(1, card);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Update_ReturnsNotFound_WhenCardDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.UpdateAsync(99, It.IsAny<AccessCard>())).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(99, new AccessCard());

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        // ─── Deactivate ───────────────────────────────────────

        [TestMethod]
        public async Task Deactivate_ReturnsNoContent_WhenCardExists()
        {
            // Arrange
            _mockService.Setup(s => s.DeactivateAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Deactivate(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task Deactivate_ReturnsNotFound_WhenCardDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.DeactivateAsync(99)).ReturnsAsync(false);

            // Act
            var result = await _controller.Deactivate(99);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        // ─── Service call verification ────────────────────────

        [TestMethod]
        public async Task Create_CallsServiceExactlyOnce()
        {
            // Arrange
            var card = new AccessCard { HolderName = "Test", CardNumber = "CARD-X", Email = "test@test.com" };
            _mockService.Setup(s => s.CreateAsync(It.IsAny<AccessCard>())).ReturnsAsync(card);

            // Act
            await _controller.Create(card);

            // Assert
            _mockService.Verify(s => s.CreateAsync(It.IsAny<AccessCard>()), Times.Once);
        }

        [TestMethod]
        public async Task Deactivate_NeverCallsDelete_UsesSoftDelete()
        {
            // Arrange
            _mockService.Setup(s => s.DeactivateAsync(1)).ReturnsAsync(true);

            // Act
            await _controller.Deactivate(1);

            // Assert — DeactivateAsync called, not any hard delete method
            _mockService.Verify(s => s.DeactivateAsync(1), Times.Once);
            _mockService.VerifyNoOtherCalls();
        }
    }
}
