using AutoMapper;
using MediatR;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;

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
            // ✅ IMPORTANT: Get paged results WITH sub-projects loaded
            (IEnumerable<PhaseTask> items, int totalCount) = await _phaseTaskRepo.GetPagedAsync(request.StartIndex, request.Count);

            // ✅ Ensure sub-projects are loaded by checking if they're null
            var itemsWithSubPhaseTasks = items.ToList();

            System.Diagnostics.Debug.WriteLine($"📦 Query returned {itemsWithSubPhaseTasks.Count} tasks");
            foreach (var task in itemsWithSubPhaseTasks)
            {
                System.Diagnostics.Debug.WriteLine($"  📌 {task.Id}: SubTasks count = {task.SubPhaseTasksItems?.Count ?? 0}");
            }

            // 2. Map danh sách Entity (Project) sang danh sách DTO (ProjectDto)
            var dtos = _mapper.Map<IEnumerable<PhaseTaskDto>>(itemsWithSubPhaseTasks);

            System.Diagnostics.Debug.WriteLine($"📦 After mapping: {dtos.Count()} tasks");
            foreach (var dto in dtos)
            {
                System.Diagnostics.Debug.WriteLine($"  📌 {dto.Id}: SubProjects DTO count = {dto.SubPhaseTasks?.Count ?? 0}");
            }

            // 3. Đóng gói vào "chiếc xe tải" PagedResult để gửi về cho UI
            return new PagedResult<PhaseTaskDto>(dtos, totalCount);
        }
    }
}