

namespace BiometricAttendanceSystem.ReturnDTOs
{
    public class UserAttendanceLogByDeviceDetails
    {
        public int DeviceId { get; set; }
        public string EnrollNumber { get; set; }
        public string DeviceName { get; set; }
        public string Username { get; set; }
        public DateTime InputDate { get; set; }
        public bool? IsActive { get; set; }
    }
}
