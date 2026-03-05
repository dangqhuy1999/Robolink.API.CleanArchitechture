using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.DTOs;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.Staffs;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals
{
    public partial class EditPhaseTaskModal : ComponentBase
    {
        [Inject] private IPhaseTaskApi PhaseTaskApi { get; set; } = null!;
        [Inject] private IStaffApi StaffApi { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid PhaseTaskId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private PhaseTaskDto? phaseTask;
        private UpdatePhaseTaskRequest updateRequest = new();
        private List<StaffDto> staffs = new();
        private int totalStaffs;
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && PhaseTaskId != Guid.Empty)
            {
                isLoading = true;
                await LoadPhaseTask();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadPhaseTask()
        {
            try
            {
                phaseTask = await PhaseTaskApi.GetByIdAsync(PhaseTaskId);
                if (phaseTask != null)
                {
                    updateRequest = new UpdatePhaseTaskRequest
                    {
                        Id = phaseTask.Id,
                        Name = phaseTask.Name,
                        Description = phaseTask.Description,
                        AssignedStaffId = phaseTask.AssignedStaffId,
                        DueDate = phaseTask.DueDate,
                        Status = phaseTask.Status,
                        Priority = phaseTask.Priority,
                        EstimatedHours = phaseTask.EstimatedHours,
                        ParentPhaseTaskId = phaseTask.ParentPhaseTaskId,
                        InternalBudget = phaseTask.InternalBudget,
                        CustomerBudget = phaseTask.CustomerBudget
                    };
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
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading task: {ex.Message}");
            }
        }

        private async Task LoadManagers()
        {
            try
            {
                // Dùng biến số thay vì viết chết số 10
                var result = await StaffApi.GetAllStaffsAsync(ProjectConstants.staffStartIndex, ProjectConstants.staffPageSize);

                if (result != null)
                {
                    staffs = result.Items?.ToList() ?? new();
                    totalStaffs = result.TotalCount; // Lưu lại tổng số để hiển thị "Trang 1/10"
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
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading managers: {ex.Message}");
            }
        }

        private async Task HandleUpdatePhaseTask()
        {
            try
            {
                var result = await PhaseTaskApi.UpdateAsync(PhaseTaskId, updateRequest);

                // Alert với thông tin xịn từ DTO trả về
                await JSRuntime.InvokeVoidAsync("alert", $"Task '{result.Name}' đã cập nhật thành công!");
                await OnSaved.InvokeAsync();
                await CloseModal();
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
        
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Lỗi hệ thống: {ex.Message}");
            }
        
        }

        private async Task SelectSubPhaseTask(Guid subProjectId)
        {
            await CloseModal();
        }

        private async Task CloseModal()
        {
            phaseTask = null;
            updateRequest = new();
            await OnClose.InvokeAsync();
        }
    }
}
