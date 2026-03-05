using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.Shared.Interfaces.API.Projects;
namespace Robolink.WebApp.Components.Features.PhaseTasks.Shared
{
    public partial class ParentPhaseTaskSelector : ComponentBase
    {
        [Inject] protected IPhaseTaskApi PhaseTaskApi { get; set; } = null!;
        [Parameter] public Guid ProjectId { get; set; } // Cha phải truyền cái này vào
        [Parameter] public Guid ProjectSystemPhaseConfigId { get; set; }   // Và cái này nữa
                                                                           // 👇 THÊM DÒNG NÀY VÀO ĐẦU CLASS
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Parameter]
        public Guid? SelectedParentId { get; set; }

        [Parameter]
        public EventCallback<Guid?> SelectedParentIdChanged { get; set; }

        [Parameter]
        public Guid? ExcludePhaseTaskId { get; set; }

        [Parameter]
        public string? CurrentParentName { get; set; }

        
        private List<PhaseTaskDto> AvailablePhaseTasks = new();

        private Guid _lastProjectId;
        private Guid _lastPhaseId;
        // Thêm biến để hứng từ khóa tìm kiếm
        private string searchTerm = string.Empty;
        private bool isLoading = false;

        private CancellationTokenSource? _searchCts;

        protected override async Task OnParametersSetAsync()
        {
            // Chỉ load lại khi ID thực sự thay đổi
            if (ProjectId != _lastProjectId || ProjectSystemPhaseConfigId != _lastPhaseId)
            {
                await RefreshPhaseTasks();
            }
        }


        private async Task OnValueChanged(Guid? value)
        {
            // Không gán trực tiếp SelectedParentId ở đây để tuân thủ Data Flow của Blazor
            await SelectedParentIdChanged.InvokeAsync(value);
        }

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
                await RefreshPhaseTasks(); // Hoặc RefreshPhaseTasks();
            }
            catch (TaskCanceledException)
            {
                // Không làm gì cả, vì user vẫn đang gõ tiếp
            }
        }
        private async Task RefreshPhaseTasks()
        {
            // 1. "Vòng gửi xe": Chưa chọn Project hoặc Phase thì nghỉ chơi luôn, không gọi API
            if (ProjectId == Guid.Empty || ProjectSystemPhaseConfigId == Guid.Empty)
            {
                AvailablePhaseTasks.Clear();
                return;
            }

            isLoading = true; // Bật hiệu ứng loading cho xịn
            try
            {
                _lastProjectId = ProjectId;
                _lastPhaseId = ProjectSystemPhaseConfigId;

                // 2. Gọi query Paged mà chị em mình đã tối ưu: 
                // Trả về tối đa 100 task của đúng Phase này, có lọc theo chữ người dùng gõ (searchTerm)
                var result = await PhaseTaskApi.GetPhaseTasksPagedAsync(
                    startIndex: 0,
                    count: 100,
                    phaseId: ProjectSystemPhaseConfigId,
                    searchTerm: searchTerm // 👈 Truyền searchTerm vào đây
                );

                // 3. LOGIC 2 TẦNG (Y hệt Project):
                // - ParentPhaseTaskId == null: Chỉ lấy Task gốc (nếu dropdown này dùng để chọn Task cha)
                // - t.Id != ExcludeTaskId: Tránh việc Task đang sửa tự chọn chính nó làm cha
                AvailablePhaseTasks = result.Items
                    .Where(t => t.ParentPhaseTaskId == null && t.Id != ExcludePhaseTaskId)
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
                isLoading = false; // Tắt loading
                StateHasChanged(); // Bắt buộc gọi để Blazor vẽ lại màn hình
            }
        }
    }
}
