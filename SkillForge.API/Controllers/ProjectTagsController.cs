using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillForge.Application.Common.Models.Pagination;
using SkillForge.Application.Features.ProjectTags.Commands.Create;
using SkillForge.Application.Features.ProjectTags.Commands.Delete;
using SkillForge.Application.Features.ProjectTags.Commands.Update;
using SkillForge.Application.Features.ProjectTags.Queries.GetAll;
using SkillForge.Application.Features.ProjectTags.Queries.GetById;
using SkillForge.Shared.Results;
using System;
using System.Threading.Tasks;

namespace SkillForge.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProjectTagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectTagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectTagCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetProjectTagByIdQuery(id));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter, [FromQuery] Guid? projectId = null)
        {
            var result = await _mediator.Send(new GetAllProjectTagsQuery(filter, projectId));
            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectTagCommand command)
        {
            if (id != command.Id)
                return BadRequest(Result.Fail("ID mismatch"));

            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] Guid projectId)
        {
            var result = await _mediator.Send(new DeleteProjectTagCommand(id, projectId));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // Additional endpoint to get tags by project
        [HttpGet("by-project/{projectId}")]
        public async Task<IActionResult> GetByProject(Guid projectId, [FromQuery] PaginationFilter filter)
        {
            var result = await _mediator.Send(new GetAllProjectTagsQuery(filter, projectId));
            return Ok(result);
        }
    }
} 