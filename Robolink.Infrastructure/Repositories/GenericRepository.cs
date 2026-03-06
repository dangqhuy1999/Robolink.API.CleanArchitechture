using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Robolink.Core.Common;
using Robolink.Core.Interfaces;
using Robolink.Infrastructure.Data;
using Robolink.Shared.DTOs;
using System.Linq.Expressions;

namespace Robolink.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation for all domain entities.
    /// Provides base CRUD and query operations.
    /// </summary>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly IDbContextFactory<AppDBContext> _contextFactory;
        protected readonly AppDBContext _context;
        protected readonly IConfigurationProvider _configurationProvider;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(IDbContextFactory<AppDBContext> contextFactory,
        IMapper mapper) // Inject Mapper để lấy cấu hình
        {
            _contextFactory = contextFactory;
            _configurationProvider = mapper.ConfigurationProvider;
            // Khởi tạo một context mặc định cho các tác vụ Write (Add/Update/Delete)
            _context = _contextFactory.CreateDbContext();
            _dbSet = _context.Set<TEntity>();
        }

        // VŨ KHÍ MỚI: Truyền ID vào, lấy ra DTO của bất kỳ bảng nào!
        // Use for create / update / delete xong thì gọi hàm này để lấy lại DTO mới nhất
        public async Task<TDto?> GetProjectedByIdAsync<TDto>(Guid id)
        {
            // Tạo context riêng cho truy vấn đọc (Best practice cho EF Core)
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Set<TEntity>()
                .AsNoTracking()
                .Where(e => e.Id == id) // TEntity kế thừa EntityBase nên chắc chắn có Id
                .ProjectTo<TDto>(_configurationProvider) // Tự động Map và Include mọi thứ
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TDto>> GetProjectedAsync<TDto>(Expression<Func<TEntity, bool>> predicate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate) // Lọc theo điều kiện (ví dụ: ProjectId == ...)
                .ProjectTo<TDto>(_configurationProvider)
                .ToListAsync();
        }

        // Hàm "thần thánh": Vừa lọc, vừa phân trang, vừa Map sang DTO bằng ProjectTo
        public async Task<PagedResult<TDto>> GetPagedProjectedAsync<TDto>(
            int startIndex,
            int count,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken ct = default)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // 1. Khởi tạo query và lọc IsDeleted (nếu có)
            var query = context.Set<TEntity>().AsNoTracking();

            // Giả sử em có thuộc tính IsDeleted dùng chung
            query = query.Where(x => !x.IsDeleted);

            // 2. Áp dụng thêm bộ lọc riêng (nếu có)
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // 3. Đếm tổng số bản ghi thỏa điều kiện
            var totalCount = await query.CountAsync();

            // 4. Phân trang và ProjectTo thẳng sang DTO
            var items = await query
                .OrderByDescending(x => x.CreatedAt) // Sắp xếp mặc định
                .Skip(startIndex)
                .Take(count)
                .ProjectTo<TDto>(_configurationProvider)
                .ToListAsync(ct);

            return new PagedResult<TDto>(items, totalCount);
        }
        // ========== READ ==========
        public virtual async Task<TEntity?> GetByIdAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            // Tạo một Context mới toanh chỉ dành riêng cho lần đọc này
            using var context = _contextFactory.CreateDbContext();

            // Lấy DbSet từ cái context mới tạo này
            IQueryable<TEntity> query = context.Set<TEntity>();

            // Nạp các bảng liên quan
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.AsNoTracking() // Đọc dữ liệu thì nên dùng AsNoTracking
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {

            // Tạo một context riêng cho luồng này
            using var context = _contextFactory.CreateDbContext();

            return await context.Set<TEntity>()
                .Where(e => !e.IsDeleted)
                .AsNoTracking() // Thêm cái này để tăng tốc độ đọc dữ liệu
                .ToListAsync();
        }
        // Thêm tham số filter để UI truyền điều kiện lọc xuống
        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            int startIndex,
            int count,
            Expression<Func<TEntity, bool>>? filter = null,
            params Expression<Func<TEntity, object>>[] includes) // THÊM CÁI NÀY
        {
            using var tempContext = await _contextFactory.CreateDbContextAsync();
            var tempDbSet = tempContext.Set<TEntity>();

            var query = tempDbSet.AsNoTracking().Where(e => !e.IsDeleted);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // ĐÂY CHÍNH LÀ ĐOẠN ĐÁNH BAY CÁI IF-ELSE XẤU XÍ CỦA EM
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
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
            // Tạo một context riêng cho luồng này
            using var context = _contextFactory.CreateDbContext();

            return await context.Set<TEntity>()
                .AsNoTracking() // Thêm cái này để tăng tốc độ đọc dữ liệu
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Tạo một context riêng cho luồng này
            using var context = _contextFactory.CreateDbContext();

            return await context.Set<TEntity>()
                .AsNoTracking() // Thêm cái này để tăng tốc độ đọc dữ liệu
                .Where(e => !e.IsDeleted)
                .AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            // 1. Mượn một context mới toanh
            using var context = _contextFactory.CreateDbContext();

            // 2. Lấy tập dữ liệu và lọc những ông chưa bị xóa
            var query = context.Set<TEntity>().Where(e => !e.IsDeleted);

            // 3. Nếu có điều kiện lọc riêng (predicate) thì áp dụng thêm
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // 4. Đếm và trả về kết quả (Xong hàm này context tự hủy nhờ 'using')
            return await query.CountAsync();
        }

        // ========== CREATE ==========
        // Project
        public virtual async Task AddAsync(TEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            using var context = _contextFactory.CreateDbContext();
            await context.Set<TEntity>().AddAsync(entity);
            // 4. BẮT BUỘC: Lưu thay đổi xuống Database
            await context.SaveChangesAsync();
        }
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityList = entities.ToList();
            foreach (var entity in entityList)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            using var context = _contextFactory.CreateDbContext();
            await context.Set<TEntity>().AddRangeAsync(entityList);
            // 4. BẮT BUỘC: Lưu thay đổi xuống Database
            await context.SaveChangesAsync();
        }
        // ========== UPDATE ==========
        public virtual async Task UpdateAsync(TEntity entity)
        {
            // 1. Cập nhật thời gian sửa đổi
            entity.UpdatedAt = DateTime.UtcNow;

            // 2. Mượn context
            using var context = _contextFactory.CreateDbContext();

            // 3. Đánh dấu entity này là đã bị thay đổi (Attach và set state là Modified)
            context.Set<TEntity>().Update(entity);

            // 4. BẮT BUỘC: Lưu thay đổi xuống Database
            await context.SaveChangesAsync();
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            var entityList = entities.ToList();

            // 1. Cập nhật thời gian cho toàn bộ danh sách
            foreach (var entity in entityList)
            {
                entity.UpdatedAt = DateTime.UtcNow;
            }

            // 2. Mượn một context mới
            using var context = _contextFactory.CreateDbContext();

            // 3. Đưa danh sách vào để EF bắt đầu theo dõi (Tracking)
            context.Set<TEntity>().UpdateRange(entityList);

            // 4. QUAN TRỌNG NHẤT: Lưu tất cả thay đổi xuống DB trong 1 lần gửi
            await context.SaveChangesAsync();
        }

        // ========== DELETE ==========
        public virtual async Task SoftDeleteAsync(Guid id)
        {
            // 1. Mượn context
            using var context = _contextFactory.CreateDbContext();

            // 2. Tìm entity (không dùng AsNoTracking ở đây vì mình cần sửa nó)
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {
                // 3. Đánh dấu xóa và cập nhật thời gian
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;

                // 4. Lưu trực tiếp (vì cùng context nên EF tự hiểu đây là Update)
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            // 1. Mượn context
            using var context = _contextFactory.CreateDbContext();

            // 2. Tìm entity
            var entity = await context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);

            if (entity != null)
            {
                // 3. Đánh dấu xóa
                context.Set<TEntity>().Remove(entity);

                // 4. BẮT BUỘC: Nhấn nút "Gửi" lệnh xuống DB
                await context.SaveChangesAsync();
            }
        }

        public virtual async Task SoftDeleteRangeAsync(IEnumerable<Guid> ids)
        {
            // 1. Mượn context
            using var context = _contextFactory.CreateDbContext();

            // 2. Tìm entity (không dùng AsNoTracking ở đây vì mình cần sửa nó)
            var entities = await context.Set<TEntity>().Where(e => ids.Contains(e.Id)).ToListAsync();
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            // 3. Đánh dấu update
            context.Set<TEntity>().UpdateRange(entities);
            // 4. BẮT BUỘC: Nhấn nút "Gửi" lệnh xuống DB
            await context.SaveChangesAsync();
        }

        // ========== SAVE ==========
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}