using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Tables
{
    public partial class ProjectTable : ComponentBase
    {
        // ✅ NEW: Handle View/Navigate to Project Management
        private async Task HandleViewProject(Guid projectId)
        {
            await OnViewProject.InvokeAsync(projectId);
        }

        private async Task HandleEdit(Guid projectId)
        {
            await OnEdit.InvokeAsync(projectId);
        }

        private async Task HandleDelete(Guid projectId)
        {
            await OnDelete.InvokeAsync(projectId);
        }

        private async Task HandleAddSub(Guid projectId)
        {
            await OnAddSubProject.InvokeAsync(projectId);
        }

        private void HandleToggleExpand(Guid projectId)
        {
            if (expandedProjects.Contains(projectId))
            {
                expandedProjects.Remove(projectId);
            }
            else
            {
                expandedProjects.Add(projectId);
            }
        }
    }
}
