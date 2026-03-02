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

        // ✂️ Bỏ ClientRepo, StaffRepo và Mapper vì GetProjectedByIdAsync đã lo hết rồi
        public GetProjectByIdQueryHandler(IGenericRepository<Project> projectRepo)
        {
            _projectRepo = projectRepo;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            // Thằng "Vũ khí mới" này sẽ thay em làm hết:
            // 1. Tìm Project theo Id.
            // 2. Nhìn vào ProjectDto xem có cần ClientName không -> Nó tự JOIN bảng Client lấy Name.
            // 3. Nhìn xem có cần ManagerName không -> Nó tự JOIN bảng Staff lấy FullName.
            // 4. Trả về kết quả gọn gàng.
            if (request.ProjectId == Guid.Empty) return null;
            return await _projectRepo.GetProjectedByIdAsync<ProjectDto>(request.ProjectId);
        }
    }
}