using System;

namespace Robolink.Shared.DTOs
{
    /// <summary>DTO for displaying Client</summary>
    public class ClientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Industry { get; set; }
        public string? ContactEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public int ProjectCount { get; set; } = 0;
    }
}