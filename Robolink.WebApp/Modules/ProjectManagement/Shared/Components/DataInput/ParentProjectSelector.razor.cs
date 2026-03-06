using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web; // Thêm cái này để dùng KeyboardEventArgs
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Queries.Projects;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.WebApp.Components.Features.Projects.Pages;    

namespace Robolink.WebApp.Components.Features.Projects.Shared
{
    public partial class ParentProjectSelector : ComponentBase
    {
        [Inject] protected IProjectApi ProjectApi { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;

        [Parameter] public Guid? SelectedParentId { get; set; }
        [Parameter] public EventCallback<Guid?> SelectedParentIdChanged { get; set; }
        [Parameter] public Guid? ExcludeProjectId { get; set; }
        [Parameter] public string? CurrentParentName { get; set; }

        private List<ProjectDto> AvailableProjects = new();

        // Thêm biến để hứng từ khóa tìm kiếm
        private string searchTerm = string.Empty;
        private bool isLoading = false;

        private CancellationTokenSource? _searchCts;

        protected override async Task OnInitializedAsync()
        {
            await RefreshProjects();
        }

        private async Task OnValueChanged(Guid? value)
        {
            SelectedParentId = value;
            await SelectedParentIdChanged.InvokeAsync(value);
        }

        // Bắt sự kiện người dùng gõ phím
        private async Task HandleSearch(KeyboardEventArgs e)
        {
            // 1. Hủy bỏ đợt tìm kiếm trước đó nếu user vẫn đang gõ
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                // 2. Chờ 500ms
                await Task.Delay(500, _searchCts.Token);

                // 3. Nếu sau 500ms mà không bị Cancel (nghĩa là user đã ngừng gõ) thì mới gọi API
                await RefreshProjects(); // Hoặc RefreshPhaseTasks();
            }
            catch (TaskCanceledException)
            {
                // Không làm gì cả, vì user vẫn đang gõ tiếp
            }
        }

        private async Task RefreshProjects()
        {
            isLoading = true;
            try
            {
                // 1. Gọi API: Lấy 100 ông khớp với searchTerm nhất (nếu searchTerm rỗng thì lấy 100 ông mới nhất)
                var result = await ProjectApi.GetProjectsPagedAsync(0, 100, searchTerm);

                // 2. LOGIC 2 TẦNG: Chỉ lấy những ông đang là ROOT (ParentId == null) 
                // và phải khác cái dự án đang sửa (tránh tự nhận mình làm cha)
                AvailableProjects = result.Items
                    .Where(p => p.ParentProjectId == null && p.Id != ExcludeProjectId)
                    .ToList();
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", "Không thể làm mới dữ liệu!");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }
    }
}