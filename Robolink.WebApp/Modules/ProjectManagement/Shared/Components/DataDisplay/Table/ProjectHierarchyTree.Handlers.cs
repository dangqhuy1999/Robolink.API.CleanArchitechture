using Microsoft.AspNetCore.Components;

namespace Robolink.WebApp.Components.Features.Projects.Tables
{
    public partial class ProjectHierarchyTree : ComponentBase
    {
        /// <summary>
        /// Toggle expand/collapse state for a node
        /// Called when user clicks chevron icon
        /// </summary>
        private async Task ToggleNode(Guid projectId)
        {
            if (ExpandedNodes.Contains(projectId))
            {
                ExpandedNodes.Remove(projectId);
                System.Diagnostics.Debug.WriteLine(
                    $"[ProjectHierarchyTree] Collapsed node: {projectId}");
            }
            else
            {
                ExpandedNodes.Add(projectId);
                System.Diagnostics.Debug.WriteLine(
                    $"[ProjectHierarchyTree] Expanded node: {projectId}");
            }

            // Trigger re-render
            StateHasChanged();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Collapse all nodes
        /// </summary>
        private async Task CollapseAll()
        {
            ExpandedNodes.Clear();
            StateHasChanged();
            await Task.CompletedTask;
        }

        /// <summary>
        /// Expand all nodes
        /// </summary>
        private async Task ExpandAll()
        {
            foreach (var project in AllProjects ?? new())
            {
                ExpandedNodes.Add(project.Id);
            }
            StateHasChanged();
            await Task.CompletedTask;
        }
    }
}