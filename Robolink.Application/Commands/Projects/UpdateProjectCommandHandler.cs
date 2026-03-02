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
            var updateId = request.Request.Id ?? throw new ArgumentNullException("Id cannot be null");

            // 1. Lấy dữ liệu cũ từ DB
            var project = await _projectRepo.GetByIdAsync(updateId)
                ?? throw new InvalidOperationException("Project not found");

            // 2. Validate Manager (Chỉ kiểm tra, không cần gán tay)
            if (request.Request.ManagerId.HasValue && request.Request.ManagerId != Guid.Empty)
            {
                var manager = await _staffRepo.GetByIdAsync(request.Request.ManagerId.Value);
                if (manager == null) throw new InvalidOperationException("Manager not found");
            }

            // 3. Validate ParentProject (Chỉ kiểm tra vòng lặp)
            if (request.Request.ParentProjectId.HasValue && request.Request.ParentProjectId != Guid.Empty)
            {
                var parentProject = await _projectRepo.GetByIdAsync(request.Request.ParentProjectId.Value);
                if (parentProject == null) throw new InvalidOperationException("Parent Project not found");
                if (request.Request.ParentProjectId == project.Id)
                    throw new InvalidOperationException("A project cannot be its own parent");
            }

            // 4. 🚀 MÁY GIẶT AUTOMAPPER: Thay thế toàn bộ đống if gán tay của em!
            // Nó sẽ tự động đè các giá trị khác Null từ Request vào Entity Project
            _mapper.Map(request.Request, project);

            // Xử lý ngoại lệ nhỏ: Nếu user truyền Guid.Empty nghĩa là muốn gỡ Parent ra (cho bằng null)
            if (request.Request.ParentProjectId == Guid.Empty)
            {
                project.ParentProjectId = null;
            }

            // Gán thông tin người cập nhật
            project.UpdatedBy = request.UpdatedBy ?? "System";

            // 5. Lưu vào Database (NHỚ PHẢI CÓ SAVE CHANGES)
            await _projectRepo.UpdateAsync(project);
            await _projectRepo.SaveChangesAsync(); // ❌ Đoạn cũ của em bị thiếu dòng này!

            // 6. 🚀 ĂN TIỀN LÀ Ở ĐÂY: Lấy DTO xịn từ Generic Framework
            return await _projectRepo.GetProjectedByIdAsync<ProjectDto>(project.Id)
                   ?? throw new InvalidOperationException("Failed to retrieve updated project");
        }
    }
}