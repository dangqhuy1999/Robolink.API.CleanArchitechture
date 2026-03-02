using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.DTOs
{
    public class UpdateProjectPhaseConfigRequest
    {
        public Guid Id { get; set; }
        public string? CustomPhaseName { get; set; }
        public int Sequence { get; set; }
        public bool IsEnabled { get; set; }
    }
}
