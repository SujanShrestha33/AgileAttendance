﻿

namespace BiometricAttendanceSystem.ReturnDTOs
{
    public class UsersAttendanceLogByDeviceDetails
    {
        public int DeviceId { get; set; }
        public string EnrollNumber { get; set; }
        public string DeviceName { get; set; }
        public string Username { get; set; }
        public DateTime InputDate { get; set; }
        public bool? IsActive { get; set; }
        public int? InOutMode { get; set; }
    }
}
