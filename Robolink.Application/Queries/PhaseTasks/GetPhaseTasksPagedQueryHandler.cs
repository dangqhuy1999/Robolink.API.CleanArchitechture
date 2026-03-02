using AutoMapper;
using MediatR;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using System.Linq.Expressions;

namespace Robolink.Application.Queries.PhaseTasks
{
    public class GetPhaseTasksPagedQueryHandler : IRequestHandler<GetPhaseTasksPagedQuery, PagedResult<PhaseTaskDto>>
    {
        private readonly IGenericRepository<PhaseTask> _phaseTaskRepo;

        // ✅ Không cần IMapper nữa, Repo sẽ lo việc ánh xạ (Map)
        public GetPhaseTasksPagedQueryHandler(IGenericRepository<PhaseTask> phaseTaskRepo)
        {
            _phaseTaskRepo = phaseTaskRepo;
        }

        public async Task<PagedResult<PhaseTaskDto>> Handle(GetPhaseTasksPagedQuery request, CancellationToken cancellationToken)
        {
            var term = request.SearchTerm?.Trim().ToLower();

            // Xây dựng query linh hoạt
            Expression<Func<PhaseTask, bool>> predicate = x =>
                x.ProjectSystemPhaseConfigId == request.PhaseId &&
                (string.IsNullOrEmpty(term) || x.Name.ToLower().Contains(term));

            // 3. Gọi hàm "thần thánh" đã được Projection (ProjectTo)
            // SQL sinh ra sẽ chỉ SELECT những cột có trong PhaseTaskDto -> Cực nhanh!
            return await _phaseTaskRepo.GetPagedProjectedAsync<PhaseTaskDto>(
                request.StartIndex,
                request.Count,
                predicate
            );
        }
    }
}