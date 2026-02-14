using Robolink.Core.Entities;
using Robolink.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Robolink.Core.Interfaces
{
    /// <summary>
    /// Specialized repository for WorkLog.
    /// Extends generic repository with work log specific queries and calculations.
    /// </summary>
    public interface IWorkLogRepository : IGenericRepository<WorkLog>
    {
        /// <summary>Get all work logs for a specific project</summary>
        Task<IEnumerable<WorkLog>> GetByProjectIdAsync(Guid projectId);

        /// <summary>Get work logs for a specific phase task</summary>
        Task<IEnumerable<WorkLog>> GetByPhaseTaskIdAsync(Guid phaseTaskId);

        /// <summary>Get work logs created by specific operator</summary>
        Task<IEnumerable<WorkLog>> GetByOperatorIdAsync(Guid operatorId);

        /// <summary>Get work logs by type (Manufacturing, HR, Sales, etc.)</summary>
        Task<IEnumerable<WorkLog>> GetByTypeAsync(LogType logType);

        /// <summary>Calculate total value for a project</summary>
        Task<decimal> GetTotalValueByProjectAsync(Guid projectId);

        /// <summary>Get work logs within date range</summary>
        Task<IEnumerable<WorkLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>Get statistics for a phase task</summary>
        Task<WorkLogStats> GetPhaseTaskStatsAsync(Guid phaseTaskId);
    }

    /// <summary>Statistics data for a phase task's work logs</summary>
    public class WorkLogStats
    {
        public int TotalLogs { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime FirstLogDate { get; set; }
        public DateTime LastLogDate { get; set; }
    }
}