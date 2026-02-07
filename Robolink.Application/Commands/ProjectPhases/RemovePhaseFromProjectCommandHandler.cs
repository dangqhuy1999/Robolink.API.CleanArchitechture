using MediatR;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.ProjectPhases
{
    public class RemovePhaseFromProjectCommandHandler : IRequestHandler<RemovePhaseFromProjectCommand, bool>
    {
        private readonly IProjectSystemPhaseConfigRepository _configRepo;

        public RemovePhaseFromProjectCommandHandler(
            IProjectSystemPhaseConfigRepository configRepo)
        {
            _configRepo = configRepo;
        }

        public async Task<bool> Handle(RemovePhaseFromProjectCommand request, CancellationToken cancellationToken)
        {
            var config = await _configRepo.GetByIdAsync(request.PhaseConfigId);
            if (config == null)
                throw new InvalidOperationException("Phase config not found");

            await _configRepo.DeleteAsync(request.PhaseConfigId);
            await _configRepo.SaveChangesAsync();

            return true;
        }
    }
}