using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Robolink.Core.Entities;
using Robolink.Shared.Enums;

namespace Robolink.Infrastructure.Data
{
    public class AppDBContext(DbContextOptions<AppDBContext> options) : DbContext(options)
    {
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<SystemPhase> SystemPhases { get; set; }
        public DbSet<ProjectSystemPhaseConfig> ProjectSystemPhaseConfigs { get; set; }
        public DbSet<PhaseTask> PhaseTasks { get; set; }
        public DbSet<WorkLog> WorkLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // DateTime Converter for PostgreSQL
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

            // ========== SYSTEM PHASE ==========
            modelBuilder.Entity<SystemPhase>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(s => s.Name).IsUnique();
                entity.Property(s => s.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]);
                entity.HasQueryFilter(s => !s.IsDeleted);
            });

            // ========== PROJECT SYSTEM PHASE CONFIG ==========
            modelBuilder.Entity<ProjectSystemPhaseConfig>(entity =>
            {
                entity.HasKey(pc => pc.Id);
                entity.HasIndex(pc => new { pc.ProjectId, pc.SystemPhaseId }).IsUnique();
                entity.Property(pc => pc.CustomPhaseName).HasMaxLength(100);
                entity.Property(pc => pc.Sequence).IsRequired(); // Sequence (int)
                entity.Property(pc => pc.IsEnabled).IsRequired(); // IsEnabled (bool)

                // ✅ NEW: Add RowVersion
                entity.Property(pc => pc.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]);

                // Relationships
                entity.HasOne(pc => pc.Project)
                      .WithMany(p => p.PhaseConfigs)
                      .HasForeignKey(pc => pc.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pc => pc.SystemPhase)
                      .WithMany(sp => sp.ProjectConfigs)
                      .HasForeignKey(pc => pc.SystemPhaseId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅ NEW: Add Query Filter
                entity.HasQueryFilter(pc => !pc.IsDeleted);
            });

            // ========== CLIENT ==========
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(c => c.Id);

                // Thêm các giới hạn vật lý vào đây thay vì để ở Entity
                entity.Property(c => c.Name)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(c => c.Industry)
                      .HasMaxLength(100);

                entity.Property(c => c.RowVersion)
                      .IsRowVersion()
                      .HasDefaultValue(new byte[0]);
                entity.HasMany(c => c.Projects)
                      .WithOne(p => p.Client)
                      .HasForeignKey(p => p.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasQueryFilter(c => !c.IsDeleted);
            });

            // ========== STAFF ==========
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasIndex(s => s.Username).IsUnique(); // Index để Login nhanh + Unique
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.FullName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Username).IsRequired().HasMaxLength(50);
                entity.Property(s => s.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]);
                entity.HasQueryFilter(s => !s.IsDeleted);
            });

            // ========== PROJECT ==========
            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => p.ProjectCode).IsUnique();
                entity.Property(p => p.CustomerBudget).HasColumnType("decimal(18,2)");
                entity.Property(p => p.InternalBudget).HasColumnType("decimal(18,2)");
                entity.Property(p => p.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]);

                // Manager relationship
                entity.HasOne(p => p.Manager)
                      .WithMany(s => s.ManagedProjects)
                      .HasForeignKey(p => p.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Parent-Sub Project relationship
                entity.HasOne(p => p.ParentProject)
                      .WithMany(p => p.SubProjectsItems)
                      .HasForeignKey(p => p.ParentProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(p => !p.IsDeleted);
            });

            // ========== PHASE TASK ==========
            modelBuilder.Entity<PhaseTask>(entity =>
            {
                entity.HasKey(t => t.Id);

                // ✅ FIXED: Configure properties
                entity.Property(t => t.ProcessRate)
                      .HasDefaultValue(0)
                      .HasColumnType("smallint");

                entity.Property(t => t.EstimatedHours)
                      .HasColumnType("decimal(10,2)")
                      .HasDefaultValue(0);

                entity.Property(t => t.CompletedAt)
                      .HasColumnType("timestamp with time zone");

                entity.Property(t => t.RowVersion)
                      .IsRowVersion()
                      .HasDefaultValue(new byte[0]);

                // ✅ FIXED: Project relationship
                entity.HasOne(t => t.Project)
                      .WithMany(p => p.Tasks)
                      .HasForeignKey(t => t.ProjectId)
                      .OnDelete(DeleteBehavior.Cascade);

                // ✅ FIXED: PhaseConfig relationship
                entity.HasOne(t => t.PhaseConfig)
                      .WithMany(pc => pc.PhaseTasks)
                      .HasForeignKey(t => t.ProjectSystemPhaseConfigId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅ FIXED: AssignedStaff relationship
                entity.HasOne(t => t.AssignedStaff)
                      .WithMany()
                      .HasForeignKey(t => t.AssignedStaffId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅ FIXED: Self-referencing (Parent-Child) relationship
                entity.HasOne(t => t.ParentPhaseTask)
                      .WithMany(parent => parent.SubPhaseTasksItems)  // ✅ CHANGED: Use 'parent' to clarify
                      .HasForeignKey(t => t.ParentPhaseTaskId)
                      .OnDelete(DeleteBehavior.Restrict);

                // ✅ Query filter
                entity.HasQueryFilter(t => !t.IsDeleted);
            });

            // ========== WORK LOG ==========
            modelBuilder.Entity<WorkLog>(entity =>
            {
                entity.HasKey(w => w.Id);
                entity.Property(w => w.RowVersion).IsRowVersion().HasDefaultValue(new byte[0]);

                // Configure V1-V10 properties
                for (int i = 1; i <= 10; i++)
                {
                    entity.Property($"V{i}").HasColumnType("decimal(18,2)");
                }

                entity.Property(w => w.ValueMain).HasColumnType("decimal(18,2)");
                entity.Property(w => w.ValueSub).HasColumnType("decimal(18,2)");

                // Relationships
                entity.HasOne(w => w.WorkProject)
                      .WithMany(p => p.WorkLogs)
                      .HasForeignKey(w => w.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.WorkTask)
                      .WithMany(t => t.WorkLogs)
                      .HasForeignKey(w => w.PhaseTaskId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.Operator)
                      .WithMany(s => s.WorkLogs)
                      .HasForeignKey(w => w.OperatorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(w => !w.IsDeleted);
            });

            // ========== SEED DATA ==========
            SeedClients(modelBuilder);
            SeedStaff(modelBuilder);
            SeedSystemPhases(modelBuilder);
        }

        // ========== SEED METHODS ==========
        private static void SeedClients(ModelBuilder modelBuilder)
        {
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
        }

        private static void SeedStaff(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Staff>().HasData(new Staff
            {
                Id = Guid.Parse("1b8c3dbf-63bb-4207-b108-9b28706185a7"),
                FullName = "Huy Dang",
                Username = "huydang.admin",
                PasswordHash = "AQAAAAEAACcQAAAAE...",
                Email = "loli123@hmail.com",
                Department = ProjectDepartment.Production,
                Role = ProjectRole.Manager,
                Status = StaffStatus.Active,
                IsDeleted = false,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RowVersion = Array.Empty<byte>()
            });
        }

        private static void SeedSystemPhases(ModelBuilder modelBuilder)
        {
            var seedDateTime = new DateTime(2026, 1, 30, 2, 38, 38, DateTimeKind.Utc);
            var rowVersionPlaceholder = new byte[0];

            modelBuilder.Entity<SystemPhase>().HasData(
                new SystemPhase
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Initialize",
                    Description = "Project initialization and setup",
                    DefaultSequence = 1,
                    IsActive = true,
                    CreatedAt = seedDateTime,
                    IsDeleted = false,
                    RowVersion = rowVersionPlaceholder
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
                    RowVersion = rowVersionPlaceholder
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
                    RowVersion = rowVersionPlaceholder
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
                    RowVersion = rowVersionPlaceholder
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
                    RowVersion = rowVersionPlaceholder
                }
            );
        }
    }
}