using MediatR;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Commands.ProjectPhases;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.ProjectPhases;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.WebApp.Components.Features.PhaseTasks.Shared; // Interface em vừa tạo


namespace Robolink.API.Controllers.ProjectPhases
{
    [ApiController]
    [Route("api/projectphases")]
    public class SystemPhasesController : ControllerBase // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public SystemPhasesController(IMediator mediator) => _mediator = mediator;

        // Đổi ActionResult<PageResult> thành ActionResult<IEnumerable>
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<IEnumerable<ProjectPhaseConfigDto>>> GetPhasesByProject(Guid projectId)
        {
            var result = await _mediator.Send(new GetProjectPhasesQuery(projectId));
            return Ok(result);
        }
        [HttpPost("assign")]
        public async Task<ActionResult<ProjectPhaseConfigDto>> AssignPhase([FromBody] AssignPhaseRequest request)
        {
            // Mapping từ Request sang Command
            var command = new AssignPhaseToProjectCommand(
                request.ProjectId,
                request.SystemPhaseId,
                request.CustomPhaseName
            );

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // PUT: api/project-phases/{id}/config
        [HttpPut("{id}/config")]
        public async Task<ActionResult<ProjectPhaseConfigDto>> UpdateConfig(Guid id, [FromBody] UpdateProjectPhaseConfigRequest request)
        {
            // 🛡️ Đảm bảo ID đồng nhất giữa URL và Body
            request.Id = id;

            // Gửi Command vào Handler "Máy giặt" mà mình vừa viết
            var result = await _mediator.Send(new UpdateProjectPhaseConfigCommand(request));

            return Ok(result);
        }


        // DELETE: api/projectphases/{phaseConfigId}
        [HttpDelete("{phaseConfigId}")]
        public async Task<ActionResult<bool>> RemovePhase(Guid phaseConfigId)
        {
            // Gửi Command thu hồi vào Handler
            var result = await _mediator.Send(new DeleteProjectPhaseCommand(phaseConfigId));

            if (!result) return NotFound(); // Nếu không tìm thấy Config để xóa

            return Ok(result);
        }
    }
}
