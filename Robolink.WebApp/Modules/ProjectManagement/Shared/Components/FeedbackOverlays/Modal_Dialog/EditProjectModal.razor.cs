using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Shared.DTOs;
using Robolink.WebApp.Components.Features.Projects.Shared;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Services;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Components.Features.Projects.Modals
{
    public partial class EditProjectModal : ComponentBase
    {
        [Inject] private IProjectService ProjectService { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        // ✅ VARIABLES
        private ProjectViewModel? project;
        private UpdateProjectRequest updateRequest = new();
        private List<StaffViewModel> managers = new();
        private int totalStaffs;
        private bool isLoading = false; 

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && ProjectId != Guid.Empty)
            {
                isLoading = true;
                await LoadProject();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadProject()
        {
            try
            {
                // Gọi hàm GetById thay vì GetPaged
                project = await ProjectApi.GetByIdAsync(ProjectId);

                if (project != null)
                {
                    // Đổ dữ liệu từ project sang updateRequest để bind vào Form
                    updateRequest = new UpdateProjectRequest
                    {
                        Name = project.Name,
                        Description = project.Description,
                        ManagerId = project.ManagerId,
                        Status = project.Status,
                        ParentProjectId = project.ParentProjectId,
                        Deadline = project.Deadline,
                        Priority = project.Priority,
                        InternalBudget = project.InternalBudget,
                        CustomerBudget = project.CustomerBudget
                    };
                }
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                System.Diagnostics.Debug.WriteLine($"API Error loading project: {ex}");
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading project: {ex}");
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading project: {ex.Message}");
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

        private async Task HandleUpdateProject()
        {
            try
            {
                // 🚀 BƯỚC QUAN TRỌNG: Gán ID vào Request Body để thỏa mãn Validation
                updateRequest.Id = ProjectId;

                var result = await ProjectApi.UpdateAsync(ProjectId, updateRequest);

                // Alert với thông tin xịn từ DTO trả về
                await JSRuntime.InvokeVoidAsync("alert", $"Dự án '{result.Name}' đã cập nhật thành công!");
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

        private async Task SelectSubProject(Guid subProjectId)
        {
            // Close current modal and open the sub-project
            await CloseModal();
            // This would be handled by parent component
        }

        private async Task CloseModal()
        {
            project = null;
            updateRequest = new();
            await OnClose.InvokeAsync();
        }
    }
}
