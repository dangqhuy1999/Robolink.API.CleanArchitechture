using Microsoft.AspNetCore.Components;
using Robolink.Shared.DTOs;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Components.Features.Projects.Tables
{
    public partial class ProjectHierarchyTree : ComponentBase
    {

        /// <summary>
        /// Filtered list - only root projects (no parent)
        /// </summary>
        public List<ProjectViewModel> MainProjects { get; set; } = new();

        /// <summary>
        /// Track which nodes are expanded in the tree
        /// </summary>
        public HashSet<Guid> ExpandedNodes { get; set; } = new();

        /// <summary>
        /// Update MainProjects when AllProjects parameter changes
        /// </summary>
        protected override void OnParametersSet()
        {
            MainProjects = AllProjects?
                .Where(p => !p.ParentProjectId.HasValue)
                .ToList() ?? new();

            // Optional: Log for debugging
            System.Diagnostics.Debug.WriteLine(
                $"[ProjectHierarchyTree] MainProjects filtered: {MainProjects.Count} items");
        }

        /// <summary>
        /// Check if a specific node is expanded
        /// </summary>
        private bool IsExpanded(Guid projectId) => ExpandedNodes.Contains(projectId);

        /// <summary>
        /// Get count of expanded nodes (for debugging)
        /// </summary>
        private int ExpandedCount => ExpandedNodes.Count;
    }
}