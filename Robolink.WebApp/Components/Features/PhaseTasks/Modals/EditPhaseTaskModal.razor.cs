using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Shared.DTOs;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals
{
    public partial class EditPhaseTaskModal : ComponentBase
    {
        [Inject] private IMediator Mediator { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Parameter] public bool ShowModal { get; set; }
        [Parameter] public Guid PhaseTaskId { get; set; }
        [Parameter] public EventCallback OnClose { get; set; }
        [Parameter] public EventCallback OnSaved { get; set; }

        private PhaseTaskDto? phaseTask;
        private UpdatePhaseTaskRequest updateRequest = new();
        private List<StaffDto> staffs = new();
        private bool isLoading = false;

        protected override async Task OnParametersSetAsync()
        {
            if (ShowModal && PhaseTaskId != Guid.Empty)
            {
                isLoading = true;
                await LoadPhaseTask();
                await LoadManagers();
                isLoading = false;
            }
        }

        private async Task LoadPhaseTask()
        {
            try
            {
                phaseTask = await Mediator.Send(new GetPhaseTaskByIdQuery(PhaseTaskId));
                if (phaseTask != null)
                {
                    updateRequest = new UpdatePhaseTaskRequest
                    {
                        Id = phaseTask.Id,
                        Name = phaseTask.Name,
                        Description = phaseTask.Description,
                        AssignedStaffId = phaseTask.AssignedStaffId,
                        DueDate = phaseTask.DueDate,
                        Status = phaseTask.Status,
                        Priority = phaseTask.Priority,
                        EstimatedHours = phaseTask.EstimatedHours,
                        ParentPhaseTaskId = phaseTask.ParentPhaseTaskId,
                        InternalBudget = phaseTask.InternalBudget,
                        CustomerBudget = phaseTask.CustomerBudget
                    };
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error loading project: {ex.Message}");
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

        private async Task HandleUpdatePhaseTask()
        {
            try
            {
                var result = await Mediator.Send(new UpdatePhaseTaskCommand
                {
                    Id = updateRequest.Id,
                    Name = updateRequest.Name ?? "",
                    Description = updateRequest.Description ?? "",
                    AssignedStaffId = updateRequest.AssignedStaffId ?? Guid.Empty,
                    DueDate = updateRequest.DueDate ?? DateTime.Now,
                    Status = updateRequest.Status ?? Task_Status.Cancelled,
                    Priority = updateRequest.Priority ?? 0,
                    EstimatedHours = updateRequest.EstimatedHours ?? 0,
                    ParentPhaseTaskId = updateRequest.ParentPhaseTaskId,
                    CreatedBy = "Huy Dang"
                });

                if (result != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Task updated successfully!");
                    await OnSaved.InvokeAsync();
                    await CloseModal();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }

        private async Task SelectSubPhaseTask(Guid subProjectId)
        {
            await CloseModal();
        }

        private async Task CloseModal()
        {
            phaseTask = null;
            updateRequest = new();
            await OnClose.InvokeAsync();
        }
    }
}
