
using Core.Entities;

namespace BiometricAttendanceSystem.ViewModels
{
    public class AttendanceLogByDeviceDetails
    {
        public DeviceConfig DeviceConfigs { get; set; }
        public AttendanceLog AttendanceLogs { get; set; }
    }
}
