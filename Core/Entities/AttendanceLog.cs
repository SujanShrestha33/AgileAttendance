using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class AttendanceLog
    {
        public int LogId { get; set; }
        public string EnrollNumber { get; set; }
        public DateTime InputDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public int DeviceId { get; set; }
        
    }
}
