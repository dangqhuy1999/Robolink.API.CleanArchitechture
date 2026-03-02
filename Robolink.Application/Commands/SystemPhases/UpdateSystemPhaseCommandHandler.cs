using MediatR;
using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.SystemPhases
{
    public class UpdateSystemPhaseCommandHandler : IRequestHandler<UpdateSystemPhaseCommand, SystemPhaseDto>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IMapper _mapper;

        public UpdateSystemPhaseCommandHandler(IGenericRepository<SystemPhase> phaseRepo, IMapper mapper)
        {
            _phaseRepo = phaseRepo;
            _mapper = mapper;
        }

        public async Task<SystemPhaseDto> Handle(UpdateSystemPhaseCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu cũ từ DB
            var phase = await _phaseRepo.GetByIdAsync(request.Request.Id)
                ?? throw new InvalidOperationException("System Phase not found");

            // 2. 🚀 MÁY GIẶT AUTOMAPPER: Cập nhật đè dữ liệu từ Request vào Entity
            // Nó sẽ tự biết mapping Name -> Name, Description -> Description...
            _mapper.Map(request.Request, phase);

            // 3. Cập nhật các thông tin hệ thống (Audit fields)
            phase.UpdatedAt = DateTime.UtcNow;

            // 4. Lưu vào Database
            await _phaseRepo.UpdateAsync(phase);
            await _phaseRepo.SaveChangesAsync();

            // 5. 🚀 ĂN TIỀN: Dùng Projection để lấy DTO trả về (luôn đồng bộ với MappingProfile)
            return await _phaseRepo.GetProjectedByIdAsync<SystemPhaseDto>(phase.Id)
                   ?? throw new InvalidOperationException("Failed to retrieve updated phase");
        }
    }
}