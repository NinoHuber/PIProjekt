using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PIProjekt.Controllers;
using PIProjekt.Data;
using PIProjekt.Dtos;
using PIProjekt.Models;

namespace PIProjekt.Tests {
    public class WeddingPlannerControllerUnitTests {
        private AppDbContext GetInMemoryDbContext() {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateWedding_ValidPayload_ReturnsCreatedAndPersists() {
            // Arrange
            using var context = GetInMemoryDbContext();

            // Seed a mock partner so verification passes
            var vendor = new Partner { Id = 10, Name = "Test Florist", Price = 500 };
            context.Partners.Add(vendor);
            await context.SaveChangesAsync();

            var controller = new WeddingPlannerController(context);
            var submission = new WeddingSubmissionDto {
                WeddingDate = DateTime.Today.AddDays(45),
                FloristId = 10
            };

            // Act
            var result = await controller.CreateWedding(submission);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            // Verify database records exist
            var wedding = await context.Weddings.Include(w => w.WeddingPartners).FirstAsync();
            Assert.Single(wedding.WeddingPartners);
            Assert.Equal(10, wedding.WeddingPartners.First().PartnerId);
        }

        [Fact]
        public async Task CreateWedding_PastDate_ReturnsBadRequest() {
            // Arrange
            using var context = GetInMemoryDbContext();
            var controller = new WeddingPlannerController(context);
            var submission = new WeddingSubmissionDto {
                WeddingDate = DateTime.Today.AddDays(-5) // Past date
            };

            // Act
            var result = await controller.CreateWedding(submission);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The wedding date cannot be set in the past.", badRequestResult.Value);
        }
    }
}