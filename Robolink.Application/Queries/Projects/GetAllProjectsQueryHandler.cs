using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Queries.Projects
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PagedResult<ProjectDto>>
    {
        private readonly IGenericRepository<Project> _projectRepo;

        public GetAllProjectsQueryHandler(
            IGenericRepository<Project> projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<PagedResult<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            // Không cần gán tay, không cần gọi _mapper.Map ở đây nữa!
            return await _projectRepo.GetPagedProjectedAsync<ProjectDto>(request.StartIndex, request.Count);
        }
    }
}