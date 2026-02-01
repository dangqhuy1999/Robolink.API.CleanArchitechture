# Robolink.Core — Domain Layer Documentation

**Version:** 2.0  

**Last Updated:** January 30, 2026  

**Status:** Complete with Dynamic Workflow System  

---

## 📋 Table of Contents

1. [Overview](#overview)
2. [Architecture & DDD](#architecture--ddd)
3. [Entity Model](#entity-model)
4. [Key Concepts: Dynamic Workflow](#key-concepts-dynamic-workflow)
5. [Aggregate Roots & Boundaries](#aggregate-roots--boundaries)
6. [Data Flow Examples](#data-flow-examples)
7. [Enums & Value Types](#enums--value-types)
8. [Repository Interfaces](#repository-interfaces)
9. [Best Practices](#best-practices)
10. [Troubleshooting](#troubleshooting)

---

## Overview

`Robolink.Core` is the **Domain/Core layer** in a 4-layer Clean Architecture:

```
Robolink.Core (Domain Layer) 
↓ depends on nothing, referenced by all 
Robolink.Application (Application Layer) 
↓ depends on Core 
Robolink.Infrastructure (Infrastructure Layer) 
↓ depends on Core & Application 
Robolink.WebApp / Robolink.API (Presentation Layer) 
↓ depends on all
```

### Core Responsibilities

- ✅ Define **domain entities** and aggregate roots
- ✅ Define **value types** (enums, structs)
- ✅ Define **repository interfaces** (no implementations)
- ✅ Define **domain events** and rules
- ✅ **ZERO dependencies** on external libraries (except System)
- ❌ NO Entity Framework, NO Data Access
- ❌ NO Business Logic (that goes in Application layer)

---

## Architecture & DDD

### DDD (Domain-Driven Design) Principles

| Principle | Implementation |
|-----------|-----------------|
| **Ubiquitous Language** | Entity names match business domain (Project, PhaseTask, WorkLog) |
| **Bounded Contexts** | Project, Staff, and Client are separate aggregates |
| **Aggregate Roots** | Client, Project, Staff own their child entities |
| **Value Objects** | Enums for status/state (ProjectStatus, LogStatus, etc.) |
| **Entities vs Value Objects** | Entities have identity (ID), VOs don't |
| **Repository Pattern** | Persist/retrieve aggregates as units |
| **Domain Events** | ProjectCreatedEvent, ProjectCompletedEvent (if needed) |

### Clean Architecture Layers

```
┌─────────────────────────────────────────────┐ 
│ Robolink.WebApp / Robolink.API              │ ← UI Layer 
├─────────────────────────────────────────────┤ 
│ Robolink.Application                        │ ← Use Cases / CQRS 
├─────────────────────────────────────────────┤ 
│ Robolink.Infrastructure                     │ ← Data Access / Services 
├─────────────────────────────────────────────┤ 
│ Robolink.Core (This project)                │ ← Domain Model 
└─────────────────────────────────────────────┘

```

**Dependency Rule:** Only inward (outer layers depend on inner)

---

## Entity Model

### Complete Entity Hierarchy

```
EntityBase (All entities inherit from this) 
├── Id (Guid) - Unique identifier 
├── CreatedAt (DateTime) - Audit 
├── CreatedBy (string) - Audit 
├── UpdatedAt (DateTime?) - Audit 
├── UpdatedBy (string) - Audit 
└── IsDeleted (bool) - Soft delete flag
  ↓

EntityRootBase (extends EntityBase) 

├── RowVersion (byte[]) - Concurrency control via timestamp 
└── Used by: Client, Project, Staff, SystemPhase 
└── Reason: These are AGGREGATE ROOTS (own other entities)
  ↓
Child Entities (inherit EntityBase only, no RowVersion)
 
├── PhaseTask - Child of Project aggregate 
├── WorkLog - Child of Project aggregate 
├── ProjectSystemPhaseConfig - Configuration, child of Project 
└── Reason: These are owned by aggregates, not independent
```

---

## Key Concepts: Dynamic Workflow

### ⚡ The Problem We Solved

**Before (Hard-coded):**

```
public enum ProjectPhase 
{ 
Initialize, 
Engineering, 
Fabrication, 
CNCCutting, Assembly 
} 
// ❌ To add a phase, you must: 
// 1. Edit this enum 
// 2. Rebuild the application 
// 3. Deploy new version 
// 4. Users have downtime

// ❌ Different projects MUST use the same phases 
// ❌ Can't rename phases per project
```

**After (Dynamic with SystemPhase + ProjectSystemPhaseConfig):**

// ✅ Add new phase at runtime via admin panel 

// ✅ Each project can have its own phases 

// ✅ Each project can rename/customize phases 

// ✅ No code changes needed 

// ✅ Zero downtime


### 🎯 How It Works: Three-Layer System

#### Layer 1: Global Master Data (`SystemPhase`)

**What it is:** Repository of ALL possible phases the company does.

**Example data:**

```
ID: 11111111... | Name: Initialize    | Active: true  | DefaultSeq: 1 |
ID: 22222222... | Name: Engineering   | Active: true  | DefaultSeq: 2 |
ID: 33333333... | Name: Fabrication   | Active: true  | DefaultSeq: 3 |
ID: 44444444... | Name: CNC Cutting   | Active: true  | DefaultSeq: 4 |
ID: 55555555... | Name: Assembly      | Active: true  | DefaultSeq: 5 |
ID: 66666666... | Name: Quality Check | Active: true  | DefaultSeq: 6 |
ID: 77777777... | Name: Packaging     | Active: true  | DefaultSeq: 7 |
```

**Characteristics:**

- ✅ Created ONCE during system setup (seeded in migration)
- ✅ Can add new phases anytime via admin
- ✅ Can deactivate phases (soft disable)
- ✅ Global reference for all projects
- ✅ **Business owns this data**

**Database Table:**

SystemPhases 

```
├── Id (PK, Guid) 
├── Name (nvarchar(100), unique) 
├── Description (nvarchar(max)) 
├── DefaultSequence (int) 
├── IsActive (bit) 
├── CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted (audit) 
└── Indexes: Name (unique)
```

---

#### Layer 2: Project-Specific Configuration (`ProjectSystemPhaseConfig`)

**What it is:** "Which phases does THIS project use, and in what order?"

**Example: Project A (Robot Assembly)**

Config 1: Project A 

+ Initialize   (Sequence: 1, Enabled: true) 

Config 2: Project A 

+ Engineering  (Sequence: 2, Enabled: true, CustomName: "Design Robot") 

Config 3: Project A 

+ Assembly     (Sequence: 3, Enabled: true) (Note: No Fabrication or CNC Cutting for this project)


**Example: Project B (Team Building)**

Config 1: Project B 

+ Initialize   (Sequence: 1, Enabled: true) 

Config 2: Project B 

+ Other_Check  (Sequence: 2, Enabled: true, CustomName: "Team Assessment") (Note: Very different process!)


**Characteristics:**

- ✅ Created when project is created or configured
- ✅ Many-to-Many junction between Project and SystemPhase
- ✅ Per-project customization (different name, different sequence)
- ✅ Can enable/disable phases per project
- ✅ Can change order (sequence) per project
- ✅ **Bridge between global and project-specific**

**Database Table:**

ProjectSystemPhaseConfigs 

```
├── Id (PK, Guid) 
├── ProjectId (FK to Projects) 
├── SystemPhaseId (FK to SystemPhases) 
├── Sequence (int) - Order in this project (1, 2, 3...) 
├── IsEnabled (bit) - Is this phase active for this project? 
├── CustomPhaseName (nvarchar(100)) - Override system phase name 
├── CustomDescription (nvarchar(max)) - Project-specific description 
├── CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted (audit) 
├── Unique Index: (ProjectId, SystemPhaseId) - One config per project per phase 
└── Foreign Keys: Project (cascade), SystemPhase (restrict)
```

---

#### Layer 3: Work Items (`PhaseTask`)

**What it is:** Individual tasks within a specific phase of a project.

**Example: Project A, Engineering Phase**

Task 1: "Create CAD drawings"     (assigned to Engineer1, due 2026-02-15)
 
Task 2: "Review design"            (assigned to Engineer2, due 2026-02-20) 

Task 3: "Approve for production"   (assigned to Manager,  due 2026-02-25)


**Characteristics:**

- ✅ Created by users after config is set up
- ✅ Belongs to BOTH a Project AND a ProjectSystemPhaseConfig
- ✅ Individual work items with assignments
- ✅ Track progress, due dates, status
- ✅ **Where actual work happens**

**Database Table:**

PhaseTasks 

```
├── Id (PK, Guid) 
├── ProjectId (FK to Projects) - Which project? 
├── ProjectSystemPhaseConfigId (FK) - Which phase in this project? 
├── Description (nvarchar(max)) 
├── AssignedStaffId (FK to Staffs) 
├── ProcessRate (int) - 0-100% 
├── DueDate (datetime2) 
├── Status (int) - Pending, InProgress, Review, Done 
├── ParentPhaseTaskId (FK) - For subtasks (self-referencing) 
├── CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted (audit) 
└── Foreign Keys: Project (cascade), Config (restrict), Staff (restrict)
```

---

### 📊 Visual Data Flow

```
SystemPhase (Global) 
↓ 
referenced by ProjectSystemPhaseConfig (Project Config) 
├─ ProjectId → Project 
├─ SystemPhaseId → SystemPhase 
├─ Sequence → Order in project 
├─ CustomPhaseName → Project override 
└─ IsEnabled → Exclude this phase from project? 
↓ 
used by PhaseTask (Work Item) 
├─ ProjectId → Which project? 
├─ ProjectSystemPhaseConfigId → Which config? (implies which phase)
├─ Description → What to do? 
├─ AssignedStaffId → Who does it? 
├─ DueDate → When? 
└─ Status → Progress?
```

---

### 🎬 Real-World Scenario

**Scenario: Robot Manufacturing vs Team Building**

**Setup Step 1: Global Phases (Database Seed)**

```
await db.SystemPhases.AddAsync(new SystemPhase 
{ 
Id = Guid.Parse("11111111-..."), 
Name = "Initialize", 
Description = "Project initialization", 
DefaultSequence = 1, IsActive = true 
}); // ... 5 more phases

await db.SaveChangesAsync();
```

**Setup Step 2: Create Robot Project**
```
var robotProject = new Project { 
Name = "Build Robot Arm", 
ClientId = samsungClientId, 
ManagerId = engineerManagerId 
};
// Config which phases to use var configs = new[] { 
new ProjectSystemPhaseConfig { 
ProjectId = robotProject.Id, 
SystemPhaseId = engineeringPhaseId, 
Sequence = 1, CustomPhaseName = "Design Robot Arm" 
}, 
new ProjectSystemPhaseConfig { 
ProjectId = robotProject.Id, 
SystemPhaseId = fabricationPhaseId, 
Sequence = 2 
}, 
new ProjectSystemPhaseConfig { 
ProjectId = robotProject.Id, 
SystemPhaseId = assemblyPhaseId, 
Sequence = 3 
} // Note: CNC Cutting excluded 
};
```

**Setup Step 3: Create Team Building Project**
```
var hrProject = new Project { 
Name = "Recruit Engineers 2026", 
ClientId = companyClientId, 
ManagerId = hrManagerId 
};
// Completely different phases! var hrConfigs = new[] { 
new ProjectSystemPhaseConfig { 
ProjectId = hrProject.Id, 
SystemPhaseId = initializePhaseId, 
Sequence = 1, 
CustomPhaseName = "Job Posting" 
}, 
new ProjectSystemPhaseConfig { 
ProjectId = hrProject.Id, 
SystemPhaseId = qualityCheckPhaseId,  // Repurposed! 
Sequence = 2, 
CustomPhaseName = "CV Screening" 
} };
```

**Result:**

- ✅ Same database, same system
- ✅ Robot project shows: Initialize → Design Robot Arm → Fabrication → Assembly
- ✅ HR project shows: Job Posting → CV Screening
- ✅ No code changes, no redeployment

---

## Aggregate Roots & Boundaries

### What is an Aggregate Root?

An **aggregate root** is an entity that:

1. ✅ Has its own identity (`Id`)
2. ✅ Owns child entities
3. ✅ Enforces business rules
4. ✅ Is a transaction boundary (updated as a unit)

### Robolink's Aggregate Roots

#### 1. **Client Aggregate** ✅

```
Client (Root) 
├── Id, Name, Industry, ContactEmail 
└── Children: Projects (collection)
```

Rule: When you delete a Client, all its Projects should be restricted (use DeleteBehavior.Restrict in migrations)

#### 2. **Project Aggregate** ✅

```
Project (Root) 
├── Id, Name, ProjectCode, StartDate, Deadline, Status, Priority 
├── Children: PhaseTask (collection) 
├── Children: WorkLog (collection) 
├── Children: ProjectSystemPhaseConfig (collection) 
├── Navigation: Client, Manager (Staff)
```

Rule: When you delete a Project, 

cascade-delete its PhaseTask and WorkLog
 
(use DeleteBehavior.Cascade in migrations)

Rule: You can only create PhaseTask if Project exists and config exists


#### 3. **Staff Aggregate** ✅

```
Staff (Root) 
├── Id, FullName, Username, PasswordHash, Department, Role, Status
├── Navigation: ManagedProjects (collection) 
├── Navigation: AssignedTasks (collection) 
├── Navigation: WorkLogs (collection)
```

Rule: When you delete a Staff, reassign their tasks/logs or prevent delete (use DeleteBehavior.Restrict in migrations)

#### 4. **SystemPhase Aggregate** ✅

```
SystemPhase (Root) 
├── Id, Name, Description, DefaultSequence, IsActive 
├── Children: ProjectConfigs (collection)
```

Rule: Once created, rarely changes Reference data — business owns this

### Child Entities (No RowVersion)

#### **PhaseTask** (Child of Project)

```
PhaseTask 
├── Belongs to exactly ONE Project 
├── Belongs to exactly ONE ProjectSystemPhaseConfig 
├── Cannot exist without Project 
├── Inherits EntityBase (no RowVersion - not critical for concurrency)
```

#### **ProjectSystemPhaseConfig** (Child of Project)

```
ProjectSystemPhaseConfig 
├── Belongs to exactly ONE Project 
├── References exactly ONE SystemPhase (but doesn't own it) 
├── Cannot exist without Project 
├── Inherits EntityBase (no RowVersion - configuration, not operational data)
```

#### **WorkLog** (Child of Project)

```
WorkLog 
├── Belongs to exactly ONE Project 
├── Belongs to exactly ONE PhaseTask 
├── References exactly ONE Operator (Staff) 
├── Includes numeric slots (V1-V10) and JSON data 
├── Inherits EntityRootBase (has RowVersion for concurrency)
```

Reason: Work logs are mission-critical, prevent overwrites

---

## Data Flow Examples

### Example 1: Create a New Project with Phases
```
// Step 1: Application layer receives request 
var command = new CreateProjectCommand { 
Request = new CreateProjectRequest { 
Name = "Build Robot", 
ClientId = clientId, 
ManagerId = managerId 
} };

// Step 2: Handler validates and creates project 
var project = new Project { 
Name = "Build Robot", 
ClientId = clientId, 
ManagerId = managerId, 
Status = ProjectStatus.Draft 
}; 
await projectRepo.AddAsync(project); 
await projectRepo.SaveChangesAsync();

// Step 3: Create phase configs 
var configs = new List<ProjectSystemPhaseConfig>(); 
foreach (var phase in selectedPhases) { 
configs.Add(new ProjectSystemPhaseConfig { 
ProjectId = project.Id, 
SystemPhaseId = phase.Id, 
Sequence = sequence++, 
IsEnabled = true 
}); } 
await configRepo.AddRangeAsync(configs); 
await configRepo.SaveChangesAsync();
// Result: Database now has: 
// - 1 Project row 
// - 3-5 ProjectSystemPhaseConfig rows 
// - Ready to create PhaseTasks
```

### Example 2: Create a Task in a Phase
```
// Step 1: Get project with its phases 
var project = await projectRepo.GetByIdAsync(projectId); 
var configs = await configRepo.GetByProjectIdAsync(projectId);
```

// Step 2: User selects a phase 
```
var selectedConfig = configs.First(c => c.Sequence == 1);

```

// Step 3: Create task 
```
var task = new PhaseTask { 
ProjectId = projectId, 
ProjectSystemPhaseConfigId = selectedConfig.Id,  // ← Key link! 
Description = "Create CAD design", 
AssignedStaffId = staffId, 
DueDate = DateTime.Now.AddDays(14), 
Status = Task_Status.Pending }; 
await taskRepo.AddAsync(task); 
await taskRepo.SaveChangesAsync();

// Result: Database now has: 
// - 1 PhaseTask row 
// - Can query: "Show all tasks in Engineering phase for Project X"
```
### Example 3: Disable a Phase for a Project

```
// Project A has 5 phases, 
but today they need to skip "Fabrication" 
var fabricationConfig = await configRepo .GetByProjectAndPhaseAsync(
projectAId, 
fabricationPhaseId
);
fabricationConfig.IsEnabled = false; 
await configRepo.UpdateAsync(fabricationConfig); 
await configRepo.SaveChangesAsync();

// Result: 
// ✅ Blazor component no longer shows "Fabrication" phase 
// ✅ Users can't create tasks in that phase 
// ✅ No code change, no downtime
```

---

## Enums & Value Types

All enums are in `Robolink.Core/Enums/` folder for easy import:

### Status Enums

| Enum | Values | Used By |
|------|--------|---------|
| `ProjectStatus` | Draft, InProgress, Completed, OnHold | Project |
| `ProjectPriority` | Low, Medium, High, Critical | Project |
| `Task_Status` | Pending, InProgress, Review, Done | PhaseTask |
| `StaffStatus` | Active, Inactive, Terminated | Staff |
| `LogStatus` | Success, Warning, Error, Processing, SystemLog | WorkLog |
| `LogType` | Manufacturing, Admin, HumanResource, Sales, Accounting | WorkLog |

### Classification Enums

| Enum | Values | Used By |
|------|--------|---------|
| `ProjectDepartment` | Engineering, Production, Planning, QC | Staff |
| `ProjectRole` | Admin, Manager, Lead, Operator | Staff |

---

## Repository Interfaces

All repository interfaces are in `Robolink.Core/Interfaces/`:

### Generic Repository

```
IGenericRepository<TEntity> 
├── GetByIdAsync(Guid id) 
├── GetAllAsync() 
├── FindAsync(Expression<Func<TEntity, bool>> predicate) 
├── FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) 
├── AddAsync(TEntity entity) 
├── UpdateAsync(TEntity entity) 
├── SoftDeleteAsync(Guid id) 
├── SaveChangesAsync() 
└── Used by: All entities via dependency injection
```

### Specialized Repositories

```
IProjectSystemPhaseConfigRepository 
├── GetByProjectIdAsync(Guid projectId) 
├── GetByProjectAndPhaseAsync(Guid projectId, Guid systemPhaseId) 
├── GetEnabledPhasesWithTasksAsync(Guid projectId) 
├── ProjectHasPhaseAsync(Guid projectId, Guid systemPhaseId) 
└── Prevents duplicate configurations
```

IWorkLogRepository 

```
├── GetByProjectIdAsync(Guid projectId) 
├── GetByPhaseTaskIdAsync(Guid phaseTaskId)
├── GetTotalValueByProjectAsync(Guid projectId)
├── GetPhaseTaskStatsAsync(Guid phaseTaskId)
└── Advanced querying and statistics
```
---

## Best Practices

### ✅ DO

1. **Use aggregate roots as boundaries**

```
// ✅ Good: Load entire Project with children 
var project = await projectRepo.GetByIdAsync(projectId); 
var tasks = project.Tasks;  // Navigate through aggregate
```
2. **Use repository interfaces, never DbContext directly**
```
// ✅ Good 
var project = await _projectRepo.GetByIdAsync(id);
// ❌ Bad 
var project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == id);
```
3. **Validate business rules in handlers**
```
// ✅ Good: Handler validates 
if (!await phaseConfigRepo.ProjectHasPhaseAsync(projectId, phaseId)) 
throw new InvalidOperationException("Phase already configured");
```
4. **Use soft delete consistently**
```
// ✅ Good: All queries exclude soft-deleted 
var projects = await projectRepo.GetAllAsync();  // IsDeleted = false
```

### ❌ DON'T

1. **Don't mix repository and DbContext**
```
// ❌ Bad 
var repo = new GenericRepository<Project>(context); 
var project = context.Projects.Find(id);  // Bypass repository!
```

2. **Don't create dependencies outside domain**
```
// ❌ Bad: Core shouldn't reference Infrastructure
using Robolink.Infrastructure.Data;
```

3. **Don't put business logic in entities**

```
// ❌ Bad: Logic should be in handlers or services 
public class Project { 
public void CreateTask() { }  
// NO! }
```

4. **Don't use DbContext in tests**
```
// ✅ Good: Mock repositories 
var mockRepo = new Mock<IGenericRepository<Project>>();
// ❌ Bad: Depend on real database 
var context = new AppDBContext();
```
---

## Troubleshooting

### Q: "How do I know if something should be in Core?"

**A:** If it:

- ✅ Represents a domain concept (Entity, Enum, Interface)
- ✅ Has NO external dependencies
- ✅ Is referenced by multiple layers

Then it belongs in Core.

---

### Q: "When should I use EntityRootBase vs EntityBase?"

**A:** 

- **EntityRootBase** (with RowVersion): Aggregate roots that need concurrency control

  - `Client`, `Project`, `Staff`, `SystemPhase`, `WorkLog`

- **EntityBase** (no RowVersion): Child entities or reference data

  - `PhaseTask`, `ProjectSystemPhaseConfig`

---

### Q: "Can I query SystemPhase like a normal entity?"
**A:** Yes, via repository:

```
var phases = await systemPhaseRepo.GetAllAsync(); 
var activePhases = await systemPhaseRepo.FindAsync(p => p.IsActive);
```

---

### Q: "What's the difference between ProjectSystemPhaseConfig and PhaseTask?"

**A:**

| Aspect | ProjectSystemPhaseConfig | PhaseTask |
|--------|--------------------------|-----------|
| **What** | "Project uses this phase" | "Do this task in this phase" |
| **Created by** | Project setup | Users/Team |
| **When** | Once per project per phase | Many times |
| **Has assignments?** | No | Yes (AssignedStaffId) |
| **Tracks progress?** | No | Yes (Status, ProcessRate) |

---

### Q: "How do I prevent duplicate phase configs?"

**A:** Use the unique index:

```
// Unique constraint: (ProjectId, SystemPhaseId) 
bool exists = await configRepo.ProjectHasPhaseAsync(projectId, phaseId); 
if (exists) throw new InvalidOperationException("Phase already configured");
```

---

## Summary

Robolink.Core provides:

1. **Domain Entities** — Strongly-typed business objects
2. **Aggregate Roots** — Owned hierarchies (Client → Projects, Project → Tasks)
3. **Dynamic Workflow** — SystemPhase + ProjectSystemPhaseConfig = flexible processes
4. **Repository Interfaces** — Data access contracts (implementations in Infrastructure)
5. **Enums & Value Types** — Type-safe status and classification
6. **DDD & Clean Architecture** — Maintainable, testable, business-aligned code

**Next Step:** Implement business logic in **Robolink.Application** layer using CQRS (Commands/Queries) via MediatR.

---

## References

- [README2.md](./README2.md) — Recent updates and checkpoint
- [Dynamic_Workflow_Design.md](./Entities/Dynamic_Workflow_Design.md) — Detailed workflow explanation
- [RepositoryPattern_UnitOfWork.md](./Interfaces/RepositoryPattern_UnitOfWork.md) — Repository pattern details
- [ParentID_SubID.md](./Entities/ParentID_SubID.md) — Hierarchical relationships

This comprehensive README now clearly explains:

•	✅ The three-layer workflow system (SystemPhase → Config → Task)

•	✅ Why ProjectSystemPhaseConfig is needed

•	✅ Aggregate boundaries and DDD

•	✅ Real-world scenarios

•	✅ Best practices

•	✅ Troubleshooting guide

Perfect for onboarding new developers! 🚀
