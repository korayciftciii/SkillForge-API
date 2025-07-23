using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Commands.Update;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Tests.Application.Projects
{
    public class UpdateProjectCommandHandlerTests
    {
        private readonly AppDbContext _context;

        public UpdateProjectCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UpdateDb")
                .Options;

            _context = new AppDbContext(options);
        }

        [Fact]
        public async Task Handle_ShouldUpdateProject_WhenExists()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Old",
                Description = "Old Desc"
            };
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            var command = new UpdateProjectCommand
            {
                Id = project.Id,
                Title = "New",
                Description = "New Desc",
                RepositoryUrl = "http://updated"
            };

            var handler = new UpdateProjectCommandHandler(_context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            var updated = await _context.Projects.FindAsync(project.Id);
            updated!.Title.Should().Be("New");
            updated.Description.Should().Be("New Desc");
            updated.RepositoryUrl.Should().Be("http://updated");
        }
    }
}
