namespace BiometricAttendanceSystem.Common
{
    public enum SearchProperty
    {
        DeviceId,
        EnrollNumber,
        DeviceName,
        Username,
        InputDate,
        IsActive
    }

    public class SearchAttendanceLog
    {
        public SearchProperty? SearchBy { get; set; }
        public int? DeviceId { get; set; }
        public string EnrollNumber { get; set; }
        public string DeviceName { get; set; }
        public string Username { get; set; }
        public DateTime? InputDate { get; set; }
        public bool? IsActive { get; set; }
    }

}
