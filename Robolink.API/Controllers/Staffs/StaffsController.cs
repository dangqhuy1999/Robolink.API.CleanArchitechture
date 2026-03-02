using MediatR;
using Microsoft.AspNetCore.Mvc;
using Robolink.Application.Queries.Clients;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Staffs;

namespace Robolink.API.Controllers.Staffs
{
    [ApiController]
    [Route("api/staffs")]
    public class StaffsController : ControllerBase // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public StaffsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<StaffDto>>> GetAllStaffsAsync([FromQuery] int startIndex, [FromQuery] int count) // Sửa kiểu trả về thành PagedResult
        {
            // ❌ Sai: New không tham số trong khi Constructor bắt phải có
            // var query = new GetAllClientsQuery(); 

            // ✅ Đúng: Truyền giá trị từ URL xuống
            var query = new GetAllStaffQuery(startIndex, count);

            var result = await _mediator.Send(query);

            // ✅ Trả về Ok kèm dữ liệu
            return Ok(result);
        }
    }
}
