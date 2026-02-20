using MediatR;
using Microsoft.AspNetCore.Mvc;
using Robolink.Application.Queries.Clients;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Staffs;

namespace Robolink.API.Controllers.Staffs
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffsController : ControllerBase, IStaffApi // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public StaffsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<PagedResult<StaffDto>> GetAllStaffsAsync() // Sửa kiểu trả về thành PagedResult
        {
            var query = new GetAllStaffQuery(); // Query chứ không phải Command vì đây là lệnh lấy dữ liệu

            var result = await _mediator.Send(query);

            // Ép kiểu về PagedResult để khớp với đầu ra
            return (PagedResult<StaffDto>)result!;
        }
    }
}
