using Robolink.Core.Common;
using System;
using System.Collections.Generic;

namespace Robolink.Core.Entities
{
    /// <summary>
    /// Global master data for all possible project phases.
    /// This is reference data that never changes — initialized once.
    /// Example: Initialize, Engineering, Fabrication, CNC Cutting, Assembly
    /// </summary>
    public class SystemPhase : EntityBase
    {
        /// <summary>Phase name (e.g., "Engineering", "Fabrication")</summary>
        public required string Name { get; set; }

        /// <summary>Detailed description</summary>
        public string? Description { get; set; }

        /// <summary>Default sequence if this phase is used</summary>
        public int DefaultSequence { get; set; }

        /// <summary>Is this phase active/available for projects?</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Projects that include this phase (through ProjectSystemPhaseConfig)</summary>
        public virtual ICollection<ProjectSystemPhaseConfig> ProjectConfigs { get; set; } = new List<ProjectSystemPhaseConfig>();
    }
}
