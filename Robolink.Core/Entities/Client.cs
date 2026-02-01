using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Core.Entities
{
    public class Client : EntityRootBase
    {
        public required string Name { get; set; } // "Ví dụ: Samsung"
        public string? Industry { get; set; } // "Ví dụ: Electronics"
        public string? ContactEmail { get; set; }
        public virtual ICollection<Project>  Projects { get; set; }  = new List<Project>();
    }
}
