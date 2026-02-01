using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.Projects
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly IGenericRepository<Project> _projectRepo;

        public DeleteProjectCommandHandler(IGenericRepository<Project> projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            try
            {
                if (request.HardDelete)
                    await _projectRepo.DeleteAsync(request.ProjectId);
                else
                    await _projectRepo.SoftDeleteAsync(request.ProjectId);

                await _projectRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete project: {ex.Message}");
            }
        }
    }
}