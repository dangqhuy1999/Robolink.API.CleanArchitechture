using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.Projects;
using Robolink.Shared.DTOs;
using Robolink.Application.Queries.Clients;
using Robolink.Application.Queries.Staff;

namespace Robolink.WebApp.Components.Features.Projects.Modals
{
    public partial class CreateProjectModal : ComponentBase
    {
        [Inject] protected IMediator Mediator { get; set; } = null!;
        [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private CreateProjectRequest request = new();
        private List<ClientDto> clients = new();
        private List<StaffDto> managers = new();
        private bool isLoading = false;

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
                var result = await Mediator.Send(new GetAllClientsQuery());
                // 2. Lấy danh sách từ thuộc tính Items (Đây là chỗ em bị lỗi)
                clients = result?.Items?.ToList() ?? new();
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
                var result = await Mediator.Send(new GetAllStaffQuery());
                managers = result?.Items?.ToList() ?? new();
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
                var result = await Mediator.Send(new CreateProjectCommand
                {
                    Request = request,
                    CreatedBy = "Huy Dang"
                });

                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", $"Project '{result.Name}' created successfully!");
                    await OnSaved.InvokeAsync();
                    await CloseModal();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }

        private async Task CloseModal()
        {
            request = new();
            await OnClose.InvokeAsync();
        }
    }
}
