using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Clients;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.Staffs;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals // Thay bằng namespace thực tế của em
{
    public partial class CreatePhaseTaskModal : ComponentBase
    {
        // Inject Service thay cho @inject
        // Thay vì dùng Mediator, em dùng Refit Client
        [Inject] private IPhaseTaskApi PhaseTaskApi { get; set; } = null!;
        [Inject] private IClientApi ClientApi { get; set; } = null!;
        [Inject] private IStaffApi StaffApi { get; set; } = null!;

        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        // Thêm Parameter ở trên
        [Parameter] public Guid ProjectId { get; set; }
        [Parameter] public Guid ProjectSystemPhaseConfigId { get; set; }
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreatePhaseTaskRequest request = new();
        private List<ClientDto> clients = new();
        private List<StaffDto> staffs = new();
        private int totalStaffs;
        private bool isLoading = false;
        private int totalClients;
        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal)
            {
                isLoading = true;
                request = new()
                {
                    ProjectId = ProjectId, // Gán vào đây
                    ProjectSystemPhaseConfigId = ProjectSystemPhaseConfigId, // Gán vào đây
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    Priority = 1
                };

                await LoadClients();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadClients()
        {
            try
            {
                var result = await ClientApi.GetAllClientsAsync(ProjectConstants.clientStartIndex, ProjectConstants.clientPageSize);
                clients = result?.Items?.ToList() ?? new();
                totalClients = result.TotalCount; // Lưu lại tổng số để hiển thị "Trang 1/10"
            }
            catch (ApiException ex) // Lỗi từ phía Server (400, 404, 500...)
            {
                // Đọc nội dung lỗi từ Server gửi về
                var errorContent = await ex.GetContentAsAsync<Dictionary<string, string>>();
                await JSRuntime.InvokeVoidAsync("alert", "Error API server: " + ex.Message);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading clients: {ex.Message}");
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

        private async Task HandleCreatePhaseTask()
        {
            try
            {
                var result = await PhaseTaskApi.CreateAsync(request);

                // Nếu tới được đây nghĩa là mã trả về là 2xx (Thành công)
                await JSRuntime.InvokeVoidAsync("alert", $"Project '{result.Name}' created successfully!");
                await OnSaved.InvokeAsync();
                await CloseModal();
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
            await OnClose.InvokeAsync();
        }
    }
}