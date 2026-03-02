using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.SystemPhases;

namespace Robolink.WebApp.Components.Features.SystemPhases.Modals
{
    public partial class CreateSystemPhaseModal : ComponentBase
    {
        [Inject] private ISystemPhaseApi SystemPhaseApi { get; set; } = null!; // ✅ NEW
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public bool ShowModal { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        private string formName = "";
        private string formDescription = "";
        private int formSequence = 1;
        private bool formIsActive = true;
        private string errorMessage = "";

        private bool isModalLoading = false;

        private async Task SavePhase()
        {
            try
            {
                errorMessage = "";
                if (string.IsNullOrWhiteSpace(formName))
                {
                    errorMessage = "Vui lòng nhập tên Giai đoạn";
                    return;
                }

                isModalLoading = true; // Hiện loading cho chuyên nghiệp

                // 🚀 Đóng gói dữ liệu vào Request DTO (Shared)
                var request = new CreateSystemPhaseRequest
                {
                    Name = formName,
                    Description = formDescription,
                    DefaultSequence = formSequence,
                    IsActive = formIsActive
                };

                // 🚀 Gọi API qua Refit
                var newPhase = await SystemPhaseApi.CreateAsync(request);

                if (newPhase != null)
                {
                    // Báo cho trang cha biết để load lại danh sách (RefreshPhases)
                    await OnSaved.InvokeAsync();
                    await OnClose.InvokeAsync();

                    // Reset form cho lần sau
                    ResetForm();
                }
            }
            catch (ApiException ex)
            {
                // Bắt lỗi Logic từ Handler (ví dụ: "Tên đã tồn tại")
                errorMessage = $"Lỗi từ hệ thống: {ex.Content}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Đã có lỗi xảy ra: {ex.Message}";
            }
            finally
            {
                isModalLoading = false;
                StateHasChanged();
            }
        }
        private void ResetForm()
        {
            formName = "";
            formDescription = "";
            formSequence = 1;
            formIsActive = true;
        }
    }
}
