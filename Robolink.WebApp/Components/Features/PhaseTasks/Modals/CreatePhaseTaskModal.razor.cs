using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.Clients;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.Staff;
using System.Xml.Linq;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals // Thay bằng namespace thực tế của em
{
    public partial class CreatePhaseTaskModal : ComponentBase
    {
        // Inject Service thay cho @inject
        [Inject] private IMediator Mediator { get; set; } = null!;
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
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal)
            {
                isLoading = true;
                request = new()
                {
                    ProjectId = ProjectId, // Gán vào đây
                    ProjectSystemPhaseConfigId = ProjectSystemPhaseConfigId, // Gán vào đây
                    StartDate = DateTime.Today,
                    DueDate = DateTime.Today.AddDays(30),
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
                clients = result?.ToList() ?? new();
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
                staffs = result?.ToList() ?? new();
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
                var result = await Mediator.Send(new CreatePhaseTaskCommand
                {
                    Request = request,
                    CreatedBy = "Huy Dang"
                });

                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", $"The task is created successfully!");
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