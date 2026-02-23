using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;
using Robolink.Core.Interfaces;
using Robolink.Application.Events;

namespace Robolink.Application.Commands.Projects
{
    /// <summary>Handler for CreateProjectCommand</summary>
    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        private readonly IProjectRepository _projectRepo;
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly IGenericRepository<Staff> _staffRepo;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateProjectCommandHandler(
            IProjectRepository projectRepo,
            IGenericRepository<Client> clientRepo,
            IGenericRepository<Staff> staffRepo,
            IMapper mapper,
            IMediator mediator)
        {
            _projectRepo = projectRepo;
            _clientRepo = clientRepo;
            _staffRepo = staffRepo;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            // ✅ Validation: Client
            var client = await _clientRepo.GetByIdAsync(request.Request.ClientId);
            if (client == null)
                throw new InvalidOperationException("Client not found");

            // ✅ Validation: Manager
            var manager = await _staffRepo.GetByIdAsync(request.Request.ManagerId);
            if (manager == null)
                throw new InvalidOperationException("Manager not found");

            // ✅ NEW: Validation: ParentProject (if provided)
            Project? parentProject = null;
            if (request.Request.ParentProjectId.HasValue && request.Request.ParentProjectId != Guid.Empty)
            {
                parentProject = await _projectRepo.GetByIdAsync(request.Request.ParentProjectId.Value);
                if (parentProject == null)
                    throw new InvalidOperationException("Parent Project not found");
                
                // Optional: Validate that both projects belong to same client
                if (parentProject.ClientId != request.Request.ClientId)
                    throw new InvalidOperationException("Sub-project must belong to the same client as parent project");
            }

            // ✅ Create entity
            var project = _mapper.Map<Project>(request.Request);
            project.CreatedBy = request.CreatedBy;

            // ✅ Save to database
            await _projectRepo.AddAsync(project);

            // ✅ Publish event
            await _mediator.Publish(
                new ProjectCreatedEvent 
                { 
                    ProjectId = project.Id, 
                    ProjectName = project.Name 
                }, 
                cancellationToken);

            // 4. THAY THẾ TOÀN BỘ ĐOẠN DƯỚI BẰNG DÒNG NÀY:
            // "Này Repo, lấy cho tôi cái DTO hoàn chỉnh của thằng vừa lưu xong"
            return await _projectRepo.GetProjectByIdWithDeepDataAsync(project.Id);
        }
    }
}