using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Interfaces;
using Robolink.Core.Entities;

namespace Robolink.Application.Queries.ProjectPhases
{
    public class GetProjectPhasesQueryHandler : IRequestHandler<GetProjectPhasesQuery, IEnumerable<ProjectPhaseConfigDto>>
    {
        // ✅ Đổi sang GenericRepository để dùng được Projection
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _phaseConfigRepo;

        public GetProjectPhasesQueryHandler(IGenericRepository<ProjectSystemPhaseConfig> phaseConfigRepo)
        {
            _phaseConfigRepo = phaseConfigRepo;
        }

        public async Task<IEnumerable<ProjectPhaseConfigDto>> Handle(GetProjectPhasesQuery request, CancellationToken cancellationToken)
        {
            // 🚀 Dùng hàm "Projected" để SQL tự Join và tự Map luôn
            // Không cần .Include, không cần _mapper.Map nữa!
            var dtos = await _phaseConfigRepo.GetProjectedAsync<ProjectPhaseConfigDto>(
                x => x.ProjectId == request.ProjectId
            );

            return dtos.OrderBy(d => d.Sequence);
        }
    }
}