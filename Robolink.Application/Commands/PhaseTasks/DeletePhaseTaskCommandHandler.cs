using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.PhaseTasks
{
    public class DeletePhaseTaskCommandHandler : IRequestHandler<DeletePhaseTaskCommand, bool>
    {
        private readonly IGenericRepository<PhaseTask> _taskRepo;

        public DeletePhaseTaskCommandHandler(IGenericRepository<PhaseTask> taskRepo)
        {
            _taskRepo = taskRepo;
        }

        public async Task<bool> Handle(DeletePhaseTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _taskRepo.GetByIdAsync(request.PhaseTaskId);
            if (task == null)
                throw new InvalidOperationException("Phase task not found");

            // Trong DeletePhaseTaskCommandHandler
            await _taskRepo.SoftDeleteAsync(request.PhaseTaskId);
            // KHÔNG CẦN gọi _taskRepo.SaveChangesAsync() nữa nhé! 
            // Vì bên trong SoftDeleteAsync chị đã cho nó tự Save luôn rồi.

            return true;
        }
    }
}