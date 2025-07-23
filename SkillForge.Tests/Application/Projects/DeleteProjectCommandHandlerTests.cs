using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Commands.Delete;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Utilities;

namespace SkillForge.Tests.Application.Projects
{
    public class DeleteProjectCommandHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly DeleteProjectCommandHandler _handler;

        public DeleteProjectCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dateTimeProvider = new DateTimeProvider();
            _context = new AppDbContext(options, dateTimeProvider, null);
            _handler = new DeleteProjectCommandHandler(_context);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProject_WhenValidRequest()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Description = "Test Description"
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var command = new DeleteProjectCommand(project.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            var deletedProject = await _context.Projects.FindAsync(project.Id);
            deletedProject.Should().BeNull();
        }
    }
}
