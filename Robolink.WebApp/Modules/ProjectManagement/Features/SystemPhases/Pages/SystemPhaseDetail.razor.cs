using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Application.Queries.SystemPhases;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.SystemPhases;

namespace Robolink.WebApp.Components.Features.SystemPhases.Pages
{
    public partial class SystemPhaseDetail : ComponentBase
    {
        
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private ISystemPhaseApi SystemPhaseApi { get; set; } = null!; // ✅ NEW
        [Inject] private NavigationManager NavigationManager { get; set; } = null!; // ✅ NEW
        [Parameter]
        public Guid PhaseId { get; set; }

        private SystemPhaseDto? phase;
        private bool isLoading = true;
        private bool EditMode = false;
        private int UsageCount = 0;

        private string formName = "";
        private string formDescription = "";
        private int formSequence = 1;
        private bool formIsActive = true;

        protected override async Task OnInitializedAsync()
        {
            await LoadPhase();
        }

        private async Task LoadPhase()
        {
            try
            {
                isLoading = true;
                // 🚀 Gọi trực tiếp ID, SQL chỉ trả về 1 dòng duy nhất, cực nhẹ!
                phase = await SystemPhaseApi.GetByIdAsync(PhaseId);

                if (phase != null)
                {
                    formName = phase.Name;
                    formDescription = phase.Description ?? "";
                    formSequence = phase.DefaultSequence;
                    formIsActive = phase.IsActive;
                }
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Lỗi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task SaveChanges()
        {
            try
            {
                isLoading = true; // Hiện spinner cho chuyên nghiệp

                // 🚀 Đóng gói dữ liệu vào Request DTO
                var request = new UpdateSystemPhaseRequest
                {
                    Id = PhaseId,
                    Name = formName,
                    Description = formDescription,
                    DefaultSequence = formSequence,
                    IsActive = formIsActive
                };

                // 🚀 Gọi API thay vì Mediator
                var updatedPhase = await SystemPhaseApi.UpdateAsync(PhaseId, request);

                if (updatedPhase != null)
                {
                    // Cập nhật lại UI từ dữ liệu Server trả về (không cần LoadPhase() lại nếu không cần thiết)
                    phase = updatedPhase;
                    EditMode = false;
                }
            }
            catch (ApiException ex)
            {
                // Bắt lỗi từ Server (ví dụ: Tên phase bị trùng)
                await JSRuntime.InvokeVoidAsync("alert", $"Lỗi hệ thống: {ex.Content}");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Lỗi: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/system-phases");
        }
    }
}
