using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Robolink.Core.Entities
{
    /// <summary>
    /// Project-specific phase configuration.
    /// This is the junction/mapping between Project and SystemPhase.
    /// Allows each project to enable/disable phases and customize sequence.
    /// </summary>
    public class ProjectSystemPhaseConfig : EntityBase
    {
        /// <summary>Which project uses this phase config</summary>
        public Guid ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        /// <summary>Which global system phase</summary>
        public Guid SystemPhaseId { get; set; }
        [ForeignKey("SystemPhaseId")]
        public virtual SystemPhase? SystemPhase { get; set; }

        /// <summary>Sequence/order of this phase within the project (1, 2, 3...)</summary>
        public int Sequence { get; set; }

        /// <summary>Is this phase enabled for this project?</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>Optional: Project-specific phase name override</summary>
        public string? CustomPhaseName { get; set; }

        /// <summary>Optional: Project-specific description</summary>
        public string? CustomDescription { get; set; }

        // ✅ ADD THIS: Collection of tasks in this phase
        /// <summary>All tasks belonging to this project phase configuration</summary>
        public virtual ICollection<PhaseTask> PhaseTasks { get; set; } = new List<PhaseTask>();
    }
}