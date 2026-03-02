using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.ProjectPhases;
using Robolink.Application.Queries.ProjectPhases;
using Robolink.Application.Queries.Projects;
using Robolink.Application.Queries.SystemPhases;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.ProjectPhases;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.SystemPhases;
using Robolink.WebApp.Components.Features.Projects.Pages;

namespace Robolink.WebApp.Components.Features.ProjectPhases.Pages
{
    public partial class ProjectDetails : ComponentBase
    {

        [Inject] private IProjectPhaseApi ProjectPhaseApi { get; set; } = null!;

        [Inject] private IProjectApi ProjectApi { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public Guid ProjectId { get; set; }

        private ProjectDto? CurrentProject;
        private List<ProjectPhaseConfigDto> ProjectPhases = new();

        // State variables
        private bool IsLoading = true;
        private string? ErrorMessage;
        private string? PhasesErrorMessage;

        // Modal State
        private bool showAddPhaseModal = false;

        private bool showEditConfigModal = false;
        private ProjectPhaseConfigDto? selectedConfig;

        protected override async Task OnInitializedAsync()
        {
            await LoadProjectData();
        }

        private async Task LoadProjectData()
        {
            try
            {
                IsLoading = true;
                CurrentProject = await ProjectApi.GetByIdAsync(ProjectId);

                if (CurrentProject == null)
                {
                    ErrorMessage = "Dự án không tồn tại hoặc đã bị xóa.";
                    return;
                }

                // Tải phase ngay tại đây sau khi có dữ liệu Project
                await LoadProjectPhases();
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                
                IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadProjectPhases()
        {
            try
            {
                IsLoading = true;
                var result = await ProjectPhaseApi.GetPhasesByProjectAsync(ProjectId);

                // Nhớ thêm OrderBy để Card không nhảy lộn xộn
                ProjectPhases = result?.OrderBy(x => x.Sequence).ToList() ?? new();
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                PhasesErrorMessage = ex.Message;
                Console.WriteLine($"Error loading phases: {ex}");
            }
            finally
            {
                IsLoading = false;
                StateHasChanged(); // Ép UI vẽ lại
            }
        }


        private void OpenAddPhaseModal()
        {
            showAddPhaseModal = true;
            // Hết rồi! Chỉ cần 1 dòng này thôi. Không cần gọi API ở đây nữa.
        }
        

        private async Task AssignPhase(Guid systemPhaseId)
        {
            try
            {
                var request = new AssignPhaseRequest
                {
                    ProjectId = ProjectId,
                    SystemPhaseId = systemPhaseId
                };

                var newPhase = await ProjectPhaseApi.AssignPhaseToProjectAsync(request);

                if (newPhase != null)
                {
                    // Gọi lại API để danh sách được sắp xếp đúng thứ tự Sequence
                    await LoadProjectPhases();
                }

                showAddPhaseModal = false;
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: lỗi trùng lặp từ Handler ném ra)
            }
            finally
            {
                StateHasChanged();
            }
        }
        private async Task RemovePhase(Guid phaseConfigId)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", "Bạn có chắc chắn muốn thu hồi Giai đoạn này?");
            if (!confirmed) return;

            try
            {
                var success = await ProjectPhaseApi.RemovePhaseFromProjectAsync(phaseConfigId);
                if (success)
                {   
                    // Xóa UI ngay lập tức
                    var item = ProjectPhases.FirstOrDefault(x => x.Id == phaseConfigId);
                    if (item != null) ProjectPhases.Remove(item);
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
                PhasesErrorMessage = "Không thể xóa: " + ex.Message;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }
        private async Task RefreshPhases() => await LoadProjectPhases();

        private void ViewPhaseTasks(Guid phaseId)
        {
            NavigationManager.NavigateTo($"/projects/{ProjectId}/phases/{phaseId}");
        }
    }
}
