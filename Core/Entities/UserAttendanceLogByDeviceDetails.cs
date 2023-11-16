using Microsoft.EntityFrameworkCore;

namespace Core.Entities
{
    [Keyless]
    public class AttendanceLogByDeviceDetails
    {
        public int LogID { get; set; }
        public int DeviceID { get; set; }
        public string? EnrollNumber { get; set; }
        public DateTime InputDate { get; set; }
        public int InOutMode { get; set; }
        public string? DeviceName { get; set; }
        public string? Username { get; set; }
    }
}