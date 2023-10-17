
using Core.Entities;

namespace BiometricAttendanceSystem.ReturnDTOs
{
    public class AttendanceLogByDeviceDetails
    {
        public DeviceConfig DeviceConfigs { get; set; }
        public AttendanceLog AttendanceLogs { get; set; }
    }
}
