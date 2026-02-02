using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.SystemPhases
{
    public class DeleteSystemPhaseCommandHandler : IRequestHandler<DeleteSystemPhaseCommand, bool>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IProjectSystemPhaseConfigRepository _configRepo;

        public DeleteSystemPhaseCommandHandler(
            IGenericRepository<SystemPhase> phaseRepo,
            IProjectSystemPhaseConfigRepository configRepo)
        {
            _phaseRepo = phaseRepo;
            _configRepo = configRepo;
        }

        public async Task<bool> Handle(DeleteSystemPhaseCommand request, CancellationToken cancellationToken)
        {
            var phase = await _phaseRepo.GetByIdAsync(request.SystemPhaseId);
            if (phase == null)
                throw new InvalidOperationException("System Phase not found");

            // Check if phase is used by any project
            var usageCount = await _configRepo.GetBySystemPhaseIdAsync(request.SystemPhaseId);
            if (usageCount.Any())
                throw new InvalidOperationException("Cannot delete phase that is in use by projects");

            await _phaseRepo.DeleteAsync(phase);
            await _phaseRepo.SaveChangesAsync();

            return true;
        }
    }
}