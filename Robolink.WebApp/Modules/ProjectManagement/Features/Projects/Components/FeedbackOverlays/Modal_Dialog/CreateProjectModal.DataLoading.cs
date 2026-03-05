using Microsoft.AspNetCore.Components;
using Robolink.WebApp.Shared.Services.Staffs;
using Robolink.WebApp.Shared.Services.Clients;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Mappers;
using Robolink.Shared.Enums;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Modals;

/// <summary>
/// Handles all API calls and data loading for CreateProjectModal.
/// Separates data fetching from UI logic.
/// </summary>
public partial class CreateProjectModal : ComponentBase
{
    // ===== INJECTED SERVICES =====
    [Inject] private IClientApi ClientApi { get; set; } = null!;
    [Inject] private IStaffApi StaffApi { get; set; } = null!;
    [Inject] private ILogger<CreateProjectModal> Logger { get; set; } = null!;

    /// <summary>
    /// Loads all required data for the modal (clients and managers).
    /// Called when modal is shown.
    /// </summary>
    private async Task LoadModalDataAsync()
    {
        try
        {
            State.IsLoadingData = true;
            Logger.LogInformation("Loading modal data for CreateProjectModal");

            // Load clients and managers concurrently
            await Task.WhenAll(
                LoadClientsAsync(),
                LoadManagersAsync()
            );

            Logger.LogInformation("Modal data loaded successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading modal data");
            State.LastErrorMessage = "Failed to load data. Please try again.";
        }
        finally
        {
            State.IsLoadingData = false;
        }
    }

    /// <summary>
    /// Loads available clients from API.
    /// </summary>
    private async Task LoadClientsAsync()
    {
        try
        {
            var result = await ClientApi.GetAllClientsAsync(0, 100);
            State.AvailableClients = ProjectUiMapper.ToClientViewModels(result?.Items ?? []);

            Logger.LogInformation("Clients loaded. Count: {Count}", State.AvailableClients.Count);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error loading clients");
            State.AvailableClients = [];
        }
    }

    /// <summary>
    /// Loads available managers from API.
    /// </summary>
    private async Task LoadManagersAsync()
    {
        try
        {
            var result = await StaffApi.GetAllStaffsAsync(0, 100);
            State.AvailableManagers = ProjectUiMapper.ToManagerViewModels(result?.Items ?? []);

            Logger.LogInformation("Managers loaded. Count: {Count}", State.AvailableManagers.Count);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error loading managers");
            State.AvailableManagers = [];
        }
    }

    /// <summary>
    /// Initializes form with default values.
    /// </summary>
    private void InitializeFormData()
    {
        State.FormData = new()
        {
            StartDate = DateTime.Today,
            Deadline = DateTime.Today.AddDays(30),
            Priority = ProjectPriority.Medium ,
            ParentProjectId = ParentProjectId
        };

        Logger.LogInformation("Form initialized. IsSubProject: {IsSubProject}", 
            State.FormData.IsSubProject);
    }
}