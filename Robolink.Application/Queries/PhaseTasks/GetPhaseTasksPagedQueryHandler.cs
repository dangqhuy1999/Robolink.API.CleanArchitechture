using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using System.Linq.Expressions;

namespace Robolink.Application.Queries.PhaseTasks
{
    public class GetPhaseTasksPagedQueryHandler : IRequestHandler<GetPhaseTasksPagedQuery, PagedResult<PhaseTaskDto>>
    {
        private readonly IGenericRepository<PhaseTask> _phaseTaskRepo;
        private readonly IMapper _mapper;

        public GetPhaseTasksPagedQueryHandler(IGenericRepository<PhaseTask> phaseTaskRepo, IMapper mapper)
        {
            _phaseTaskRepo = phaseTaskRepo;
            _mapper = mapper;
        }

        public async Task<PagedResult<PhaseTaskDto>> Handle(GetPhaseTasksPagedQuery request, CancellationToken cancellationToken)
        {
            // Tạo filter để SQL chỉ lấy Task thuộc về đúng Phase hiện tại
            Expression<Func<PhaseTask, bool>> filter = x => x.ProjectSystemPhaseConfigId == request.PhaseId;

            // Gọi Repo với filter đã tạo. 
            // Repo mới (dùng ContextFactory) sẽ giúp tránh lỗi "Second operation"
            (IEnumerable<PhaseTask> items, int totalCount) = await _phaseTaskRepo.GetPagedAsync(
                request.StartIndex,
                request.Count,
                filter);

            // Debug để kiểm tra dữ liệu đã lọc đúng chưa
            System.Diagnostics.Debug.WriteLine($"📦 Phase {request.PhaseId} returned {items.Count()} tasks");

            // Map sang DTO
            var dtos = _mapper.Map<IEnumerable<PhaseTaskDto>>(items);

            return new PagedResult<PhaseTaskDto>(dtos, totalCount);
        }
    }
}