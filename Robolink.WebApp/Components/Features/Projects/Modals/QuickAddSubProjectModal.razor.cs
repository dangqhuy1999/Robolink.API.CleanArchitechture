using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Queries.Projects;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.Staffs;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Features.Projects.Modals
{
    public partial class QuickAddSubProjectModal : ComponentBase
    {
        [Inject] private IProjectApi ProjectApi { get; set; } = null!;
        [Inject] private IStaffApi StaffApi { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid ParentProjectId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreateProjectRequest request = new();
        private ProjectDto? parentProject;
        private List<StaffDto> managers = new();
        private int totalStaffs;
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && ParentProjectId != Guid.Empty)
            {
                isLoading = true;
                request = new()
                {
                    StartDate = DateTime.Today,
                    Deadline = DateTime.Today.AddDays(30),
                    Priority = 1
                };

                await LoadParentProject();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadParentProject()
        {
            if (ParentProjectId == Guid.Empty) return; // Chặn ngay từ đầu cho chắc

            try
            {
                // Gọi Handler "vũ khí mới" của em
                parentProject = await ProjectApi.GetByIdAsync(ParentProjectId);

                if (parentProject != null)
                {
                    // ✅ GIỮ LẠI: Đây là Logic nghiệp vụ (Inheritance)
                    // Khi chọn dự án cha, con phải theo Client của cha
                    request.ClientId = parentProject.ClientId;
                    request.ParentProjectId = ParentProjectId;

                    // Em có thể gán thêm các thứ khác nếu muốn kế thừa từ cha
                    // Ví dụ: request.Priority = parentProject.Priority;
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
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading parent project: {ex.Message}");
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
                    managers = result.Items?.ToList() ?? new();
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

        private async Task HandleCreateSubProject()
        {
            try
            {
                var result = await ProjectApi.CreateAsync(request);
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
            parentProject = null;
            await OnClose.InvokeAsync();
        }
    }
}
