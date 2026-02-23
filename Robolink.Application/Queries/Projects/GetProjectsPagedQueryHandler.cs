using AutoMapper;
using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Shared.DTOs;
using System.Linq.Expressions;

namespace Robolink.Application.Queries.Projects
{
    public class GetProjectsPagedQueryHandler : IRequestHandler<GetProjectsPagedQuery, PagedResult<ProjectDto>>
    {
        // 1. Đổi từ Generic sang Repo chuyên biệt
        private readonly IProjectRepository _projectRepo;

        // Lúc này em thậm chí KHÔNG CẦN IMapper ở Handler nữa 
        // vì Repo đã làm hộ việc đó thông qua ProjectTo rồi!
        public GetProjectsPagedQueryHandler(IProjectRepository projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<PagedResult<ProjectDto>> Handle(GetProjectsPagedQuery request, CancellationToken cancellationToken)
        {
            // 2. Gọi hàm "đặc sản" của ProjectRepository
            // Hàm này trả về thẳng ProjectDto nên cực kỳ nhàn
            (var items, var totalCount) = await _projectRepo.GetProjectsWithDeepDataAsync(
                request.StartIndex,
                request.Count
            );

            // --- ĐOẠN DEBUG (Tùy chọn, để em kiểm tra xem con cháu đã lên đủ chưa) ---
            System.Diagnostics.Debug.WriteLine($"📦 Query (ProjectTo) returned {items.Count()} DTOs");
            foreach (var dto in items)
            {
                // Kiểm tra ClientName và SubProjects xem có "Unknown" hay 0 không
                System.Diagnostics.Debug.WriteLine($"  📌 {dto.ProjectCode}: Client = {dto.ClientName}, SubProjects = {dto.SubProjects?.Count ?? 0}");
            }
            // ----------------------------------------------------------------------

            // 3. Trả về kết quả luôn - Không cần gọi _mapper.Map nữa!
            return new PagedResult<ProjectDto>(items, totalCount);
        }
    }
}