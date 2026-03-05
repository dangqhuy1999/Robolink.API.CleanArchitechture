using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Application.Queries.ProjectPhases;
using Robolink.Application.Queries.SystemPhases;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.SystemPhases;
using Robolink.WebApp.Components.Features.Projects.Pages;

namespace Robolink.WebApp.Components.Features.SystemPhases.Pages
{
    public partial class SystemPhases
    {
        [Inject] private ISystemPhaseApi SystemPhaseApi { get; set; } = null!; // ✅ NEW
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!; // ✅ NEW

        private List<SystemPhaseDto>? allPhases;

        private Dictionary<Guid, int> phaseUsageCount = new();
        private bool isLoading = true;
        private bool showCreateModal = false;
        private bool showEditModal = false;
        private Guid selectedPhaseId = Guid.Empty;

        // ✅ PAGINATION
        private int currentPage = 1;
        private int pageSize = 5;
        private int totalPhases = 0;
        private int totalPages = 0;

        protected override async Task OnInitializedAsync()
        {
            await RefreshPhases();
        }

        
        private async Task RefreshPhases()
        {
            isLoading = true;
            try
            {
                // Tính toán vị trí dựa trên currentPage (đã được GoToPage cập nhật)
                int startIndex = (currentPage - 1) * pageSize;

                // Lấy đúng 10 cái từ Server
                var result = await SystemPhaseApi.GetSystemPhasesPagedAsync(startIndex, pageSize);

                allPhases = result.Items.ToList();
                totalPhases = result.TotalCount;
                totalPages = (int)Math.Ceiling((double)totalPhases / pageSize);

                await CalculatePhaseUsage();
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading projects: {ex}");
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading projects: {ex.Message}");
                allPhases = new List<SystemPhaseDto>();
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task GoToPage(int page)
        {
            // 1. Chặn nếu trang bấm vào không hợp lệ
            if (page < 1 || page > totalPages || page == currentPage) return;

            // 2. Cập nhật trang hiện tại
            currentPage = page;

            // 3. Gọi Server lấy 10 ông tiếp theo
            await RefreshPhases();
        }

        // ✅ NEW: NAVIGATE TO PROJECT MANAGEMENT (LEVEL 2)
        private void NavigateToViewPhaseManagement(Guid phaseId)
        {
            NavigationManager.NavigateTo($"/systemphases/{phaseId}");
        }
        private async Task CalculatePhaseUsage()
        {
            phaseUsageCount.Clear();
            
            if (allPhases == null) return;

            foreach (var phase in allPhases)
            {
                // TODO: Query database for actual usage count
                phaseUsageCount[phase.Id] = 0;
            }
        }

        private void ShowCreateModal()
        {
            showCreateModal = true;
        }

        private void HideCreateModal()
        {
            showCreateModal = false;
        }

        private void ShowEditModal(Guid phaseId)
        {
            selectedPhaseId = phaseId;
            showEditModal = true;
        }

        private void HideEditModal()
        {
            showEditModal = false;
            selectedPhaseId = Guid.Empty;
        }

        private async Task DeletePhase(Guid phaseId)
        {
            // Tìm phase trong list hiện tại để lấy thông tin
            var systemPhaseToDelete = allPhases?.FirstOrDefault(p => p.Id == phaseId);
            if (systemPhaseToDelete == null) return;

            string message = $"Bạn có chắc chắn muốn xóa phase '{systemPhaseToDelete.Name}' không?";
            // Hiện Confirm (Có thể dùng SweetAlert2 thay cho confirm này)
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", message);

            if (confirmed)
            {
                try
                {
                    await SystemPhaseApi.DeleteAsync(phaseId);
                    // Refresh lại danh sách
                    await RefreshPhases();
                }
                catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
                {
                    // Đọc nội dung lỗi từ Server gửi về
                    var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                    await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Lỗi khi xóa: " + ex.Message);
                }
            }
            
        }
    }
}