using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Queries.GetById;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Utilities;

namespace SkillForge.Tests.Application.Projects
{
    public class GetProjectByIdQueryHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly GetProjectByIdQueryHandler _handler;

        public GetProjectByIdQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dateTimeProvider = new DateTimeProvider();
            _context = new AppDbContext(options, dateTimeProvider, null);
            _handler = new GetProjectByIdQueryHandler(_context);
        }

        [Fact]
        public async Task Handle_ShouldReturnProject_WhenExists()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Test Project",
                Description = "Test Description"
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var query = new GetProjectByIdQuery(project.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be("Test Project");
        }
    }
}
