using MediatR;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Queries.Projects;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.WebApp.Components.Features.Projects.Shared; // Interface em vừa tạo


namespace Robolink.API.Controllers.Projects
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase, IProjectApi // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<PagedResult<ProjectDto>> GetProjectsPagedAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            // Controller lúc này chỉ đóng vai trò "người chuyển tin"
            // Nó gửi yêu cầu xuống tầng Application thông qua Mediator
            return await _mediator.Send(new GetProjectsPagedQuery(startIndex, count));
        }

        [HttpGet("{id}")]
        public async Task<ProjectDto> GetByIdAsync(Guid id)
        {
            return await _mediator.Send(new GetProjectByIdQuery(id));
        }


        [HttpPost("sample")]
        public async Task<ProjectDto> QuickCreateAsync()
        {
            // Tạo Command từ Request (Application nắm giữ logic này)
            var command = new CreateProjectCommand
            {
                CreatedBy = "Huy Dang",
                Request = new CreateProjectRequest()
                {
                    ProjectCode = $"{ProjectConstants.ProjectCodePrefix}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
                    Name = $"Project {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Description = "Auto created project",
                    ClientId = ProjectConstants.DefaultClientId,
                    ManagerId = ProjectConstants.DefaultManagerId,
                    StartDate = DateTime.UtcNow,
                    Deadline = DateTime.Today.AddDays(ProjectConstants.DefaultProjectDurationDays),
                    Priority = ProjectConstants.DefaultProjectPriority,
                    InternalBudget = ProjectConstants.DefaultInternalBudget,
                    CustomerBudget = ProjectConstants.DefaultCustomerBudget
                }
            };

            // Gửi đi và nhận lại DTO
            var result = await _mediator.Send(command);

            return (ProjectDto)result!;
        }

        [HttpPost]
        public async Task<ProjectDto> CreateAsync([FromBody] CreateProjectRequest request)
        {
            // Tạo Command từ Request (Application nắm giữ logic này)
            var command = new CreateProjectCommand
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
            return (ProjectDto)result!;
        }

        [HttpPut("{id}")]
        public async Task<ProjectDto> UpdateAsync(Guid id, [FromBody] UpdateProjectRequest request)
        {
            var command = new UpdateProjectCommand
            {
                Id = id,
                Request = request,
                UpdatedBy = "Huy Dang"
            };

            var result = await _mediator.Send(command);
            return (ProjectDto)result!; // Ép kiểu để hết lỗi đỏ
        }

        [HttpDelete("{id}")]
        public async Task<bool> DeleteAsync(Guid id) // Bỏ ActionResult để khớp 100% với Interface
        {
            // Đảm bảo tên tham số trong Command là PhaseTaskId giống Handler
            return await _mediator.Send(new DeleteProjectCommand(id));
        }
    }
}
