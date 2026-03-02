using Microsoft.AspNetCore.Mvc;
using Robolink.Application.Queries.Clients;
using Robolink.Shared.DTOs;
using MediatR;
using Robolink.Shared.Interfaces.API.Clients;

namespace Robolink.API.Controllers.Clients
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<ClientDto>>> GetAllClientsAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            // ❌ Sai: New không tham số trong khi Constructor bắt phải có
            // var query = new GetAllClientsQuery(); 

            // ✅ Đúng: Truyền giá trị từ URL xuống
            var query = new GetAllClientsQuery(startIndex, count);

            var result = await _mediator.Send(query);

            // ✅ Trả về Ok kèm dữ liệu
            return Ok(result);
        }
    }
}
