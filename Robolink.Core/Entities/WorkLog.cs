using Robolink.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema; // Thêm dòng này
using Robolink.Core.Enums;

namespace Robolink.Core.Entities
{
    public class WorkLog : EntityRootBase
    {
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")] // Chỉ định: ProjectId chính là ID của WorkProject
        public virtual Project WorkProject { get; set; } = null!;
        public Guid PhaseTaskId { get; set; }

        [ForeignKey("PhaseTaskId")] // Chỉ định: PhaseTaskId chính là ID của WorkTask
        public virtual PhaseTask WorkTask { get; set; } = null!;

        public Guid OperatorId { get; set; }

        [ForeignKey("OperatorId")]
        public virtual Staff Operator { get; set; } = null!;
        // --- PHẦN DỮ LIỆU ĐA NĂNG (QUAN TRỌNG) ---
        // 10 Slot số liệu định dạng decimal để tính toán chính xác
        public decimal? V1 { get; set; }
        public decimal? V2 { get; set; }
        public decimal? V3 { get; set; }
        public decimal? V4 { get; set; }
        public decimal? V5 { get; set; }
        public decimal? V6 { get; set; }
        public decimal? V7 { get; set; }
        public decimal? V8 { get; set; }
        public decimal? V9 { get; set; }
        public decimal? V10 { get; set; }

        // 1. Loại kết quả (Để biết đây là log của Robot hay log của HR)
        public LogType Type { get; set; } // Enum: Manufacturing, HR, Sales...

        // 2. Các chỉ số chung (Common Metrics)
        // Dùng cho Robot thì là Quantity. Dùng cho Sale thì là Revenue (Tiền). Dùng cho HR thì là CandidateCount.
        public decimal ValueMain { get; set; }
        public decimal? ValueSub { get; set; }

        // 3. Trạng thái chung
        public LogStatus Status { get; set; } // Success, Fail, Pending

        // 4. DỮ LIỆU ĐỘNG (JSON) - "VŨ KHÍ TỐI THƯỢNG"
        // Robot lưu: { "Temperature": 60, "MachineId": "R1" }
        // HR lưu: { "CandidateName": "Nguyen Van A", "CV_Link": "..." }
        // Sale lưu: { "CustomerFeedback": "Very good", "ContractId": "C123" }
        // Gộp chung vào đây để quản lý tập trung
        // Chứa cả dữ liệu Excel, Robot, HR, Sales...
        public string? DynamicDataJson { get; set; }
    }

    
    
}
