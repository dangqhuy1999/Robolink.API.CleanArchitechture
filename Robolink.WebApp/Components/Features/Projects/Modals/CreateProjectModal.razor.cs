using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;
using Robolink.Application.Commands.Projects;
using Robolink.Application.Queries.Clients;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Clients;
using Robolink.Shared.Interfaces.API.Projects;
using Robolink.Shared.Interfaces.API.Staffs;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Features.Projects.Modals
{
    public partial class CreateProjectModal : ComponentBase
    {
        [Inject] private IProjectApi ProjectApi { get; set; } = null!;
        [Inject] private IClientApi ClientApi { get; set; } = null!;
        [Inject] private IStaffApi StaffApi { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreateProjectRequest request = new();
        private List<ClientDto> clients = new();
        private List<StaffDto> managers = new();
        private bool isLoading = false;
        private int totalClients;
        private int totalStaffs;
        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal)
            {
                isLoading = true;
                request = new()
                {
                    StartDate = DateTime.Today,
                    Deadline = DateTime.Today.AddDays(30),
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
                // Dùng biến số thay vì viết chết số 10
                var result = await ClientApi.GetAllClientsAsync(ProjectConstants.clientStartIndex, ProjectConstants.clientPageSize);

                if (result != null)
                {
                    clients = result.Items?.ToList() ?? new();
                    totalClients = result.TotalCount; // Lưu lại tổng số để hiển thị "Trang 1/10"
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

        private async Task HandleCreateProject()
        {
            try
            {
                var result = await ProjectApi.CreateAsync(request);

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
        private bool IsSubProject =>
            request?.ParentProjectId.HasValue == true &&
            request.ParentProjectId != Guid.Empty;

        private string ModalTitle =>
            IsSubProject ? "Create Sub-Project" : "Create Project";
    }
}
