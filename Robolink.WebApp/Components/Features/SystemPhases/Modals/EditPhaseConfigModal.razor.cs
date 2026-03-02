using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.ProjectPhases;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.ProjectPhases;
using Robolink.Shared.Interfaces.API.SystemPhases;

namespace Robolink.WebApp.Components.Features.SystemPhases.Modals
{
    public partial class EditPhaseConfigModal : ComponentBase
    {
        [Inject] private ISystemPhaseApi SystemPhaseApi { get; set; } = null!; // ✅ NEW
        [Inject] private IProjectPhaseApi ProjectPhaseApi { get; set; } = null!; // ✅ NEW
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public bool ShowModal { get; set; }

        [Parameter]
        public ProjectPhaseConfigDto? Phase { get; set; }

        [Parameter]
        public EventCallback OnModalClose { get; set; }

        [Parameter]
        public EventCallback OnConfigChanged { get; set; }

        private string? CustomPhaseName;
        private int Sequence;
        private bool IsEnabled;
        private string? ErrorMessage;
        private bool IsLoading = true;

        private string? PhasesErrorMessage;
        protected override void OnParametersSet()
        {
            if (Phase != null)
            {
                CustomPhaseName = Phase.CustomPhaseName;
                Sequence = Phase.Sequence;
                IsEnabled = Phase.IsEnabled;
                ErrorMessage = null;
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                // 🚀 Đóng gói dữ liệu vào Request DTO (Shared)
                var request = new UpdateProjectPhaseConfigRequest
                {
                    Id = Phase!.Id,
                    CustomPhaseName = CustomPhaseName,
                    Sequence = Sequence,
                    IsEnabled = IsEnabled
                };

                // 🚀 Gọi API qua Refit (Thay vì gọi Mediator trực tiếp)
                var updatedDto = await ProjectPhaseApi.UpdateConfigAsync(Phase.Id, request);

                if (updatedDto != null)
                {
                    await OnConfigChanged.InvokeAsync(); // Báo trang cha load lại list
                    await OnClose(); // Đóng Modal
                }
            }
            catch (ApiException ex)
            {
                // Đọc nội dung lỗi từ Backend (ví dụ: lỗi logic, validation)
                ErrorMessage = $"Lỗi hệ thống: {ex.Content}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task RemovePhase(Guid phaseConfigId)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Bạn có chắc chắn muốn xóa Giai đoạn này khỏi dự án không?");
            if (!confirmed) return;

            try
            {
                IsLoading = true;
                // Gọi API xóa ở Backend
                var success = await ProjectPhaseApi.RemovePhaseFromProjectAsync(phaseConfigId);

                if (success)
                {
                    // 🚀 Báo cho trang cha load lại danh sách mới
                    await OnConfigChanged.InvokeAsync();

                    // 🚀 Gọi hàm OnClose() em vừa viết để đóng Modal
                    await OnClose();
                }
            }
            catch (ApiException ex)
            {
                ErrorMessage = $"Lỗi từ Server: {ex.Content}";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi hệ thống: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
        private async Task OnClose()
        {
            await OnModalClose.InvokeAsync();
        }
    }
}
