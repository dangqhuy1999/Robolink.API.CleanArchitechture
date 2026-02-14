using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.ProjectPhases;
using Robolink.Shared.DTOs;
using Robolink.Application.Queries.ProjectPhases;
using Robolink.Application.Queries.Projects;
using Robolink.Application.Queries.SystemPhases;

namespace Robolink.WebApp.Components.Pages.ProjectPhases
{
    public partial class ProjectDetails : ComponentBase
    {
        [Inject] private IMediator Mediator { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Parameter] public Guid ProjectId { get; set; }

        private ProjectDto? CurrentProject;
        private List<ProjectPhaseConfigDto> ProjectPhases = new();
        private List<SystemPhaseDto> AvailablePhases = new();

        // State variables
        private bool IsLoading = true;
        private string? ErrorMessage;
        private string? PhasesErrorMessage;

        // Modal State
        private bool showAddPhaseModal = false;
        private bool isModalLoading = false;
        private string? modalErrorMessage;

        private bool showEditConfigModal = false;
        private ProjectPhaseConfigDto? selectedConfig;

        protected override async Task OnInitializedAsync()
        {
            await LoadProjectData();
            await LoadProjectPhases();
        }

        private async Task LoadProjectData()
        {
            try
            {
                CurrentProject = await Mediator.Send(new GetProjectByIdQuery(ProjectId));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadProjectPhases()
        {
            try
            {
                IsLoading = true;
                ProjectPhases = (await Mediator.Send(new GetProjectPhasesQuery(ProjectId)))?.ToList() ?? new();
            }
            catch (Exception ex)
            {
                PhasesErrorMessage = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OpenAddPhaseModal()
        {
            showAddPhaseModal = true;
            isModalLoading = true;
            modalErrorMessage = null;
            try
            {
                var allPhases = await Mediator.Send(new GetAllSystemPhasesQuery());
                var assignedIds = ProjectPhases.Select(p => p.SystemPhaseId).ToHashSet();
                AvailablePhases = allPhases.Where(p => !assignedIds.Contains(p.Id)).ToList();
            }
            catch (Exception ex)
            {
                modalErrorMessage = ex.Message;
            }
            finally
            {
                isModalLoading = false;
            }
        }

        private async Task AssignPhase(Guid systemPhaseId)
        {
            try
            {
                await Mediator.Send(new AssignPhaseToProjectCommand(ProjectId, systemPhaseId));
                showAddPhaseModal = false;
                await RefreshPhases();
            }
            catch (Exception ex)
            {
                modalErrorMessage = ex.Message;
            }
        }

        private async Task RefreshPhases() => await LoadProjectPhases();

        private void ViewPhaseTasks(Guid phaseId)
        {
            NavigationManager.NavigateTo($"/projects/{ProjectId}/phases/{phaseId}");
        }
    }
}
