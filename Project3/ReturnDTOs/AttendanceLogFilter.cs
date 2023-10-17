namespace BiometricAttendanceSystem.ReturnDTOs
{
    public class AttendanceLogFilter
    {
        public int? DeviceId { get; set; }
        public string EnrollNumber { get; set; }
        public string DeviceName { get; set; }
        public string Username { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }

    }

    
}
