using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.Staffs;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals
{
    public partial class QuickAddSubPhaseTaskModal : ComponentBase
    {
        [Inject] private IPhaseTaskApi PhaseTaskApi { get; set; } = null!;
        [Inject] private IStaffApi StaffApi { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid ParentPhaseTaskId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreatePhaseTaskRequest request = new();
        private PhaseTaskDto? parentPhaseTask;
        private List<StaffDto> staffs = new();
        private int totalStaffs;
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && ParentPhaseTaskId != Guid.Empty)
            {
                isLoading = true;
                request = new()
                {
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    Priority = 1
                };

                await LoadParentPhaseTask();
                await LoadManagers();
                isLoading = false;
            }
        }


        private async Task LoadParentPhaseTask()
        {
            try
            {
                // Gọi Handler "vũ khí mới" của em
                parentPhaseTask = await PhaseTaskApi.GetByIdAsync(ParentPhaseTaskId);

                if (parentPhaseTask != null)
                {
                    // Inherit client and some settings from parent
                    request.AssignedStaffId = parentPhaseTask.AssignedStaffId;
                    request.ParentPhaseTaskId = ParentPhaseTaskId;

                    // PHẢI CÓ 2 DÒNG NÀY:
                    request.ProjectId = parentPhaseTask.ProjectId;
                    request.ProjectSystemPhaseConfigId = parentPhaseTask.ProjectSystemPhaseConfigId;
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
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading parent task: {ex.Message}");
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

        private async Task HandleCreateSubPhaseTask()
        {
            try
            {
                var result = await PhaseTaskApi.CreateAsync(request);
                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", $"Sub-project '{result.Name}' created successfully!");
                    await OnSaved.InvokeAsync();
                    await CloseModal();
                }
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex) // Lỗi mạng hoặc lỗi code
            {
                await JSRuntime.InvokeVoidAsync("alert", "Lỗi hệ thống: " + ex.Message);
            }
        }

        private async Task CloseModal()
        {
            request = new();
            parentPhaseTask = null;
            await OnClose.InvokeAsync();
        }
    }
}
