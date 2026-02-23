using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.Core.Entities;
using Robolink.Shared.DTOs;
using Robolink.Shared.Interfaces.API.Clients;
using Robolink.Shared.Interfaces.API.PhaseTasks;
using Robolink.WebApp.Components.Features.PhaseTasks.Shared;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Pages.PhaseTasks;

public partial class PhaseTasks : ComponentBase
{
    
    [Inject] private IPhaseTaskApi PhaseTaskApi { get; set; } = null!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!; // ✅ NEW
    [Parameter] public Guid ProjectId { get; set; } // Cha phải truyền cái này vào
    [Parameter] public Guid ProjectSystemPhaseConfigId { get; set; }   // Và cái này nữa

    // ✅ VARIABLES
    private List<PhaseTaskDto>? phaseTasks;
    private bool isLoading = true;
    private bool showCreateModal = false;
    private bool showEditModal = false;
    private bool showQuickAddSubModal = false;
    private string? modalErrorMessage = null;
    private Guid selectedPhaseTaskId = Guid.Empty;

    // Pagination
    private int currentPage = 1;
    private int pageSize = PhaseTaskConstants.DefaultPageSize;
    private int totalPhaseTasks = 0;
    private int totalPages = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadPhaseTasks();
    }
    private void ShowTaskDetails(Guid phaseTaskId)
    {
        // Thay vì điều hướng, hãy mở Modal chỉnh sửa hoặc Modal chi tiết
        selectedPhaseTaskId = phaseTaskId;
        showEditModal = true;
    }
    // ✅ LOAD PROJECTS WITH PAGINATION
    private async Task LoadPhaseTasks()
    {
        if (ProjectSystemPhaseConfigId == Guid.Empty) return;

        isLoading = true;
        try
        {
            // 1. Lấy đúng Task của Phase này (startIndex=0, count=1000 để lấy đủ cha con)
            

            var result = await PhaseTaskApi.GetPhaseTasksPagedAsync(0, 1000, ProjectSystemPhaseConfigId);
            var allItems = result?.Items?.ToList() ?? new();

            // 2. Lọc danh sách Cha
            // Lúc này allItems đã sạch, chỉ toàn task của Phase này nên không sợ bị lẫn task phase khác
            var allParentTasks = allItems.Where(p => !p.ParentPhaseTaskId.HasValue).ToList();

            // 3. Gán con vào cha (Nếu Mapping chưa tự làm)
            foreach (var parent in allParentTasks)
            {
                parent.SubPhaseTasks = allItems
                    .Where(sub => sub.ParentPhaseTaskId == parent.Id)
                    .ToList();
            }

            // 4. Thực hiện phân trang trên danh sách Cha
            totalPhaseTasks = allParentTasks.Count;
            totalPages = (int)Math.Ceiling((double)totalPhaseTasks / pageSize);

            phaseTasks = allParentTasks
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }
        catch (Exception ex) 
        { 
            modalErrorMessage = ex.Message; // Gán lỗi để hiển thị lên UI
        }
        finally 
        { 
            isLoading = false; 
        }
    }

    // ✅ GO TO PAGE
    private async Task GoToPage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
            await LoadPhaseTasks();
        }
    }

    
    private void BackToPhases()
    {
        // Điều hướng quay lại trang chi tiết Project (nơi hiển thị danh sách các Phase)
        NavigationManager.NavigateTo($"/projects/{ProjectId}");
    }
    // ✅ QUICK ADD PROJECT
    private async Task QuickAddPhaseTask()
    {
        try
        { 
            var result = await PhaseTaskApi.QuickCreateAsync( ProjectId, ProjectSystemPhaseConfigId); 
            if (result != null)
            {
                currentPage = 1;
                await LoadPhaseTasks();
                await JSRuntime.InvokeVoidAsync("alert", "Task created successfully!");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating quick Task: {ex}");
            await JSRuntime.InvokeVoidAsync("alert", $"Error creating Task: {ex.Message}");
        }
    }

    // ✅ QUICK ADD SUB-PROJECT
    private void ShowQuickAddSubModal(Guid parentPhaseTaSKId)
    {
        selectedPhaseTaskId = parentPhaseTaSKId;
        showQuickAddSubModal = true;
    }

    private void HideQuickAddSubModal()
    {
        showQuickAddSubModal = false;
        selectedPhaseTaskId = Guid.Empty;
    }

    // ✅ MODAL METHODS
    private void ShowCreateModal() => showCreateModal = true;

    private void HideCreateModal()
    {
        showCreateModal = false;
        selectedPhaseTaskId = Guid.Empty;
    }

    private void ShowEditModal(Guid projectId)
    {
        selectedPhaseTaskId = projectId;
        showEditModal = true;
    }

    private void HideEditModal()
    {
        showEditModal = false;
        selectedPhaseTaskId = Guid.Empty;
    }

    // ✅ DELETE PROJECT
    private async Task DeletePhaseTask(Guid taskId)
    {
        if (await JSRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete this task?"))
        {
            try
            {
                // ✅ Gọi qua API thay vì gọi trực tiếp Mediator
                var result = await PhaseTaskApi.DeleteAsync(taskId);
                if (result) // Nếu Handler trả về true
                {
                    await LoadPhaseTasks();
                    await JSRuntime.InvokeVoidAsync("alert", "Task deleted successfully!");
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Delete failed: Task might not exist.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting task: {ex}");
                await JSRuntime.InvokeVoidAsync("alert", $"Error deleting task: {ex.Message}");
            }
        }
    }

    // ✅ SETTINGS (TODO: Implement in future)
    private async Task ShowSettings()
    {
        await JSRuntime.InvokeVoidAsync("alert", "Settings feature coming soon!");
    }
}