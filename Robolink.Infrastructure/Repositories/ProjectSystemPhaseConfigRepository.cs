using Microsoft.EntityFrameworkCore;
using Robolink.Core.Entities;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Robolink.Infrastructure.Repositories
{
    /// <summary>
    /// Specialized repository for ProjectSystemPhaseConfig with business logic.
    /// Prevents duplicate configurations and provides phase management queries.
    /// </summary>
    public class ProjectSystemPhaseConfigRepository : GenericRepository<ProjectSystemPhaseConfig>, IProjectSystemPhaseConfigRepository
    {
        
        public ProjectSystemPhaseConfigRepository(IDbContextFactory<AppDBContext> contextFactory)
    : base(contextFactory) { }
        /// <summary>Get all phase configs for a specific project (ordered by sequence)</summary>
        public async Task<IEnumerable<ProjectSystemPhaseConfig>> GetByProjectIdAsync(Guid projectId)
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Repository.GetByProjectIdAsync: projectId = {projectId}");
            
            var result = await _dbSet
                .Where(pc => pc.ProjectId == projectId && !pc.IsDeleted)
                .Include(pc => pc.SystemPhase)
                .Include(pc => pc.PhaseTasks)
                .OrderBy(pc => pc.Sequence)
                .ToListAsync();
            
            System.Diagnostics.Debug.WriteLine($"✅ Repository found {result.Count} records");
            
            foreach (var item in result)
            {
                System.Diagnostics.Debug.WriteLine($"   - {item.Id}: SystemPhaseId={item.SystemPhaseId}, IsDeleted={item.IsDeleted}");
            }
            
            return result;
        }

        /// <summary>Get specific phase config by project and system phase</summary>
        public async Task<ProjectSystemPhaseConfig?> GetByProjectAndPhaseAsync(Guid projectId, Guid systemPhaseId)
        {
            return await _dbSet
                .Where(pc => pc.ProjectId == projectId 
                         && pc.SystemPhaseId == systemPhaseId 
                         && !pc.IsDeleted)
                .Include(pc => pc.SystemPhase)
                .Include(pc => pc.PhaseTasks)
                .FirstOrDefaultAsync();
        }

        /// <summary>Get enabled phases for project with all associated tasks</summary>
        public async Task<IEnumerable<ProjectSystemPhaseConfig>> GetEnabledPhasesWithTasksAsync(Guid projectId)
        {
            return await _dbSet
                .Where(pc => pc.ProjectId == projectId 
                         && pc.IsEnabled 
                         && !pc.IsDeleted)
                .Include(pc => pc.SystemPhase)
                .Include(pc => pc.PhaseTasks)
                    .ThenInclude(pt => pt.AssignedStaff)
                .OrderBy(pc => pc.Sequence)
                .ToListAsync();
        }

        /// <summary>Check if project already has this phase (prevent duplicates)</summary>
        public async Task<bool> ProjectHasPhaseAsync(Guid projectId, Guid systemPhaseId)
        {
            return await _dbSet.AnyAsync(pc => pc.ProjectId == projectId 
                                           && pc.SystemPhaseId == systemPhaseId 
                                           && !pc.IsDeleted);
        }

        /// <summary>Get next sequence number for new phase in project</summary>
        public async Task<int> GetNextSequenceAsync(Guid projectId)
        {
            var maxSequence = await _dbSet
                .Where(pc => pc.ProjectId == projectId && !pc.IsDeleted)
                .MaxAsync(pc => (int?)pc.Sequence) ?? 0;

            return maxSequence + 1;
        }
        // Thêm vào trong ProjectSystemPhaseConfigRepository.cs
        public override async Task<ProjectSystemPhaseConfig?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(pc => pc.SystemPhase) // ✅ Quan trọng nhất là dòng này
                .FirstOrDefaultAsync(pc => pc.Id == id && !pc.IsDeleted);
        }
    }
}