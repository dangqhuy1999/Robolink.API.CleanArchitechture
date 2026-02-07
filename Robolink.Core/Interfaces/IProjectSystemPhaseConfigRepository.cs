using Robolink.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Robolink.Core.Interfaces
{
    /// <summary>
    /// Specialized repository for ProjectSystemPhaseConfig.
    /// Extends generic repository with project-phase specific queries.
    /// </summary>
    public interface IProjectSystemPhaseConfigRepository : IGenericRepository<ProjectSystemPhaseConfig>
    {
        /// <summary>Get all phase configs for a specific project</summary>
        Task<IEnumerable<ProjectSystemPhaseConfig>> GetByProjectIdAsync(Guid projectId);

        /// <summary>Get phase config by project and system phase</summary>
        Task<ProjectSystemPhaseConfig?> GetByProjectAndPhaseAsync(Guid projectId, Guid systemPhaseId);

        /// <summary>Get enabled phases for project with tasks</summary>
        Task<IEnumerable<ProjectSystemPhaseConfig>> GetEnabledPhasesWithTasksAsync(Guid projectId);

        /// <summary>Check if project already has this phase configured</summary>
        Task<bool> ProjectHasPhaseAsync(Guid projectId, Guid systemPhaseId);

        /// <summary>Get next sequence number for project</summary>
        Task<int> GetNextSequenceAsync(Guid projectId);

        /// <summary>✅ NEW: Get all projects using this system phase</summary>
        Task<IEnumerable<ProjectSystemPhaseConfig>> GetBySystemPhaseIdAsync(Guid systemPhaseId);
    }
}