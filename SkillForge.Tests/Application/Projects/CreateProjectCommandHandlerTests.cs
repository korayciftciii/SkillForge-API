using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Commands.Create;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Utilities;

namespace SkillForge.Tests.Application.Projects
{
    public class CreateProjectCommandHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly CreateProjectCommandHandler _handler;

        public CreateProjectCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dateTimeProvider = new DateTimeProvider();
            _context = new AppDbContext(options, dateTimeProvider, null);
            _handler = new CreateProjectCommandHandler(_context);
        }

        [Fact]
        public async Task Handle_ShouldCreateProject_WhenValidRequest()
        {
            // Arrange
            var command = new CreateProjectCommand
            {
                Title = "Test Title",
                Description = "Test Description",
                RepositoryUrl = "http://github.com/test"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be("Test Title");
        }
    }
}
