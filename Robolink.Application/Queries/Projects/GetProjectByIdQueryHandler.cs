using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.Projects
{
    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
    {
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly IGenericRepository<Robolink.Core.Entities.Staff> _staffRepo;
        private readonly IMapper _mapper;

        public GetProjectByIdQueryHandler(
            IGenericRepository<Project> projectRepo,
            IGenericRepository<Client> clientRepo,
            IGenericRepository<Robolink.Core.Entities.Staff> staffRepo,
            IMapper mapper)
        {
            _projectRepo = projectRepo;
            _clientRepo = clientRepo;
            _staffRepo = staffRepo;
            _mapper = mapper;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var project = await _projectRepo.GetByIdAsync(request.ProjectId);
            if (project == null)
                return null;

            var dto = _mapper.Map<ProjectDto>(project);

            // Load related data
            if (project.ClientId != Guid.Empty)
            {
                var client = await _clientRepo.GetByIdAsync(project.ClientId);
                if (client != null)
                    dto.ClientName = client.Name;
            }

            if (project.ManagerId != Guid.Empty)
            {
                var manager = await _staffRepo.GetByIdAsync(project.ManagerId);
                if (manager != null)
                    dto.ManagerName = manager.FullName ?? "Unknown";
            }

            return dto;
        }
    }
}