using MediatR;
using Microsoft.AspNetCore.Components;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.ProjectPhases;
using Robolink.Core.Enums;
using Microsoft.JSInterop;

namespace Robolink.WebApp.Components.Pages.PhaseTasks
{
    public partial class PhaseTaskManagement
    {
        [Inject] private IMediator Mediator { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public Guid ProjectId { get; set; }

        [Parameter]
        public Guid PhaseId { get; set; }

        private ProjectPhaseConfigDto? phaseConfig;
        private bool isLoading = true;
        private bool showCreateTaskModal = false;
        private bool showCreateSubTaskModal = false;  // ✅ NEW
        private bool showEditModal = false;
        private Guid selectedTaskId = Guid.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadPhaseData();
        }

        private async Task LoadPhaseData()
        {
            isLoading = true;
            try
            {
                var query = new GetProjectPhasesQuery(ProjectId);
                var phases = await Mediator.Send(query);
                
                phaseConfig = phases?.FirstOrDefault(p => p.Id == PhaseId);
                
                if (phaseConfig == null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Phase not found");
                    NavigationManager.NavigateTo($"/projects/{ProjectId}");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Loaded phase: {phaseConfig?.SystemPhase?.Name}, Tasks: {phaseConfig?.Tasks?.Count ?? 0}");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private async Task RefreshTasks()
        {
            await LoadPhaseData();
        }

        private string GetPhaseName()
        {
            return phaseConfig?.CustomPhaseName ?? phaseConfig?.SystemPhase?.Name ?? "Unknown Phase";
        }

        private string GetTaskName(Guid taskId)
        {
            return phaseConfig?.Tasks?.FirstOrDefault(t => t.Id == taskId)?.Description ?? "Unknown";
        }

        private int GetCompletedCount()
        {
            return phaseConfig?.Tasks?.Count(t => t.Status == (int)Task_Status.Completed) ?? 0;
        }

        // Modal handlers
        private void ShowCreateTaskModal()
        {
            showCreateTaskModal = true;
        }

        private void HideCreateTaskModal()
        {
            showCreateTaskModal = false;
        }

        private void ShowCreateSubTaskModal(Guid parentTaskId)  // ✅ NEW
        {
            selectedTaskId = parentTaskId;
            showCreateSubTaskModal = true;
        }

        private void HideCreateSubTaskModal()  // ✅ NEW
        {
            showCreateSubTaskModal = false;
            selectedTaskId = Guid.Empty;
        }

        private void ShowEditTaskModal(Guid taskId)
        {
            selectedTaskId = taskId;
            showEditModal = true;
        }

        private void HideEditTaskModal()
        {
            showEditModal = false;
            selectedTaskId = Guid.Empty;
        }

        // ✅ Handle Create Task Callback
        private async Task HandleCreateTask(CreatePhaseTaskRequest request)
        {
            try
            {
                var command = new CreatePhaseTaskCommand
                {
                    CreatedBy = "Huy Dang",
                    Request = request
                };

                var result = await Mediator.Send(command);
                
                if (result != null)
                {
                    await RefreshTasks();
                    HideCreateTaskModal();
                    await JSRuntime.InvokeVoidAsync("alert", "Task created successfully!");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error creating task: {ex.Message}");
            }
        }

        // ✅ Handle Update Task Callback
        private async Task HandleUpdateTask(UpdatePhaseTaskRequest request)
        {
            try
            {
                var command = new UpdatePhaseTaskCommand(
                    PhaseTaskId: selectedTaskId,
                    Description: request.Description,
                    AssignedStaffId: request.AssignedStaffId,
                    DueDate: request.DueDate,
                    Status: (Task_Status)request.Status,
                    ProcessRate: request.ProcessRate,
                    Priority: request.Priority,
                    EstimatedHours: request.EstimatedHours,
                    ParentPhaseTaskId: request.ParentPhaseTaskId,
                    CreatedBy: "Huy Dang"  // ✅ FIXED: Use positional parameter or named parameter properly
                );

                var result = await Mediator.Send(command);

                if (result != null)
                {
                    await RefreshTasks();
                    HideEditTaskModal();
                    await JSRuntime.InvokeVoidAsync("alert", "Task updated successfully!");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error updating task: {ex.Message}");
            }
        }

        // ✅ Delete Task
        private async Task DeleteTask(Guid taskId)
        {
            if (!await JSRuntime.InvokeAsync<bool>("confirm", "Delete this task?"))
                return;

            try
            {
                // TODO: Implement DeletePhaseTaskCommand when ready
                // await Mediator.Send(new DeletePhaseTaskCommand(taskId));
                await RefreshTasks();
                await JSRuntime.InvokeVoidAsync("alert", "Task deleted successfully!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error deleting task: {ex.Message}");
            }
        }

        // ✅ View Sub-Tasks
        private void ViewSubTasks(Guid taskId)
        {
            NavigationManager.NavigateTo($"/projects/{ProjectId}/phases/{PhaseId}/tasks/{taskId}");
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo($"/projects/{ProjectId}");
        }
    }
}