using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Shared.DTOs
{
    public class UpdateSystemPhaseRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int DefaultSequence { get; set; }
        public bool IsActive { get; set; }
    }
}
