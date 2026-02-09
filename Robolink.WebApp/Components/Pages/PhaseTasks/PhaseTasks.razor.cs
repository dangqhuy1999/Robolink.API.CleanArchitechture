using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Robolink.Application.Commands.PhaseTasks;
using Robolink.Application.DTOs;
using Robolink.Application.Queries.PhaseTasks;
using Robolink.WebApp.Components.Features.PhaseTasks.Shared;
using Robolink.WebApp.Components.Features.Projects.Shared;

namespace Robolink.WebApp.Components.Pages.PhaseTasks;

public partial class PhaseTasks
{
    [Inject] private IMediator Mediator { get; set; } = null!;
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
        isLoading = true;
        try
        {
            // 1. Lấy TOÀN BỘ task của Phase này (Bỏ phân trang phẳng nếu số lượng task không quá lớn > 1000)
            // Hoặc Query này phải được viết để trả về Task cha kèm SubTasks bên trong Items
            var result = await Mediator.Send(new GetPhaseTasksPagedQuery(0, 1000));
            var allItems = result.Items;

            // 2. Lọc ra danh sách cha gốc
            phaseTasks = allItems.Where(p => !p.ParentPhaseTaskId.HasValue).ToList();

            // 3. Quan trọng: Tự tay gắn con vào cha nếu Backend chưa làm
            foreach (var parent in phaseTasks)
            {
                parent.SubPhaseTasks = allItems
                    .Where(sub => sub.ParentPhaseTaskId == parent.Id)
                    .ToList();
            }

            // 4. Tính toán phân trang dựa trên danh sách CHA
            totalPhaseTasks = phaseTasks.Count;
            totalPages = (int)Math.Ceiling((double)totalPhaseTasks / pageSize);

            // Chỉ hiển thị các cha của trang hiện tại
            phaseTasks = phaseTasks.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
        catch (Exception ex) { /* handle error */ }
        finally { isLoading = false; }
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
            var result = await Mediator.Send(new CreatePhaseTaskCommand()
            {
                CreatedBy = "Huy Dang",
                Request = new CreatePhaseTaskRequest()
                {
                    Name = "New Auto Task",
                    Description = "Auto created task",

                    // Cần truyền đủ ID của Project và Phase (Lấy từ Parameter của component)
                    ProjectId = ProjectId,
                    ProjectSystemPhaseConfigId = ProjectSystemPhaseConfigId,

                    // Thông tin nhân viên (Cần Guid và Name cụ thể)
                    AssignedStaffId = PhaseTaskConstants.DefaultManagerId, // Giả sử dùng ManagerId làm StaffId
                    AssignedStaffName = "Manager Name", // Phải có vì Request yêu cầu null!

                    // Thời gian (Lưu ý: dùng DueDate thay vì Deadline)
                    StartDate = DateTime.UtcNow,
                    DueDate = DateTime.Today.AddDays(PhaseTaskConstants.DefaultPhaseTaskDurationDays),

                    // Tài chính & Trạng thái
                    InternalBudget = PhaseTaskConstants.DefaultInternalBudget,
                    CustomerBudget = PhaseTaskConstants.DefaultCustomerBudget,
                    Priority = PhaseTaskConstants.DefaultPhaseTaskPriority,
                    Status = 1, // Thêm giá trị mặc định cho Status
                    ProcessRate = 0,
                    EstimatedHours = 8, // Thêm số giờ dự kiến nếu cần

                    // Nếu là Task cha thì để null, nếu là Sub-task thì truyền ID cha vào
                    ParentPhaseTaskId = null
                }
            });
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
                await Mediator.Send(new DeletePhaseTaskCommand(taskId));
                await LoadPhaseTasks();
                await JSRuntime.InvokeVoidAsync("alert", "Task deleted successfully!");
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