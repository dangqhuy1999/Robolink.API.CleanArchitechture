using AutoMapper;
using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Shared.DTOs;
using System.Linq.Expressions;

namespace Robolink.Application.Queries.SystemPhases
{
    public class GetAllSystemPhasesQueryHandler : IRequestHandler<GetAllSystemPhasesQuery, IEnumerable<SystemPhaseDto>>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;

        public GetAllSystemPhasesQueryHandler(
            IGenericRepository<SystemPhase> phaseRepo)
        {
            _phaseRepo = phaseRepo;
        }

        public async Task<IEnumerable<SystemPhaseDto>> Handle(GetAllSystemPhasesQuery request, CancellationToken cancellationToken)
        {
            // ✅ Nếu request.OnlyActive = true thì lọc, ngược lại thì lấy hết
            Expression<Func<SystemPhase, bool>> filter = p => !request.OnlyActive || p.IsActive;

            return await _phaseRepo.GetProjectedAsync<SystemPhaseDto>(filter);
        }
    }
}