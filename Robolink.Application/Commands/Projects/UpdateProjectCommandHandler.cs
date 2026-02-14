using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.Projects
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
    {
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IGenericRepository<Staff> _staffRepo;
        private readonly IMapper _mapper;

        public UpdateProjectCommandHandler(
            IGenericRepository<Project> projectRepo,
            IGenericRepository<Staff> staffRepo,
            IMapper mapper)
        {
            _projectRepo = projectRepo;
            _staffRepo = staffRepo;
            _mapper = mapper;
        }

        public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            // ✅ Get existing project
            var project = await _projectRepo.GetByIdAsync(request.Request.Id);
            if (project == null)
                throw new InvalidOperationException("Project not found");

            // ✅ Validate manager if updating
            if (request.Request.ManagerId.HasValue && request.Request.ManagerId != Guid.Empty)
            {
                var manager = await _staffRepo.GetByIdAsync(request.Request.ManagerId.Value);
                if (manager == null)
                    throw new InvalidOperationException("Manager not found");
            }

            // ✅ NEW: Validate ParentProject if updating
            if (request.Request.ParentProjectId.HasValue)
            {
                if (request.Request.ParentProjectId == Guid.Empty)
                {
                    // Allow removing parent (setting to null/empty)
                    project.ParentProjectId = null;
                }
                else
                {
                    // Prevent circular reference: parent project cannot be a sub-project
                    var parentProject = await _projectRepo.GetByIdAsync(request.Request.ParentProjectId.Value);
                    if (parentProject == null)
                        throw new InvalidOperationException("Parent Project not found");

                    // Prevent circular: project cannot be its own parent
                    if (request.Request.ParentProjectId == project.Id)
                        throw new InvalidOperationException("A project cannot be its own parent");

                    project.ParentProjectId = request.Request.ParentProjectId.Value;
                }
            }

            // ✅ Update other fields (only if provided)
            if (!string.IsNullOrEmpty(request.Request.Name))
                project.Name = request.Request.Name;

            if (!string.IsNullOrEmpty(request.Request.Description))
                project.Description = request.Request.Description;

            if (request.Request.ManagerId.HasValue && request.Request.ManagerId != Guid.Empty)
                project.ManagerId = request.Request.ManagerId.Value;

            if (request.Request.Deadline.HasValue)
                project.Deadline = request.Request.Deadline.Value;

            if (request.Request.Priority.HasValue)
                project.Priority = (ProjectPriority)request.Request.Priority.Value;

            if (request.Request.InternalBudget.HasValue)
                project.InternalBudget = request.Request.InternalBudget.Value;

            if (request.Request.CustomerBudget.HasValue)
                project.CustomerBudget = request.Request.CustomerBudget.Value;

            if (request.Request.Status.HasValue)
                project.Status = (ProjectStatus)request.Request.Status.Value;

            project.UpdatedBy = request.UpdatedBy ?? "System";

            // ✅ Save
            await _projectRepo.UpdateAsync(project);
            await _projectRepo.SaveChangesAsync();

            // ✅ Return DTO
            return _mapper.Map<ProjectDto>(project);
        }
    }
}