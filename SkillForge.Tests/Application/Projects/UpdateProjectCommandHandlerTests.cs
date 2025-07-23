using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Commands.Update;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using SkillForge.Shared.Utilities;

namespace SkillForge.Tests.Application.Projects
{
    public class UpdateProjectCommandHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly UpdateProjectCommandHandler _handler;

        public UpdateProjectCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dateTimeProvider = new DateTimeProvider();
            _context = new AppDbContext(options, dateTimeProvider, null);
            _handler = new UpdateProjectCommandHandler(_context);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProject_WhenValidRequest()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Original Title",
                Description = "Original Description"
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var command = new UpdateProjectCommand
            {
                Id = project.Id,
                Title = "Updated Title",
                Description = "Updated Description"
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            var updatedProject = await _context.Projects.FindAsync(project.Id);
            updatedProject!.Title.Should().Be("Updated Title");
            updatedProject.Description.Should().Be("Updated Description");
        }
    }
}
