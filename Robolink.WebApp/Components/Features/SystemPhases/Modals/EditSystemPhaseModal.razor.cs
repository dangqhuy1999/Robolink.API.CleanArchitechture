using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Application.Queries.SystemPhases;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.SystemPhases;

namespace Robolink.WebApp.Components.Features.SystemPhases.Modals
{
    public partial class EditSystemPhaseModal : ComponentBase
    {
        [Inject] private ISystemPhaseApi SystemPhaseApi { get; set; } = null!; // ✅ NEW
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter]
        public bool ShowModal { get; set; }

        [Parameter]
        public Guid PhaseId { get; set; }

        [Parameter]
        public EventCallback OnClose { get; set; }

        [Parameter]
        public EventCallback OnSaved { get; set; }

        private SystemPhaseDto? phase;
        private string formName = "";
        private string formDescription = "";
        private int formSequence = 1;
        private bool formIsActive = true;
        private string errorMessage = "";

        private bool isLoading = false;
        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && PhaseId != Guid.Empty)
            {
                await LoadPhase();
            }
        }

        private async Task LoadPhase()
        {
            try
            {
                // 🚀 Gọi API: Client bắn Request lên Server -> Controller -> Handler
                phase = await SystemPhaseApi.GetByIdAsync(PhaseId);
                if (phase != null)
                {
                    formName = phase.Name;
                    formDescription = phase.Description ?? "";
                    formSequence = phase.DefaultSequence;
                    formIsActive = phase.IsActive;
                }
            }
            catch (ApiException ex) // Lỗi trả về từ Server (ví dụ: 400 Bad Request, 404...)
            {
                // Đọc nội dung lỗi chi tiết từ Server nếu có ném ra Exception ở Handler
                errorMessage = $"Lỗi từ hệ thống: {ex.Content}";
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        private async Task SavePhase()
        {
            try
            {
                errorMessage = "";
                isLoading = true; // Thêm cái này cho User biết là máy đang "nổ máy"

                // 1. Đóng gói dữ liệu vào Request DTO (Shared) cho đúng chuẩn API
                var request = new UpdateSystemPhaseRequest
                {
                    Id = PhaseId,
                    Name = formName,
                    Description = formDescription,
                    DefaultSequence = formSequence,
                    IsActive = formIsActive
                };

                // 2. 🚀 GỌI API (Refit): Không dùng Mediator ở đây nữa nhé!
                // Client bắn Request -> API Controller -> Mediator (ở Server) -> Handler
                var updatedDto = await SystemPhaseApi.UpdateAsync(PhaseId, request);

                if (updatedDto != null)
                {
                    // 3. Thành công thì báo cho trang cha load lại list và đóng Modal
                    await OnSaved.InvokeAsync();
                    await OnClose.InvokeAsync();
                }
            }
            catch (ApiException ex) // Lỗi trả về từ Server (ví dụ: 400 Bad Request, 404...)
            {
                // Đọc nội dung lỗi chi tiết từ Server nếu có ném ra Exception ở Handler
                errorMessage = $"Lỗi từ hệ thống: {ex.Content}";
            }
            catch (Exception ex)
            {
                errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}
