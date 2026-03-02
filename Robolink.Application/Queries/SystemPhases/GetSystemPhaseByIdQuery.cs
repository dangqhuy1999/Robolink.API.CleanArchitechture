using MediatR;
using Robolink.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Application.Queries.SystemPhases
{
    
    public class GetSystemPhaseByIdQuery : IRequest<SystemPhaseDto?>
    {
        public Guid SystemPhaseId { get; set; }

        public GetSystemPhaseByIdQuery(Guid systemPhaseId)
        {
            SystemPhaseId = systemPhaseId;
        }
    }
}
