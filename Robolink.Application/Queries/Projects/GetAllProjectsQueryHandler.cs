using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.Projects
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, IEnumerable<ProjectDto>>
    {
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IGenericRepository<Client> _clientRepo;
        private readonly IGenericRepository<Robolink.Core.Entities.Staff> _staffRepo;
        private readonly IMapper _mapper;

        public GetAllProjectsQueryHandler(
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

        public async Task<IEnumerable<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _projectRepo.GetAllAsync();
            var dtos = _mapper.Map<List<ProjectDto>>(projects);

            // Load related data
            var clientIds = projects.Select(p => p.ClientId).Distinct();
            var managerIds = projects.Select(p => p.ManagerId).Distinct();

            var clients = await _clientRepo.FindAsync(c => clientIds.Contains(c.Id));
            var managers = await _staffRepo.FindAsync(s => managerIds.Contains(s.Id));

            foreach (var dto in dtos)
            {
                var client = clients.FirstOrDefault(c => c.Id == dto.ClientId);
                if (client != null)
                    dto.ClientName = client.Name;

                var manager = managers.FirstOrDefault(m => m.Id == dto.ManagerId);
                if (manager != null)
                    dto.ManagerName = manager.FullName ?? "Unknown";
            }

            return dtos;
        }
    }
}