using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.SystemPhases
{
    public class CreateSystemPhaseCommandHandler : IRequestHandler<CreateSystemPhaseCommand, SystemPhaseDto>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IMapper _mapper;

        public CreateSystemPhaseCommandHandler(IGenericRepository<SystemPhase> phaseRepo, IMapper mapper)
        {
            _phaseRepo = phaseRepo;
            _mapper = mapper;
        }

        public async Task<SystemPhaseDto> Handle(CreateSystemPhaseCommand request, CancellationToken cancellationToken)
        {
            // 1. ✅ Validation: Check trùng tên dùng AnyAsync (Cực nhanh vì SQL chỉ trả về true/false)
            var isNameExists = await _phaseRepo.AnyAsync(p =>
                p.Name.ToLower() == request.Request.Name.ToLower());

            if (isNameExists)
                throw new InvalidOperationException($"Giai đoạn '{request.Request.Name}' đã tồn tại trong hệ thống.");

            // 2. ✅ MÁY GIẶT AUTOMAPPER: Biến Request thành Entity
            var phase = _mapper.Map<SystemPhase>(request.Request);

            // Gán các thông tin hệ thống
            phase.Id = Guid.NewGuid();
            phase.CreatedAt = DateTime.UtcNow;
            phase.CreatedBy = request.CreatedBy ?? "System";
            phase.RowVersion = Array.Empty<byte>(); // Giải quyết lỗi compiler

            // 3. ✅ Lưu vào database
            await _phaseRepo.AddAsync(phase);
            await _phaseRepo.SaveChangesAsync();

            // 4. ✅ ĂN TIỀN: Dùng Projection để lấy DTO xịn trả về
            // Đảm bảo dữ liệu trả về nhất quán với cấu hình MappingProfile
            return await _phaseRepo.GetProjectedByIdAsync<SystemPhaseDto>(phase.Id)
                   ?? throw new InvalidOperationException("Lỗi khi lấy dữ liệu sau khi tạo.");
        }
    }
}