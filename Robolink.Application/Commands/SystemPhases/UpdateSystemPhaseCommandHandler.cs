using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.SystemPhases
{
    public class UpdateSystemPhaseCommandHandler : IRequestHandler<UpdateSystemPhaseCommand, SystemPhaseDto>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IMapper _mapper;

        public UpdateSystemPhaseCommandHandler(
            IGenericRepository<SystemPhase> phaseRepo,
            IMapper mapper)
        {
            _phaseRepo = phaseRepo;
            _mapper = mapper;
        }

        public async Task<SystemPhaseDto> Handle(UpdateSystemPhaseCommand request, CancellationToken cancellationToken)
        {
            var phase = await _phaseRepo.GetByIdAsync(request.SystemPhaseId);
            if (phase == null)
                throw new InvalidOperationException("System Phase not found");

            phase.Name = request.Name;
            phase.Description = request.Description;
            phase.IsActive = request.IsActive;
            phase.UpdatedAt = DateTime.UtcNow;

            await _phaseRepo.UpdateAsync(phase);
            await _phaseRepo.SaveChangesAsync();

            return _mapper.Map<SystemPhaseDto>(phase);
        }
    }
}