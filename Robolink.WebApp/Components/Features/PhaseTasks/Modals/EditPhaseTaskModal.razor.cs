using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Shared.DTOs;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Application.Queries.Staff;
using Robolink.Shared.Enums;
using Robolink.Shared.Interfaces.API.Staffs;
using Robolink.Shared.Interfaces.API.PhaseTasks;

namespace Robolink.WebApp.Components.Features.PhaseTasks.Modals
{
    public partial class EditPhaseTaskModal : ComponentBase
    {
        [Inject] private IPhaseTaskApi PhaseTaskApi { get; set; } = null!;
        [Inject] private IStaffApi StaffApi { get; set; } = null!;
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
                phaseTask = await PhaseTaskApi.GetByIdAsync(PhaseTaskId);
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
                var result = await StaffApi.GetAllStaffsAsync();
                staffs = result?.Items?.ToList() ?? new();
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
                var result = await PhaseTaskApi.UpdateAsync(PhaseTaskId,updateRequest);

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
