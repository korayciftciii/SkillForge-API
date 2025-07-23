using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Commands.Delete;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Tests.Application.Projects
{
    public class DeleteProjectCommandHandlerTests
    {
        private readonly AppDbContext _context;

        public DeleteProjectCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "DeleteDb")
                .Options;

            _context = new AppDbContext(options);
        }

        [Fact]
        public async Task Handle_ShouldDeleteProject_WhenExists()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Delete Me"
            };
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            var handler = new DeleteProjectCommandHandler(_context);
            var command = new DeleteProjectCommand(project.Id);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            _context.Projects.Any(p => p.Id == project.Id).Should().BeFalse();
        }
    }
}
