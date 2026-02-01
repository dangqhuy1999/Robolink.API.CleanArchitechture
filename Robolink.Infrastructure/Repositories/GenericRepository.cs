using Microsoft.EntityFrameworkCore;
using Robolink.Core.Common;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using System.Linq.Expressions;

namespace Robolink.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation for all domain entities.
    /// Provides base CRUD and query operations.
    /// </summary>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : EntityBase
    {
        private readonly IDbContextFactory<AppDBContext> _contextFactory;
        protected readonly AppDBContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(IDbContextFactory<AppDBContext> contextFactory)
        {
            _contextFactory = contextFactory;

            // Khởi tạo một context mặc định cho các tác vụ Write (Add/Update/Delete)
            _context = _contextFactory.CreateDbContext();
            _dbSet = _context.Set<TEntity>();
        }

        // ========== READ ==========
        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }
        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(int startIndex, int count)
        {
            using var tempContext = await _contextFactory.CreateDbContextAsync();
            var tempDbSet = tempContext.Set<TEntity>();

            var query = tempDbSet.AsNoTracking().Where(e => !e.IsDeleted);
            
            // ✅ For Project entity, include related entities
            if (typeof(TEntity).Name == "Project")
            {
                query = query
                    .Include("Client")
                    .Include("Manager")
                    .Include("ParentProject")
                    .Include("SubProjectsItems"); // ✅ THIS IS CRITICAL!
            }
     
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip(startIndex)
                .Take(count)
                .ToListAsync();

            return (items, totalCount);
        }
        // Sửa các hàm READ (FindAsync, GetPagedAsync, v.v.)
        public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Tạo một context tạm thời chỉ dành riêng cho lệnh này
            using var tempContext = await _contextFactory.CreateDbContextAsync();

            return await tempContext.Set<TEntity>()
                .AsNoTracking() // Cực kỳ quan trọng để tăng tốc triệu dòng
                .Where(e => !e.IsDeleted)
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            var query = _dbSet.Where(e => !e.IsDeleted);
            
            if (predicate != null)
                query = query.Where(predicate);

            return await query.CountAsync();
        }

        // ========== CREATE ==========
        // Project
        public virtual async Task AddAsync(TEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);
        }
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            await _dbSet.AddRangeAsync(entityList);
        }
        // ========== UPDATE ==========
        public virtual async Task UpdateAsync(TEntity entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }
            _dbSet.UpdateRange(entityList);
            await Task.CompletedTask;
        }

        // ========== DELETE ==========
        public virtual async Task SoftDeleteAsync(Guid id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
                _dbSet.Update(entity);
            }
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task SoftDeleteRangeAsync(IEnumerable<Guid> ids)
        {
            var entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            _dbSet.UpdateRange(entities);
        }

        // ========== SAVE ==========
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}