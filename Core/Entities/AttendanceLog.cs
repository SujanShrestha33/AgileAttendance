using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class AttendanceLog
    {
        public int DeviceId { get; set; }
        public string EnrollNumber { get; set; } = null!;
        public DateTime InputDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int LogId { get; set; }
        public int? InOutMode { get; set; }
    }
}
