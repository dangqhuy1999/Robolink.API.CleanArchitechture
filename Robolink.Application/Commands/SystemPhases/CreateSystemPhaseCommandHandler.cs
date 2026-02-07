using MediatR;
using AutoMapper;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

namespace Robolink.Application.Commands.SystemPhases
{
    public class CreateSystemPhaseCommandHandler : IRequestHandler<CreateSystemPhaseCommand, SystemPhaseDto>
    {
        private readonly IGenericRepository<SystemPhase> _phaseRepo;
        private readonly IMapper _mapper;

        public CreateSystemPhaseCommandHandler(
            IGenericRepository<SystemPhase> phaseRepo,
            IMapper mapper)
        {
            _phaseRepo = phaseRepo;
            _mapper = mapper;
        }

        public async Task<SystemPhaseDto> Handle(CreateSystemPhaseCommand request, CancellationToken cancellationToken)
        {
            // Validate name uniqueness
            var existing = await _phaseRepo.GetAllAsync();
            if (existing.Any(p => p.Name.ToLower() == request.Name.ToLower()))
                throw new InvalidOperationException($"Phase with name '{request.Name}' already exists");

            var phase = new SystemPhase
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                DefaultSequence = request.DefaultSequence,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.CreatedBy ?? "System",
                // ✅ THÊM DÒNG NÀY ĐỂ HẾT LỖI COMPILER
                RowVersion = Array.Empty<byte>()
            };

            await _phaseRepo.AddAsync(phase);
            await _phaseRepo.SaveChangesAsync();

            return _mapper.Map<SystemPhaseDto>(phase);
        }
    }
}