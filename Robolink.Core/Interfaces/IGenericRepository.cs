using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Robolink.Core.Interfaces
{
    /// <summary>
    /// Generic repository interface for all domain entities.
    /// Provides base CRUD and query operations.
    /// </summary>
    public interface IGenericRepository<TEntity> where TEntity : EntityBase
    {
        // ========== READ ==========
        /// <summary>Get entity by ID</summary>
        Task<TEntity?> GetByIdAsync(Guid id);

        /// <summary>Get all entities (respects soft delete by default)</summary>
        Task<IEnumerable<TEntity>> GetAllAsync();
        /// <summary>Find entities by predicate</summary>
        /// 
        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int startIndex, int count);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>Find single entity by predicate</summary>
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>Check if entity exists</summary>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>Get count of entities</summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

        // ========== CREATE ==========
        /// <summary>Add single entity</summary>
        Task AddAsync(TEntity entity);

        /// <summary>Add multiple entities</summary>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        // ========== UPDATE ==========
        /// <summary>Update entity</summary>
        Task UpdateAsync(TEntity entity);

        /// <summary>Update multiple entities</summary>
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        // ========== DELETE ==========
        /// <summary>Soft delete (sets IsDeleted = true)</summary>
        Task SoftDeleteAsync(Guid id);

        /// <summary>Hard delete</summary>
        Task DeleteAsync(Guid id);

        /// <summary>Soft delete multiple entities</summary>
        Task SoftDeleteRangeAsync(IEnumerable<Guid> ids);

        // ========== SAVE ==========
        /// <summary>Persist changes to database</summary>
        Task<int> SaveChangesAsync();
    }
}