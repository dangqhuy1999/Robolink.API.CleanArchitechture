using Microsoft.AspNetCore.Components;
using Robolink.WebApp.Modules.ProjectManagement.Features.Projects.ViewModels;

namespace Robolink.WebApp.Modules.ProjectManagement.Shared.Components.DataDisplay.Table
{
    public partial class ProjectTable : ComponentBase
    {


        /// <summary>
        /// Track which rows are expanded to show sub-projects
        /// </summary>
        private HashSet<Guid> ExpandedProjects { get; set; } = new();

        /// <summary>
        /// Current sorting column
        /// </summary>
        private string SortColumn { get; set; } = "Name";

        /// <summary>
        /// Sort direction: "asc" or "desc"
        /// </summary>
        private string SortDirection { get; set; } = "asc";

        // ===== COMPUTED PROPERTIES =====

        /// <summary>
        /// Check if a row is expanded
        /// </summary>
        private bool IsExpanded(Guid projectId) => ExpandedProjects.Contains(projectId);
        

        /// <summary>
        /// Get count of expanded rows
        /// </summary>
        private int ExpandedCount => ExpandedProjects.Count;

        /// <summary>
        /// Check if any projects are displayed
        /// </summary>
        private bool HasProjects => Projects?.Items.ToList().Any() == true;

        /// <summary>
        /// Get sorted projects based on current sort state
        /// </summary>
        private List<ProjectViewModel> SortedProjects
        {
            get
            {
                if (!Projects?.Items.ToList().Any() == true) return new();

                return SortDirection == "asc"
                    ? Projects.Items.ToList().OrderBy(GetSortValue).ToList()
                    : Projects.Items.ToList().OrderByDescending(GetSortValue).ToList();
            }
        }

        private object GetSortValue(ProjectViewModel project) => SortColumn switch
        {
            "Name" => project.Name,
            "Status" => project.Status,
            "Priority" => project.Priority,
            "StartDate" => project.StartDate,
            "Deadline" => project.Deadline,
            _ => project.Name
        };
    }
}