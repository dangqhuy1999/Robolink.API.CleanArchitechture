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
        private readonly IGenericRepository<Project> _projectRepo;

        // Lúc này em thậm chí KHÔNG CẦN IMapper ở Handler nữa 
        // vì Repo đã làm hộ việc đó thông qua ProjectTo rồi!
        public GetProjectsPagedQueryHandler(IGenericRepository<Project> projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<PagedResult<ProjectDto>> Handle(GetProjectsPagedQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Project, bool>> predicate;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.ToLower();
                // Search thì tìm tuốt, không phân biệt cha con (hiện Flat list)
                predicate = x => x.Name.ToLower().Contains(term) || x.ProjectCode.ToLower().Contains(term);
            }
            else
            {
                // Không search thì chỉ hiện ông Cha (hiện Tree list)
                predicate = x => x.ParentProjectId == null;
            }

            // 3. Gọi hàm "thần thánh" của Repo, ném thêm cái predicate vào
            return await _projectRepo.GetPagedProjectedAsync<ProjectDto>(
                request.StartIndex,
                request.Count,
                predicate // 👈 Nếu searchTerm rỗng, cái này là null, nó chạy y hệt bản cũ!
            );
        }
    }
}