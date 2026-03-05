using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.SystemPhases;
namespace Robolink.WebApp.Modules.ProjectManagement.Features.ProjectPhases.Modals
{
    public partial class AddPhaseModal : ComponentBase
    {
        [Inject] private ISystemPhaseApi SystemPhaseApi { get; set; } = null!;

        [Parameter] public bool ShowModal { get; set; }

        // Thay vì nhận list Available, ta nhận list ID đã gán để lọc
        [Parameter] public List<Guid> AssignedSystemPhaseIds { get; set; } = new();

        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback<Guid> OnAssign { get; set; }

        // State nội bộ của Modal (Tự quản lý)
        private List<SystemPhaseDto> availablePhases = new();
        private bool isLoading;
        private string? errorMessage;

        // ✅ Chạy mỗi khi Parameter thay đổi (Khi ShowModal chuyển từ false -> true)
        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal)
            {
                await LoadSystemPhases();
            }
        }

        private async Task LoadSystemPhases()
        {
            try
            {
                isLoading = true;
                errorMessage = null;

                // 1. Lấy tất cả phase từ hệ thống
                var allPhases = await SystemPhaseApi.GetAllAsync(onlyActive: true);

                // 2. Lọc bỏ những cái đã có trong Project này
                availablePhases = allPhases
                    .Where(p => !AssignedSystemPhaseIds.Contains(p.Id))
                    .OrderBy(p => p.Name) // Sắp xếp cho đẹp
                    .ToList();
            }
            catch (Exception ex)
            {
                errorMessage = "Could not load phases: " + ex.Message;
            }
            finally
            {
                isLoading = false;
            }
        }

        private Task CloseModal() => OnClose.InvokeAsync();
    }
}