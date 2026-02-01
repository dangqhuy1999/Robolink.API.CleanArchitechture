using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Robolink.Core.Entities;
using Robolink.Core.Enums;

namespace Robolink.Infrastructure.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<SystemPhase> SystemPhases { get; set; }  // ✅ NEW
        public DbSet<ProjectSystemPhaseConfig> ProjectSystemPhaseConfigs { get; set; }  // ✅ NEW
        public DbSet<PhaseTask> PhaseTasks { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            // Chỉ áp dụng Converter nếu đang dùng Postgres
            if (Database.IsNpgsql())
            {
                var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc));

                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        {
                            property.SetValueConverter(dateTimeConverter);
                        }
                    }
                }
            }

            // 1. SystemPhase (Global Master Data)
            modelBuilder.Entity<SystemPhase>(entity =>
            {
                entity.Property(s => s.RowVersion).IsRowVersion()
                .HasDefaultValue(new byte[0]); // ✅ Thêm dòng này
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(s => s.Name).IsUnique();
                
            });
            // 2. ProjectSystemPhaseConfig (Project-Phase Mapping)
            modelBuilder.Entity<ProjectSystemPhaseConfig>(entity =>
            {
                entity.HasKey(pc => pc.Id);
                entity.HasIndex(pc => new { pc.ProjectId, pc.SystemPhaseId }).IsUnique();

                entity.HasOne(pc => pc.Project)
                      .WithMany(p => p.PhaseConfigs)
                      .HasForeignKey(pc => pc.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.SystemPhase)
                      .WithMany(sp => sp.ProjectConfigs)
                      .HasForeignKey(pc => pc.SystemPhaseId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(pc => pc.CustomPhaseName).HasMaxLength(100);
            });
            // 1. Cấu hình Client
            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(c => c.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]); // ✅ Thêm dòng này
                entity.HasMany(c => c.Projects)
                      .WithOne(p => p.Client)
                      .HasForeignKey(p => p.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            // Bổ sung vào OnModelCreating
            modelBuilder.Entity<Staff>(entity =>
            {
                // Giúp SQL Server hiểu đây là cột tự động quản lý phiên bản
                entity.Property(s => s.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]); // ✅ Thêm dòng này

                // Các cấu hình khác nếu cần
                entity.Property(s => s.FullName).HasMaxLength(200);
            });
            // 2. Cấu hình Project
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasIndex(p => p.ProjectCode).IsUnique();
                entity.Property(p => p.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]); // ✅ Thêm dòng này
                entity.Property(p => p.CustomerBudget).HasColumnType("decimal(18,2)");
                entity.Property(p => p.InternalBudget).HasColumnType("decimal(18,2)");

                // Quan hệ với Manager (Staff)
                entity.HasOne(p => p.Manager)
                      .WithMany(s => s.ManagedProjects) // Nếu em thêm ICollection<Project> vào Staff thì sửa thành .WithMany(s => s.ManagedProjects)
                      .HasForeignKey(p => p.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ParentProject)
                      .WithMany(t => t.SubProjectsItems)
                      .HasForeignKey(t => t.ParentProjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // 3. Cấu hình PhaseTask
            modelBuilder.Entity<PhaseTask>(entity =>
            {
                entity.HasOne(t => t.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(t => t.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa Project thì xóa Task

                entity.HasOne(t => t.AssignedStaff)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedStaffId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.ParentPhaseTask)
                      .WithMany(t => t.SubPhaseTasksItems)
                      .HasForeignKey(t => t.ParentPhaseTaskId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(t => t.PhaseConfig)  // ✅ NEW
                      .WithMany(pc => pc.PhaseTasks)
                      .HasForeignKey(t => t.ProjectSystemPhaseConfigId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<WorkLog>(entity =>
            {
                // ✅ THÊM DÒNG NÀY ĐỂ FIX LỖI ROWVERSION
                entity.Property(pr => pr.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]); // ✅ Thêm dòng này
                // CHUYỂN vòng lặp cấu hình V1-V10 sang ĐÂY
                for (int i = 1; i <= 10; i++)
                {
                    entity.Property($"V{i}").HasColumnType("decimal(18,2)");
                }
                entity.Property(pr => pr.ValueMain).HasColumnType("decimal(18,2)");
                entity.Property(pr => pr.ValueSub).HasColumnType("decimal(18,2)"); 
                // Chỉ cấu hình OnDelete và xác nhận quan hệ, 
                // EF Core sẽ tự khớp với [ForeignKey] trong Class
                entity.HasOne(pr => pr.WorkProject).WithMany(t => t.WorkLogs).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(pr => pr.WorkTask).WithMany(t => t.WorkLogs).OnDelete(DeleteBehavior.Restrict); // CHỈ ĐỊNH RÕ: Task.WorkLogs nối vào đây
                entity.HasOne(pr => pr.Operator).WithMany(t => t.WorkLogs).OnDelete(DeleteBehavior.Restrict);
            });

            // 5. Global Query Filter (Giữ nguyên - Rất tốt)
            modelBuilder.Entity<Staff>().HasQueryFilter(s => !s.IsDeleted);
            modelBuilder.Entity<Client>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<SystemPhase>().HasQueryFilter(sp => !sp.IsDeleted);
            modelBuilder.Entity<ProjectSystemPhaseConfig>().HasQueryFilter(pc => !pc.IsDeleted);
            modelBuilder.Entity<PhaseTask>().HasQueryFilter(t => !t.IsDeleted);
            modelBuilder.Entity<WorkLog>().HasQueryFilter(pr => !pr.IsDeleted);


            // Tìm đoạn này trong code của bạn và sửa lại
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var prop = entityType.FindProperty("RowVersion");
                if (prop != null && prop.ClrType == typeof(byte[]))
                {
                    // CẤU HÌNH CŨ:
                    // modelBuilder.Entity(entityType.ClrType).Property("RowVersion").IsRowVersion();

                    // ✅ CẤU HÌNH MỚI (SỬA LẠI NHƯ THẾ NÀY):
                    modelBuilder.Entity(entityType.ClrType)
                                .Property("RowVersion")
                                .IsRowVersion()
                                .HasDefaultValue(new byte[0]); // <--- DÒNG CỨU TINH
                }
            }

            // Seed Client
            modelBuilder.Entity<Client>().HasData(new Client
            {
                Id = Guid.Parse("188cd869-567e-4cd2-870a-48bdb04af5cd"),
                Name = "Samsung Vina",
                Industry = "Electronics",
                ContactEmail = "contact@samsung.com",
                IsDeleted = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RowVersion = Array.Empty<byte>()
            });

            // Seed Staff
            modelBuilder.Entity<Staff>().HasData(new Staff
            {
                Id = Guid.Parse("1b8c3dbf-63bb-4207-b108-9b28706185a7"),
                FullName = "Huy Dang",
                Username = "huydang.admin",
                PasswordHash = "AQAAAAEAACcQAAAAE...",
                Department = ProjectDepartment.Production,
                Role = ProjectRole.Manager,
                Status = StaffStatus.Active,
                IsDeleted = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RowVersion = Array.Empty<byte>()
            });
            // ✅ Seed System Phases (Master Data)
            SeedSystemPhases(modelBuilder);
        }
        /// <summary>
        /// Initialize global system phases that all projects can use
        /// </summary>
        private static void SeedSystemPhases(ModelBuilder modelBuilder)
        {
            // ✅ Use hardcoded DateTime instead of DateTime.UtcNow
            var seedDateTime = new DateTime(2026, 1, 30, 2, 38, 38, DateTimeKind.Utc);
            var rowVersionPlaceholder = new byte[0]; // ✅ Tạo một giá trị trống để seed
            var systemPhases = new SystemPhase[]
            {
                
                new SystemPhase
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "Initialize",
                        Description = "Project initialization and setup",
                        DefaultSequence = 1,
                        IsActive = true,
                        CreatedAt = seedDateTime,
                        IsDeleted = false,
                        RowVersion = rowVersionPlaceholder // ✅ BỔ SUNG DÒNG NÀY
                    },
                    new SystemPhase
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Name = "Engineering",
                        Description = "Design and engineering phase",
                        DefaultSequence = 2,
                        IsActive = true,
                        CreatedAt = seedDateTime,
                        IsDeleted = false,
                        RowVersion = rowVersionPlaceholder // ✅ BỔ SUNG DÒNG NÀY
                    },
                    new SystemPhase
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Name = "Fabrication",
                        Description = "Manufacturing and fabrication",
                        DefaultSequence = 3,
                        IsActive = true,
                        CreatedAt = seedDateTime,
                        IsDeleted = false,
                        RowVersion = rowVersionPlaceholder // ✅ BỔ SUNG DÒNG NÀY
                    },
                    new SystemPhase
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Name = "CNC Cutting",
                        Description = "CNC cutting operations",
                        DefaultSequence = 4,
                        IsActive = true,
                        CreatedAt = seedDateTime,
                        IsDeleted = false,
                        RowVersion = rowVersionPlaceholder // ✅ BỔ SUNG DÒNG NÀY
                    },
                    new SystemPhase
                    {
                        Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                        Name = "Assembly",
                        Description = "Product assembly phase",
                        DefaultSequence = 5,
                        IsActive = true,
                        CreatedAt = seedDateTime,
                        IsDeleted = false,
                        RowVersion = rowVersionPlaceholder // ✅ BỔ SUNG DÒNG NÀY
                    }
                };

                modelBuilder.Entity<SystemPhase>().HasData(systemPhases);
        }
    }
}