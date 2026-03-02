using MediatR;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.ProjectPhases;
using Robolink.Application.Queries.Projects;
using Robolink.Application.Queries.SystemPhases;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.WebApp.Components.Features.PhaseTasks.Shared; // Interface em vừa tạo


namespace Robolink.API.Controllers.SystemPhases
{
    [ApiController]
    [Route("api/systemphases")]
    public class SystemPhasesController : ControllerBase // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public SystemPhasesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SystemPhaseDto>>> GetAllAsync([FromQuery] bool onlyActive = true)
        {
            // Gửi Query kèm theo "công tắc" OnlyActive mà chị em mình đã bàn
            var result = await _mediator.Send(new GetAllSystemPhasesQuery(onlyActive));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SystemPhaseDto>> Create([FromBody] CreateSystemPhaseRequest request)
        {
            // Đưa vào Command (kèm theo thông tin người tạo nếu có)
            var command = new CreateSystemPhaseCommand(request, User.Identity?.Name ?? "System");

            var result = await _mediator.Send(command);

            // ✅ THAY THẾ CreatedAtAction BẰNG Ok
            // Không còn lo lỗi "No route matches", không lo sập Server nữa.
            return Ok(result);
        }

        [HttpGet("{id:guid}", Name = "GetSystemPhaseById")]
        public async Task<ActionResult<SystemPhaseDto>> GetById(Guid id)
        {
            // Em có thể tạo 1 Query riêng: GetSystemPhaseByIdQuery(id)
            // Hoặc nếu lười tạo file mới, gọi thẳng Repo ở đây (nhưng không khuyến khích nhé)
            var result = await _mediator.Send(new GetSystemPhaseByIdQuery(id));

            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SystemPhaseDto>> Update(Guid id, [FromBody] UpdateSystemPhaseRequest request)
        {
            // Đảm bảo ID trong Body khớp với ID trên URL
            request.Id = id;

            var result = await _mediator.Send(new UpdateSystemPhaseCommand(request));
            return Ok(result);
        }
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SystemPhaseDto>>> GetSystemPhasesPagedAsync(
            [FromQuery] int startIndex,
            [FromQuery] int count,
            [FromQuery] string? searchTerm = null) // 👈 Thêm cái này)
        {
            var result = await _mediator.Send(new GetSystemPhasesPagedQuery(startIndex, count, searchTerm));
            return Ok(result); // Nhất quán với các hàm khác
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            // Handler nên trả về bool (thành công/thất bại)
            var result = await _mediator.Send(new DeleteSystemPhaseCommand(id));

            if (!result)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy để xóa
            }

            return NoContent(); // Trả về 204: "Đã xóa xong, không còn gì để trả về"
        }
    }
}
