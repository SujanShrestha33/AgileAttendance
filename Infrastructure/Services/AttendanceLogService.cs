using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;

namespace Infrastructure.Services
{
    public interface IAttendanceLogRepository
    {
        public List<AttendanceLog> GetAttendanceLogListLIVE();
    }

    public class AttendanceLogRepository : IAttendanceLogRepository
    {
        private readonly BiometricAttendanceReaderDBContext _db;
        public AttendanceLogRepository(BiometricAttendanceReaderDBContext db)
        {
            _db = db;
        }

        public List<AttendanceLog> GetAttendanceLogListLIVE()
        {
           
            var deviceConfigs = _db.DeviceConfigs.ToList();

            foreach (var deviceConfig in deviceConfigs)
            {
                var attendanceLogs = GetAttendanceLogsCZKEM(deviceConfig);
                if (attendanceLogs.Count > 0)
                {
                    if (deviceConfig.LastSyncDate.HasValue)
                    {
                        deviceConfig.LastSyncDate = deviceConfig.LastSyncDate.Value.AddDays(-7);
                    }
                    UpdateAttendanceLogs(attendanceLogs, deviceConfig.DeviceId);
                }
            }
            Console.WriteLine("AttendanceLog Scheduling success..\n");
            return _db.AttendanceLogs.ToList();
        }

        private int UpdateAttendanceLogs(List<AttendanceLog> attendanceLogs, int deviceId)
        {
            int rowsCount = 0;
            //List<AttendanceLog> attenLog;

            var lastCreatedDates = _db.AttendanceLogs
                                    .GroupBy(log => log.DeviceId)
                                    .Select(group => new
                                    {
                                        DeviceId = group.Key,
                                        LastCreatedOn = group.Max(log => log.CreatedOn)
                                    })
                                    .ToDictionary(item => item.DeviceId, item => item.LastCreatedOn);

            var forthisdevice = lastCreatedDates.TryGetValue(deviceId, out var lastCreatedOn);
            if (forthisdevice == false)
            {
                _db.AttendanceLogs.AddRange(attendanceLogs);
                rowsCount++;
            }
            else
            {
                var latestLogs = attendanceLogs.FindAll(log => log.InputDate >= lastCreatedOn);
                _db.AttendanceLogs.AddRange(latestLogs);
                rowsCount++;
            }

            _db.SaveChanges();

            return rowsCount;
        }
        public List<AttendanceLog> GetAttendanceLogsCZKEM(DeviceConfig deviceConfig)
        {
            var attendanceLogs = new List<AttendanceLog>();
            var czkem = new CZKEM();

            var isDeviceActive = czkem.Connect_Net(deviceConfig.Ipaddress, deviceConfig.Port);
            if (isDeviceActive)
            {
                string dwEnrollNumber = "";
                int dwVerifyMode = 0;
                int dwInOutMode = 0;
                int dwYear = 0;
                int dwMonth = 0;
                int dwDay = 0;
                int dwHour = 0;
                int dwMinute = 0;
                int dwSecond = 0;
                int dwWorkCode = 0;

                if (!ShouldSkipDevice(deviceConfig.DeviceId))
                {
                    while (czkem.SSR_GetGeneralLogData(deviceConfig.DeviceId, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                    {
                        attendanceLogs.Add(new AttendanceLog
                        {
                            DeviceId = deviceConfig.DeviceId,
                            EnrollNumber = dwEnrollNumber,
                            InputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond),
                            CreatedOn = DateTime.Now,
                            InOutMode = dwInOutMode
                        });
                    }
                }
            }

            return attendanceLogs;
        }
        bool ShouldSkipDevice(int deviceId)
        {
            List<int> deviceIdsToSkip = new List<int> { };
            //List<int> deviceIdsToSkip = new List<int>
            //{
            // 3, 13, 14, 15, 17, 19, 23, 24, 31, 32, 33, 34, 35, 36, 37, 38, 47,
            // 83, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96
            //};
            return deviceIdsToSkip.Contains(deviceId);
        }
    }
}