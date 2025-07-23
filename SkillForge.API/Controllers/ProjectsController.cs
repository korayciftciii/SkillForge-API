using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkillForge.Application.Common.Models.Pagination;
using SkillForge.Application.Features.Projects.Commands.Create;
using SkillForge.Application.Features.Projects.Commands.Delete;
using SkillForge.Application.Features.Projects.Commands.Update;
using SkillForge.Application.Features.Projects.Queries.GetAll;
using SkillForge.Application.Features.Projects.Queries.GetById;
using SkillForge.Shared.Results;
namespace SkillForge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProjectByIdQuery(id));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var result = await _mediator.Send(new GetAllProjectsQuery(filter));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateProjectCommand command)
        {
            if (id != command.Id)
                return BadRequest(Result.Fail("ID mismatch"));

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProjectCommand(id));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
