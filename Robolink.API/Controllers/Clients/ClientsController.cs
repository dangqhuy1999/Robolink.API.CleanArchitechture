using Microsoft.AspNetCore.Mvc;
using Robolink.Application.Queries.Clients;
using Robolink.Shared.DTOs;
using MediatR;
using Robolink.Shared.Interfaces.API.Clients;

namespace Robolink.API.Controllers.Clients
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase, IClientApi // Kế thừa để ép đúng chuẩn API
    {
        private readonly IMediator _mediator;

        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<PagedResult<ClientDto>> GetAllClientsAsync() // Sửa kiểu trả về thành PagedResult
        {
            var query = new GetAllClientsQuery(); // Query chứ không phải Command vì đây là lệnh lấy dữ liệu

            var result = await _mediator.Send(query);

            // Ép kiểu về PagedResult để khớp với đầu ra
            return (PagedResult<ClientDto>)result!;
        }
    }
}
