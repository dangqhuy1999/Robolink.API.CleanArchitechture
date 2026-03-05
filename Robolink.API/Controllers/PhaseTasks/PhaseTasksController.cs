using MediatR;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.WebApp.Components.Features.PhaseTasks.Shared; // Interface em vừa tạo


namespace Robolink.API.Controllers.PhaseTasks
{
    [ApiController]
    [Route("api/phasetasks")]
    public class ProjectPhasesController : ControllerBase // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public ProjectPhasesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<PhaseTaskDto>>> GetPhaseTasksPagedAsync(
            [FromQuery] int startIndex, 
            [FromQuery] int count, 
            [FromQuery] Guid phaseId,
            [FromQuery] string? searchTerm
            )
        {
            // Controller lúc này chỉ đóng vai trò "người chuyển tin"
            // Nó gửi yêu cầu xuống tầng Application thông qua Mediator
            var result = await _mediator.Send(new GetPhaseTasksPagedQuery(startIndex, count, phaseId, searchTerm));
            return Ok(result); // Nhất quán với các hàm khác
        }

        [HttpGet("{id:guid}", Name = "GetPhaseTaskById")]
        public async Task<ActionResult<PhaseTaskDto>> GetByIdAsync(Guid id)
        {
            var result = await _mediator.Send(new GetPhaseTaskByIdQuery(id));
            if (result == null)
            {
                return NotFound(); // Trả về 404: Chuẩn dự án lớn
            }
            return Ok(result); // Trả về 200 kèm data
        }


        [HttpPost("sample")]
        public async Task<ActionResult<PhaseTaskDto>> QuickCreateAsync(Guid ProjectId,Guid ProjectSystemPhaseConfigId)
        {
            // Tạo Command từ Request (Application nắm giữ logic này)
            var command = new CreatePhaseTaskCommand
            {
                Request = new CreatePhaseTaskRequest()
                {
                    Name = "New Auto Task",
                    Description = "Auto created task",

                    // Cần truyền đủ ID của Project và Phase (Lấy từ Parameter của component)
                    ProjectId = ProjectId,
                    ProjectSystemPhaseConfigId = ProjectSystemPhaseConfigId,

                    // Thông tin nhân viên (Cần Guid và Name cụ thể)
                    AssignedStaffId = PhaseTaskConstants.DefaultManagerId, // Giả sử dùng ManagerId làm StaffId
                    AssignedStaffName = "Manager Name", // Phải có vì Request yêu cầu null!

                    // Thời gian (Lưu ý: dùng DueDate thay vì Deadline)
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(PhaseTaskConstants.DefaultPhaseTaskDurationDays),

                    // Tài chính & Trạng thái
                    InternalBudget = PhaseTaskConstants.DefaultInternalBudget,
                    CustomerBudget = PhaseTaskConstants.DefaultCustomerBudget,
                    Priority = PhaseTaskConstants.DefaultPhaseTaskPriority,
                    Status = 1, // Thêm giá trị mặc định cho Status
                    ProcessRate = 0,
                    EstimatedHours = 8, // Thêm số giờ dự kiến nếu cần

                    // Nếu là Task cha thì để null, nếu là Sub-task thì truyền ID cha vào
                    ParentPhaseTaskId = null
                },
                CreatedBy = "Huy Dang"
            };

            // Gửi đi và nhận lại DTO
            var result = await _mediator.Send(command);


            // ✅ THAY THẾ CreatedAtAction BẰNG Ok
            // Không còn lo lỗi "No route matches", không lo sập Server nữa.
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PhaseTaskDto>> CreateAsync([FromBody] CreatePhaseTaskRequest request)
        {
            // Tạo Command từ Request (Application nắm giữ logic này)
            var command = new CreatePhaseTaskCommand
            {
                Request = request,
                CreatedBy = "Huy Dang"
            };

            // Gửi đi và nhận lại DTO
            var result = await _mediator.Send(command);

            // Ép kiểu để khớp với khai báo Task<PhaseTaskDto> của hàm
            /*
             1. Tại sao phải ép kiểu (Cast)?
            Đầu vào khác Đầu ra: Trong Controller, 
            em nhận vào CreatePhaseTaskRequest (Dữ liệu thô từ WebApp). 
            Nhưng sau khi xử lý xong, 
            người dùng cần nhận lại PhaseTaskDto 
            (Dữ liệu đã có ID, có thông tin đầy đủ để hiển thị lên UI).

            Cơ chế MediatR: Khi em gọi _mediator.Send(command), 
            MediatR trả về một kết quả kiểu object hoặc kiểu 
            được định nghĩa trong IRequest<T>. 
            Nếu Visual Studio không chắc chắn T đó là gì, 
            nó sẽ báo lỗi CS0266. 
            Việc ép kiểu (PhaseTaskDto) là để khẳng định với chương trình: 
            "Tôi biết chắc chắn kết quả trả về là DTO này".
             */
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PhaseTaskDto>> UpdateAsync(Guid id, [FromBody] UpdatePhaseTaskRequest request)
        {
            var command = new UpdatePhaseTaskCommand
            {
                Id = id,
                Request = request,
                UpdatedBy = "Huy Dang"
            };

            var result = await _mediator.Send(command);

            // Nếu kết quả null (do ID không tồn tại), trả về 404
            if (result == null) return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id) // Bỏ ActionResult để khớp 100% với Interface
        {
            // Handler nên trả về bool (thành công/thất bại)
            var result = await _mediator.Send(new DeletePhaseTaskCommand(id));

            if (!result)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy để xóa
            }

            return NoContent(); // Trả về 204: "Đã xóa xong, không còn gì để trả về"
        }
    }
}
