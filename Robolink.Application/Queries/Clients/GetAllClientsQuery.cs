using MediatR;
using Robolink.Shared.DTOs;
using System.Collections.Generic;

namespace Robolink.Application.Queries.Clients
{
    public class GetAllClientsQuery : IRequest<PagedResult<ClientDto>>
    {
        public int StartIndex { get; set; }
        public int Count { get; set; }

        // Constructor để gán giá trị nhanh
        public GetAllClientsQuery(int startIndex, int count)
        {
            StartIndex = startIndex;
            Count = count;
        }
    }
}