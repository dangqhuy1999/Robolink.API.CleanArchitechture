using System;
using System.Collections.Generic;
using System.Text;

namespace Robolink.Core.Enums
{
    public enum Task_Status
    {
        Pending = 0,      // ✅ Default
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,    // ✅ Optional
        OnHold = 4        // ✅ Optional
    }
}
