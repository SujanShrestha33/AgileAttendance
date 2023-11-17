using Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class AttendanceRepository
    {
        public static AttendanceDBContext _db;
        public AttendanceRepository(AttendanceDBContext db)
        {
            _db = db;
        }

        public async Task<List<AttendanceLogByDeviceDetails>> GetJoinedLogs()
        {
            var latestAttendanceLogs = await _db.AttendanceLogByDeviceDetails
                                     .FromSqlRaw("EXEC dbo.GetLatestAttendanceLogs")
                                     .ToListAsync();

            return latestAttendanceLogs;
        }

        public async Task<List<AttendanceLogByDeviceDetails>> GetJoinedLogsByDeviceId(int deviceId)
        {
            var latestAttendanceLogs = await _db.AttendanceLogByDeviceDetails
                                     .FromSqlRaw("EXEC dbo.GetLatestAttendanceLogsByDeviceId @DeviceId",
                                     new SqlParameter("@DeviceId", deviceId))
                                     .ToListAsync();

            return latestAttendanceLogs;
        }

        public async Task<List<AttendanceLogByDeviceDetails>> GetLatestAttendanceLogsByMultipleDeviceIdsAsync(string deviceIds)
        {
            var results = await _db.AttendanceLogByDeviceDetails
                .FromSqlRaw("EXEC dbo.GetLatestAttendanceLogsByMultipleDeviceIds @DeviceIds", new SqlParameter("@DeviceIds", deviceIds))
                .ToListAsync();

            return results;
        }
    }
}
