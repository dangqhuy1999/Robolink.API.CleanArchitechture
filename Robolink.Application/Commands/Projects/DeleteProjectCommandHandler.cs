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
            if (project == null) return false;

            // 1. Tìm tất cả dự án con (bao gồm cả những thằng đã bị xóa nếu cần, hoặc chỉ thằng active)
            var subProjects = await _projectRepo.FindAsync(p => p.ParentProjectId == request.ProjectId);

            if (subProjects.Any())
            {
                foreach (var sub in subProjects)
                {
                    if (request.HardDelete)
                        await _projectRepo.DeleteAsync(sub.Id);
                    else
                        await _projectRepo.SoftDeleteAsync(sub.Id);
                }
            }

            // 2. Xóa chính nó
            if (request.HardDelete)
                await _projectRepo.DeleteAsync(request.ProjectId);
            else
                await _projectRepo.SoftDeleteAsync(request.ProjectId);

            await _projectRepo.SaveChangesAsync();
            return true;
        }
    }
}