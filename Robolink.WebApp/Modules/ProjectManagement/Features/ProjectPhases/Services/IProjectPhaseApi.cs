using Refit;
using Robolink.Shared.DTOs; // Thay bằng namespace DTO của em

namespace Robolink.WebApp.Modules.ProjectManagement.Features.ProjectPhases.Services
{
    public interface IProjectPhaseApi
    {
        // Bỏ chữ 's' ở project và đổi PageResult thành IEnumerable
        [Get("/api/projectphases/project/{projectId}")]
        Task<IEnumerable<ProjectPhaseConfigDto>> GetPhasesByProjectAsync(Guid projectId);

        [Post("/api/projectphases/assign")]
        Task<ProjectPhaseConfigDto> AssignPhaseToProjectAsync([Body] AssignPhaseRequest request);

        [Put("/api/project-phases/{id}/config")]
        Task<ProjectPhaseConfigDto> UpdateConfigAsync(Guid id, [Body] UpdateProjectPhaseConfigRequest request);

        [Delete("/api/projectphases/{phaseConfigId}")]
        Task<bool> RemovePhaseFromProjectAsync(Guid phaseConfigId);
    }
}
