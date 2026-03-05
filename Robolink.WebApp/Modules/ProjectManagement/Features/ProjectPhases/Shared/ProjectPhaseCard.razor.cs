using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;

namespace Robolink.WebApp.Components.Features.ProjectPhases.Shared
{
    public partial class ProjectPhaseCard : ComponentBase
    {
        [Parameter] public ProjectPhaseConfigDto Phase { get; set; } = new();
        [Parameter] public EventCallback<Guid> OnClick { get; set; }
        // ✅ Thêm Parameter này để nhận sự kiện xóa/thu hồi
        [Parameter] public EventCallback<Guid> OnDelete { get; set; }
        private string PhaseName => Phase.CustomPhaseName ?? Phase.SystemPhase?.Name ?? "Unknown Phase";

        private async Task HandleClick()
        {
            await OnClick.InvokeAsync(Phase.Id);
        }

        // ✅ Hàm xử lý khi nhấn nút Thu hồi
        private async Task HandleDelete()
        {
            await OnDelete.InvokeAsync(Phase.Id);
        }

    }
}
