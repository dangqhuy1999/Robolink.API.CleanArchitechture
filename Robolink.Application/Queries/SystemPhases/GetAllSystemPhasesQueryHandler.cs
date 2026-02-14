using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.SystemPhases
{
    public class GetAllSystemPhasesQueryHandler : IRequestHandler<GetAllSystemPhasesQuery, IEnumerable<SystemPhaseDto>>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IMapper _mapper;

        public GetAllSystemPhasesQueryHandler(
            IGenericRepository<SystemPhase> phaseRepo,
            IMapper mapper)
        {
            _phaseRepo = phaseRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SystemPhaseDto>> Handle(GetAllSystemPhasesQuery request, CancellationToken cancellationToken)
        {
            var phases = await _phaseRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<SystemPhaseDto>>(phases.Where(p => p.IsActive));
        }
    }
}