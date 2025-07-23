using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SkillForge.Application.Features.Projects.Queries.GetById;
using SkillForge.Domain.Entities;
using SkillForge.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillForge.Tests.Application.Projects
{
    public class GetProjectByIdQueryHandlerTests
    {
        private readonly AppDbContext _context;
        private readonly GetProjectByIdQueryHandler _handler;

        public GetProjectByIdQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "GetByIdDb")
                .Options;

            _context = new AppDbContext(options);
            _handler = new GetProjectByIdQueryHandler(_context);
        }

        [Fact]
        public async Task Handle_ShouldReturnProject_WhenProjectExists()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                Title = "Sample",
                Description = "Sample Desc",
                RepositoryUrl = "http://repo"
            };
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();

            var query = new GetProjectByIdQuery(project.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Title.Should().Be("Sample");
        }
    }
}
