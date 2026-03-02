using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.DTOs
{
    public class AssignPhaseRequest
    {
        public Guid ProjectId { get; set; }
        public Guid SystemPhaseId { get; set; }
        public string? CustomPhaseName { get; set; }
    }
}
