using MediatR;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.PhaseTasks; // Interface em vừa tạo


namespace Robolink.API.Controllers.PhaseTasks
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhaseTasksController : ControllerBase, IPhaseTaskApi // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public PhaseTasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<PagedResult<PhaseTaskDto>> GetPhaseTasksAsync(int startIndex, int count, Guid phaseId)
        {
            // Controller lúc này chỉ đóng vai trò "người chuyển tin"
            // Nó gửi yêu cầu xuống tầng Application thông qua Mediator
            return await _mediator.Send(new GetPhaseTasksPagedQuery(startIndex, count, phaseId));
        }

        [HttpGet("{id}")]
        public async Task<PhaseTaskDto> GetByIdAsync(Guid id)
        {
            return await _mediator.Send(new GetPhaseTaskByIdQuery(id));
        }

        [HttpPost]
        public async Task<PhaseTaskDto> CreateAsync([Body] CreatePhaseTaskRequest request)
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
            return (PhaseTaskDto)result!;
        }

        [HttpPut("{id}")]
        public async Task<PhaseTaskDto> UpdateAsync(Guid id, [Body] UpdatePhaseTaskRequest request)
        {
            var command = new UpdatePhaseTaskCommand
            {
                Id = id,
                Request = request,
                UpdatedBy = "Huy Dang"
            };

            var result = await _mediator.Send(command);
            return (PhaseTaskDto)result!; // Ép kiểu để hết lỗi đỏ
        }
    }
}
