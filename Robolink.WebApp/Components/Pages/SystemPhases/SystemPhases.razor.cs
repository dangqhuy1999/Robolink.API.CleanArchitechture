using MediatR;
using Microsoft.AspNetCore.Components;
using Robolink.Application.Commands.SystemPhases;
using Robolink.Shared.DTOs;
using Robolink.Application.Queries.SystemPhases;
using Robolink.Application.Queries.ProjectPhases;
using Microsoft.JSInterop;

namespace Robolink.WebApp.Components.Pages.SystemPhases
{
    public partial class SystemPhases
    {
        [Inject] private IMediator Mediator { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        private List<SystemPhaseDto>? allPhases;
        private List<SystemPhaseDto>? pagedPhases;
        private Dictionary<Guid, int> phaseUsageCount = new();
        private bool isLoading = true;
        private bool showCreateModal = false;
        private bool showEditModal = false;
        private Guid selectedPhaseId = Guid.Empty;

        // ✅ PAGINATION
        private int currentPage = 1;
        private int pageSize = 5;
        private int totalPhases = 0;
        private int totalPages = 0;

        protected override async Task OnInitializedAsync()
        {
            await RefreshPhases();
        }

        private async Task RefreshPhases()
        {
            isLoading = true;
            try
            {
                var query = new GetAllSystemPhasesQuery();
                allPhases = (await Mediator.Send(query))?.ToList();
                
                totalPhases = allPhases?.Count ?? 0;
                totalPages = (int)Math.Ceiling((double)totalPhases / pageSize);
                
                await CalculatePhaseUsage();
                GoToPage(1);
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

        // ✅ PAGINATION LOGIC
        private void GoToPage(int page)
        {
            currentPage = Math.Max(1, Math.Min(page, totalPages));
            
            if (allPhases != null)
            {
                pagedPhases = allPhases
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
        }

        private async Task CalculatePhaseUsage()
        {
            phaseUsageCount.Clear();
            
            if (allPhases == null) return;

            foreach (var phase in allPhases)
            {
                // TODO: Query database for actual usage count
                phaseUsageCount[phase.Id] = 0;
            }
        }

        private void ShowCreateModal()
        {
            showCreateModal = true;
        }

        private void HideCreateModal()
        {
            showCreateModal = false;
        }

        private void ShowEditModal(Guid phaseId)
        {
            selectedPhaseId = phaseId;
            showEditModal = true;
        }

        private void HideEditModal()
        {
            showEditModal = false;
            selectedPhaseId = Guid.Empty;
        }

        private async Task DeletePhase(Guid phaseId)
        {
            bool confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
                "Are you sure you want to delete this phase?");
            
            if (!confirmed)
                return;

            try
            {
                // ✅ NEW: Send command with soft delete (default)
                var command = new DeleteSystemPhaseCommand(phaseId, HardDelete: false);
                await Mediator.Send(command);
                
                await RefreshPhases();
                await JSRuntime.InvokeVoidAsync("alert", "Phase deleted successfully!");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Error: {ex.Message}");
            }
        }
    }
}