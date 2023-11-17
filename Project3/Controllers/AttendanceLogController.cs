using BiometricAttendanceSystem.Helper;
using BiometricAttendanceSystem.Pagination;
using BiometricAttendanceSystem.ReturnDTOs;
using Core;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using zkemkeeper;

namespace BiometricAttendanceSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttendancelogController : ControllerBase
    {
        private static AttendanceDBContext _db;
        private readonly AttendanceRepository _repo;
        public AttendancelogController(AttendanceDBContext db, AttendanceRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        [HttpGet("filter")]
        [Route("[action]")]
        public async Task<IActionResult> GetFilteredAttendance([FromQuery] PaginationFilter pagefilter, [FromQuery] AttendanceLogFilter filter)
        {
            var validFilter = new PaginationFilter(pagefilter.PageNumber, pagefilter.PageSize);

            //get data using stored procedure
            var results = await _repo.GetJoinedLogs();
            var query = results.AsQueryable();

            if (filter.DeviceId.HasValue)
            {
                query = query.Where(a => a.DeviceID == filter.DeviceId);
            }

            if (!string.IsNullOrEmpty(filter.EnrollNumber))
            {
                query = query.Where(a => a.EnrollNumber == filter.EnrollNumber);
            }

            if (!string.IsNullOrEmpty(filter.DeviceName))
            {
                query = query.Where(a => a.DeviceName.StartsWith(filter.DeviceName));
            }

            if (!string.IsNullOrEmpty(filter.Username))
            {
                query = query.Where(a => a.Username.StartsWith(filter.Username));
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(a => a.InputDate >= filter.StartDate);
            }
            if (filter.EndDate.HasValue)
            {
                var endDate = filter.EndDate.Value.AddDays(1);
                query = query.Where(a => a.InputDate <= endDate);
            }

            if (!string.IsNullOrEmpty(filter.InOutMode.ToString()))
            {
                query = query.Where(a => a.InOutMode == filter.InOutMode);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(a => a.IsActive == filter.IsActive);
            }

            
            var pagedData = query
                           .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                           .Take(validFilter.PageSize)
                           .ToList();

            var totalRecords = query.Count(); 
            var pagedResponse = PaginationHelper.CreatePagedReponse<AttendanceLogByDeviceDetails>(pagedData, validFilter, totalRecords);
            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<AttendanceLog>>> GetUserAttendanceLogOfMultipleDevicesLIVE([FromQuery] string deviceIds, [FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var deviceConfigs = _db.DeviceConfigs.Where(d => deviceIds.Contains(d.DeviceId.ToString())).ToList();
          
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
                _db.SaveChanges();

            try
            {
                var results = await _repo.GetLatestAttendanceLogsByMultipleDeviceIdsAsync(deviceIds);
                var attendanceLogs = results.ToList();

                var pagedData = attendanceLogs
                                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                .Take(validFilter.PageSize)
                                .ToList();

                var totalRecords = attendanceLogs.Count(); ;
                var pagedResponse = PaginationHelper.CreatePagedReponse<AttendanceLogByDeviceDetails>(pagedData, validFilter, totalRecords);
                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
      
        }

        [HttpGet("getupdatedattendancelog")]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<AttendanceLog>>> GetUpdatedAttendanceLog([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

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

            try
            {
                var results = await _repo.GetJoinedLogs();
                var attendanceLogs = results.ToList();

                var pagedData = attendanceLogs
                                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                .Take(validFilter.PageSize)
                                .ToList();

                var totalRecords = attendanceLogs.Count(); ;
                var pagedResponse = PaginationHelper.CreatePagedReponse<AttendanceLogByDeviceDetails>(pagedData, validFilter, totalRecords);
                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        static private int UpdateAttendanceLogs(List<AttendanceLog> attendanceLogs, int deviceId)
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
            var latestLogs = attendanceLogs.FindAll(log => log.InputDate >= lastCreatedOn);

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
        public List<DeviceConfig> GetDeviceConfigLIVE()
        {
            var deviceDbData = _db.DeviceConfigs.ToList();
            var czkem = new CZKEM();

            foreach (var item in deviceDbData)
            {
                var isDeviceActive = czkem.Connect_Net(item.Ipaddress, item.Port); //Connects to Biometric Devic using IP and Port             
                var currentDevice = _db.DeviceConfigs.Find(item.DeviceId);

                if (currentDevice != null)
                {
                    currentDevice.IsActive = isDeviceActive;
                    if (isDeviceActive == true)
                    {
                        currentDevice.LastSyncDate = DateTime.Now;
                    }
                    _db.SaveChanges();
                }
            }
            return _db.DeviceConfigs.ToList();
        }
        bool ShouldSkipDevice(int deviceId)
        {
            //List<int> deviceIdsToSkip = new List<int> { };
            List<int> deviceIdsToSkip = new List<int>
            {
             3, 13, 14, 15, 17, 19, 23, 24, 31, 32, 33, 34, 35, 36, 37, 38, 47,
             83, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96
            };
            return deviceIdsToSkip.Contains(deviceId);
        }
    }
}
