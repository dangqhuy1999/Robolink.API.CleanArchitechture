using AutoMapper;
using Robolink.Application.DTOs;
using Robolink.Core.Entities;
using Robolink.Core.Enums;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for PhaseTask entity and related DTOs</summary>
    public class PhaseTaskMappingProfile : Profile
    {
        public PhaseTaskMappingProfile()
        {
            // ✅ PhaseTask → PhaseTaskDto (Entity to DTO)
            CreateMap<PhaseTask, PhaseTaskDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => (int)s.Status))
                .ForMember(d => d.PhaseName, opt => opt.MapFrom(s => 
                    s.PhaseConfig != null && s.PhaseConfig.SystemPhase != null 
                        ? s.PhaseConfig.CustomPhaseName ?? s.PhaseConfig.SystemPhase.Name 
                        : "Unknown"))
                .ForMember(d => d.AssignedStaffName, opt => opt.MapFrom(s => 
                    s.AssignedStaff != null 
                        ? s.AssignedStaff.FullName 
                        : "Unassigned"));
            
            // ✅ PhaseTaskDto → PhaseTask (DTO to Entity) - ONLY map existing properties on PhaseTask
            CreateMap<PhaseTaskDto, PhaseTask>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => (Task_Status)s.Status))
                // ❌ REMOVED: .ForMember(d => d.PhaseName, ...) - doesn't exist on PhaseTask
                // ❌ REMOVED: .ForMember(d => d.AssignedStaffName, ...) - doesn't exist on PhaseTask
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // ✅ CreatePhaseTaskRequest → PhaseTask (Request to Entity)
            CreateMap<CreatePhaseTaskRequest, PhaseTask>()
                .ForMember(d => d.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(d => d.Status, opt => opt.MapFrom(_ => Task_Status.Pending));
                // ❌ REMOVED: PhaseName & AssignedStaffName don't exist on PhaseTask

            // ✅ UpdatePhaseTaskRequest → PhaseTask (Request to Entity)
            CreateMap<UpdatePhaseTaskRequest, PhaseTask>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => (Task_Status)s.Status))
                // ❌ REMOVED: PhaseName & AssignedStaffName don't exist on PhaseTask
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}