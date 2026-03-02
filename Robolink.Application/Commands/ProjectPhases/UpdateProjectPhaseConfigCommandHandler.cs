using AutoMapper;
using MediatR;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Shared.DTOs;

namespace Robolink.Application.Commands.ProjectPhases
{
    public class UpdateProjectPhaseConfigCommandHandler : IRequestHandler<UpdateProjectPhaseConfigCommand, ProjectPhaseConfigDto>
    {
        private readonly IGenericRepository<ProjectSystemPhaseConfig> _configRepo;
        private readonly IMapper _mapper;

        public UpdateProjectPhaseConfigCommandHandler(
            IGenericRepository<ProjectSystemPhaseConfig> configRepo,
            IMapper mapper)
        {
            _configRepo = configRepo;
            _mapper = mapper;
        }

        public async Task<ProjectPhaseConfigDto> Handle(UpdateProjectPhaseConfigCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu từ DB (Bao gồm cả các liên kết nếu cần để Mapper làm việc)
            var config = await _configRepo.GetByIdAsync(request.Request.Id)
                ?? throw new InvalidOperationException("Không tìm thấy cấu hình giai đoạn.");

            // 2. 🚀 MÁY GIẶT AUTOMAPPER: Đè dữ liệu từ Request vào Entity
            // Xóa sạch đống config.CustomPhaseName = ... cũ đi nhé
            _mapper.Map(request.Request, config);

            config.UpdatedAt = DateTime.UtcNow;

            // 3. Lưu vào Database
            await _configRepo.UpdateAsync(config);
            await _configRepo.SaveChangesAsync();

            // 4. 🚀 ĂN TIỀN: Trả về DTO xịn qua Projection
            // Không cần "new ProjectPhaseConfigDto" thủ công nữa, SQL sẽ lo hết
            return await _configRepo.GetProjectedByIdAsync<ProjectPhaseConfigDto>(config.Id)
                   ?? throw new InvalidOperationException("Lỗi khi lấy dữ liệu sau cập nhật.");
        }
    }
}