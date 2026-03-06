using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Modules.ProjectManagement.Features.Projects.Components.Tables
{
    public partial class ProjectTable : ComponentBase
    {
        /// <summary>
        /// Toggle expand/collapse for a row
        /// </summary>
        private async Task HandleToggleRow(Guid projectId)
        {
            if (ExpandedProjects.Contains(projectId))
                ExpandedProjects.Remove(projectId);
            else
                ExpandedProjects.Add(projectId);

            StateHasChanged();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Handle edit button click
        /// </summary>
        private async Task HandleEdit(Guid projectId)
        {
            await OnEdit.InvokeAsync(projectId);
        }

        /// <summary>
        /// Handle delete button click
        /// </summary>
        private async Task HandleDelete(Guid projectId)
        {
            await OnDelete.InvokeAsync(projectId);
        }

        /// <summary>
        /// Handle add sub-project button click
        /// </summary>
        private async Task HandleAddSubProject(Guid projectId)
        {
            await OnAddSubProject.InvokeAsync(projectId);
        }

        /// <summary>
        /// Handle view project button click
        /// </summary>
        private async Task HandleViewProject(Guid projectId)
        {
            await OnViewProject.InvokeAsync(projectId);
        }

        /// <summary>
        /// Handle sort column click
        /// </summary>
        private async Task HandleSort(string columnName)
        {
            if (SortColumn == columnName)
            {
                // Toggle sort direction
                SortDirection = SortDirection == "asc" ? "desc" : "asc";
            }
            else
            {
                // Change column, reset to ascending
                SortColumn = columnName;
                SortDirection = "asc";
            }

            StateHasChanged();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Clear all expanded rows
        /// </summary>
        private async Task HandleCollapseAll()
        {
            ExpandedProjects.Clear();
            StateHasChanged();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Expand all rows
        /// </summary>
        private async Task HandleExpandAll()
        {
            foreach (var project in Projects)
            {
                ExpandedProjects.Add(project.Id);
            }
            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}
