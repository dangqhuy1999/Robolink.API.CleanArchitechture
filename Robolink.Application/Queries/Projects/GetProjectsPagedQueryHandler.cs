using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.Projects
{
    public class GetProjectsPagedQueryHandler : IRequestHandler<GetProjectsPagedQuery, PagedResult<ProjectDto>>
    {
        private readonly IGenericRepository<Project> _projectRepo;
        private readonly IMapper _mapper;

        public GetProjectsPagedQueryHandler(IGenericRepository<Project> projectRepo, IMapper mapper)
        {
            _projectRepo = projectRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProjectDto>> Handle(GetProjectsPagedQuery request, CancellationToken cancellationToken)
        {
            // ✅ IMPORTANT: Get paged results WITH sub-projects loaded
            (IEnumerable<Project> items, int totalCount) = await _projectRepo.GetPagedAsync(request.StartIndex, request.Count);

            // ✅ Ensure sub-projects are loaded by checking if they're null
            var itemsWithSubProjects = items.ToList();

            System.Diagnostics.Debug.WriteLine($"📦 Query returned {itemsWithSubProjects.Count} projects");
            foreach (var proj in itemsWithSubProjects)
            {
                System.Diagnostics.Debug.WriteLine($"  📌 {proj.ProjectCode}: SubProjects count = {proj.SubProjectsItems?.Count ?? 0}");
            }

            // 2. Map danh sách Entity (Project) sang danh sách DTO (ProjectDto)
            var dtos = _mapper.Map<IEnumerable<ProjectDto>>(itemsWithSubProjects);

            System.Diagnostics.Debug.WriteLine($"📦 After mapping: {dtos.Count()} projects");
            foreach (var dto in dtos)
            {
                System.Diagnostics.Debug.WriteLine($"  📌 {dto.ProjectCode}: SubProjects DTO count = {dto.SubProjects?.Count ?? 0}");
            }

            // 3. Đóng gói vào "chiếc xe tải" PagedResult để gửi về cho UI
            return new PagedResult<ProjectDto>(dtos, totalCount);
        }
    }
}