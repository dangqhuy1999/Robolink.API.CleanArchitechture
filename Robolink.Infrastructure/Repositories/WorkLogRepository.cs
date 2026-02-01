using Microsoft.EntityFrameworkCore;
using Robolink.Core.Entities;
using Robolink.Core.Enums;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Robolink.Infrastructure.Repositories
{
    /// <summary>
    /// Specialized repository for WorkLog with advanced querying and calculations.
    /// Handles work log statistics, aggregations, and date-range queries.
    /// </summary>
    public class WorkLogRepository : GenericRepository<WorkLog>, IWorkLogRepository
    {
        public WorkLogRepository(IDbContextFactory<AppDBContext> contextFactory)
    : base(contextFactory) { }

        /// <summary>Get all work logs for a specific project</summary>
        public async Task<IEnumerable<WorkLog>> GetByProjectIdAsync(Guid projectId)
        {
            return await _dbSet
                .Where(wl => wl.ProjectId == projectId && !wl.IsDeleted)
                .Include(wl => wl.WorkProject)
                .Include(wl => wl.WorkTask)
                .Include(wl => wl.Operator)
                .OrderByDescending(wl => wl.CreatedAt)
                .ToListAsync();
        }

        /// <summary>Get work logs for a specific phase task</summary>
        public async Task<IEnumerable<WorkLog>> GetByPhaseTaskIdAsync(Guid phaseTaskId)
        {
            return await _dbSet
                .Where(wl => wl.PhaseTaskId == phaseTaskId && !wl.IsDeleted)
                .Include(wl => wl.Operator)
                .OrderByDescending(wl => wl.CreatedAt)
                .ToListAsync();
        }

        /// <summary>Get work logs created by specific operator</summary>
        public async Task<IEnumerable<WorkLog>> GetByOperatorIdAsync(Guid operatorId)
        {
            return await _dbSet
                .Where(wl => wl.OperatorId == operatorId && !wl.IsDeleted)
                .Include(wl => wl.WorkProject)
                .Include(wl => wl.WorkTask)
                .OrderByDescending(wl => wl.CreatedAt)
                .ToListAsync();
        }

        /// <summary>Get work logs by type (Manufacturing, HR, Sales, etc.)</summary>
        public async Task<IEnumerable<WorkLog>> GetByTypeAsync(LogType logType)
        {
            return await _dbSet
                .Where(wl => wl.Type == logType && !wl.IsDeleted)
                .Include(wl => wl.WorkProject)
                .Include(wl => wl.Operator)
                .OrderByDescending(wl => wl.CreatedAt)
                .ToListAsync();
        }

        /// <summary>Calculate total value for a project</summary>
        public async Task<decimal> GetTotalValueByProjectAsync(Guid projectId)
        {
            return await _dbSet
                .Where(wl => wl.ProjectId == projectId && !wl.IsDeleted && wl.Status == LogStatus.Success)
                .SumAsync(wl => wl.ValueMain);
        }

        /// <summary>Get work logs within date range</summary>
        public async Task<IEnumerable<WorkLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(wl => wl.CreatedAt >= startDate 
                         && wl.CreatedAt <= endDate 
                         && !wl.IsDeleted)
                .Include(wl => wl.WorkProject)
                .Include(wl => wl.Operator)
                .OrderByDescending(wl => wl.CreatedAt)
                .ToListAsync();
        }

        /// <summary>Get statistics for a phase task's work logs</summary>
        public async Task<WorkLogStats> GetPhaseTaskStatsAsync(Guid phaseTaskId)
        {
            var logs = await _dbSet
                .Where(wl => wl.PhaseTaskId == phaseTaskId && !wl.IsDeleted)
                .ToListAsync();

            return new WorkLogStats
            {
                TotalLogs = logs.Count,
                SuccessCount = logs.Count(wl => wl.Status == LogStatus.Success),
                ErrorCount = logs.Count(wl => wl.Status == LogStatus.Error),
                TotalValue = logs.Where(wl => wl.Status == LogStatus.Success).Sum(wl => wl.ValueMain),
                FirstLogDate = logs.Any() ? logs.Min(wl => wl.CreatedAt) : DateTime.UtcNow,
                LastLogDate = logs.Any() ? logs.Max(wl => wl.CreatedAt) : DateTime.UtcNow
            };
        }
    }
}