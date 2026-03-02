using AutoMapper;
using Robolink.Shared.DTOs;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;

namespace Robolink.Application.Mappers
{
    /// <summary>AutoMapper profile for PhaseTask entity and related DTOs</summary>
    public class PhaseTaskMappingProfile : Profile
    {
            public PhaseTaskMappingProfile()
            {
                // ✅ 1. Entity → DTO (Để hiển thị)
                CreateMap<PhaseTask, PhaseTaskDto>()
                    // Tự động Join lấy tên Phase
                    .ForMember(d => d.PhaseName, opt => opt.MapFrom(s =>
                        s.PhaseConfig != null ? (s.PhaseConfig.CustomPhaseName ?? s.PhaseConfig.SystemPhase.Name) : "Unknown"))

                    // Tự động Join lấy tên nhân viên
                    .ForMember(d => d.AssignedStaffName, opt => opt.MapFrom(s =>
                        s.AssignedStaff != null ? s.AssignedStaff.FullName : "Unassigned"))

                    // Map tên Task cha (nếu có) - Em đang thiếu cái này!
                    .ForMember(d => d.ParentPhaseTaskName, opt => opt.MapFrom(s =>
                        s.ParentPhaseTask != null ? s.ParentPhaseTask.Name : null));

                // ✅ 2. Request → Entity (Để tạo mới)
                CreateMap<CreatePhaseTaskRequest, PhaseTask>()
                    // Không gán ID và Status ở đây, hãy để Entity Default lo
                    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

                // ✅ 3. Request → Entity (Để cập nhật)
                CreateMap<UpdatePhaseTaskRequest, PhaseTask>()
                    // Chỉ cập nhật những gì User có nhập (khác null)
                    .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

                // 💡 Ghi chú: CreateMap<PhaseTaskDto, PhaseTask> thường không cần thiết 
                // vì chúng ta nên dùng Request object để cập nhật DB thay vì dùng ngược DTO.
            }
        }
    }
