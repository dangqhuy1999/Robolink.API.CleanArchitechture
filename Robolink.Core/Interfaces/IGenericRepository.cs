using Robolink.Core.Common;
using Robolink.Shared.DTOs;
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
        // Thêm tham số includes để cho phép nạp các bảng liên quan
        // Hàm "thần thánh": Vừa lọc, vừa phân trang, vừa Map sang DTO bằng ProjectTo
        Task<PagedResult<TDto>> GetPagedProjectedAsync<TDto>(
            int startIndex,
            int count,
            Expression<Func<TEntity, bool>>? predicate = null);

        // VŨ KHÍ MỚI: Truyền ID vào, lấy ra DTO của bất kỳ bảng nào!/ 
        // Dung create / update / delete xong thì gọi hàm này để lấy lại DTO mới nhất
        Task<TDto?> GetProjectedByIdAsync<TDto>(Guid id);
        // Lấy danh sách DTO dựa trên một điều kiện (predicate)
        Task<IEnumerable<TDto>> GetProjectedAsync<TDto>(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>Get all entities (respects soft delete by default)</summary>
        Task<IEnumerable<TEntity>> GetAllAsync();
        /// <summary>Find entities by predicate</summary>
        /// 
        Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            int startIndex,
            int count,
            Expression<Func<TEntity, bool>>? filter = null,
            params Expression<Func<TEntity, object>>[] includes); // THÊM CÁI NÀY
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