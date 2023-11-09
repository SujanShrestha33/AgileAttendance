using BiometricAttendanceSystem.Helper;
using BiometricAttendanceSystem.Pagination;
using BiometricAttendanceSystem.ReturnDTOs;
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
        private static BiometricAttendanceReaderDBContext _db;
        public AttendancelogController(BiometricAttendanceReaderDBContext db)
        {
            _db = db;
        }

        [HttpGet("filter")]
        [Route("[action]")]
        public async Task<IActionResult> GetFilteredAttendance([FromQuery] PaginationFilter pagefilter, [FromQuery] AttendanceLogFilter filter)
        {
            var validFilter = new PaginationFilter(pagefilter.PageNumber, pagefilter.PageSize);

            //inner join of DeviceConfig, UserInfo and AttendanceLog
            var userAttendanceLogOfMultipleDevice = (from a in _db.AttendanceLogs
                                                     join d in _db.DeviceConfigs on a.DeviceId equals d.DeviceId
                                                     join u in _db.UserInfos on a.EnrollNumber equals u.EnrollNumber
                                                     select new UserAttendanceLogByDeviceDetails
                                                     {
                                                         DeviceId = d.DeviceId,
                                                         EnrollNumber = a.EnrollNumber,
                                                         DeviceName = d.Name,
                                                         Username = u.Name,
                                                         InputDate = a.InputDate,
                                                         InOutMode = a.InOutMode,
                                                         IsActive = d.IsActive,
                                                     }).Distinct();

            var query = userAttendanceLogOfMultipleDevice.AsQueryable();

            if (filter.DeviceId.HasValue)
            {
                query = query.Where(a => a.DeviceId == filter.DeviceId);
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

            // Execute the query and return the filtered results
            //Apply Pagination
            var pagedData = await query              
                .OrderByDescending(x => x.InputDate)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalRecords = await query.CountAsync(); ;
            var pagedResponse = PaginationHelper.CreatePagedReponse<UserAttendanceLogByDeviceDetails>(pagedData, validFilter, totalRecords);
            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<AttendanceLog>>> GetUserAttendanceLogOfMultipleDevicesLIVE(List<int> deviceIds/*, [FromQuery] PaginationFilter filter*/)
        {
            //var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var deviceConfigs = _db.DeviceConfigs.Where(d => deviceIds.Contains(d.DeviceId)).ToList();
          
                foreach (var deviceConfig in deviceConfigs)
                {
                    var attendanceLogs = GetAttendanceLogsCZKEMAtOnce(deviceConfig);

                    var existingLogs = _db.AttendanceLogs.Where(log => log.DeviceId == deviceConfig.DeviceId);
                    _db.AttendanceLogs.RemoveRange(existingLogs);

                    _db.AttendanceLogs.AddRange(attendanceLogs);
                    //if (attendanceLogs.Count > 0)
                    //{
                    //    if (deviceConfig.LastSyncDate.HasValue)
                    //    {
                    //        deviceConfig.LastSyncDate = deviceConfig.LastSyncDate.Value.AddDays(-7);
                    //    }
                    //    UpdateAttendanceLogs(deviceConfig.Name, attendanceLogs, deviceConfig.Ipaddress, deviceConfig.Port, deviceConfig.LastSyncDate);
                    //}
                }
                _db.SaveChanges();           

            //GetUserInfoLIVE();

            //inner join of DeviceConfig, UserInfo and AttendanceLog
            var userAttendanceLogOfMultipleDevice = (from a in _db.AttendanceLogs
                                                     join d in _db.DeviceConfigs on a.DeviceId equals d.DeviceId
                                                     join u in _db.UserInfos on a.EnrollNumber equals u.EnrollNumber
                                                     where deviceIds.Contains(d.DeviceId)
                                                     select new UserAttendanceLogByDeviceDetails
                                                     {
                                                         DeviceId = d.DeviceId,
                                                         EnrollNumber = a.EnrollNumber,
                                                         DeviceName = d.Name,
                                                         Username = u.Name,
                                                         InputDate = a.InputDate,
                                                         InOutMode = a.InOutMode,
                                                         IsActive = d.IsActive,
                                                     }).Distinct();

            var pagedData = await userAttendanceLogOfMultipleDevice               
                .OrderByDescending(x => x.InputDate)
                //.Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                //.Take(validFilter.PageSize)
                .ToListAsync();

            //var totalRecords = await userAttendanceLogOfMultipleDevice.CountAsync(); ;
            //var pagedResponse = PaginationHelper.CreatePagedReponse<UserAttendanceLogByDeviceDetails>(pagedData, validFilter, totalRecords);
            return Ok(pagedData);
        }

        [HttpGet("getupdatedattendancelog")]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<AttendanceLog>>> GetUpdatedAttendanceLog()
        {
            List<DeviceConfig> deviceConfigs = GetDeviceConfigLIVE();

            if (deviceConfigs.Count > 0)
            {
                foreach (var deviceConfig in deviceConfigs)
                {
                    var attendanceLogs = GetAttendanceLogsCZKEM(deviceConfig);
                    if (attendanceLogs.Count > 0)
                    {
                        if (deviceConfig.LastSyncDate.HasValue)
                        {
                            deviceConfig.LastSyncDate = deviceConfig.LastSyncDate.Value.AddDays(-7);
                        }
                        UpdateAttendanceLogs(deviceConfig.Name, attendanceLogs, deviceConfig.Ipaddress, deviceConfig.Port, deviceConfig.LastSyncDate);
                    }
                }
            }
            //inner join of DeviceConfig, UserInfo and AttendanceLog
            var query = (from a in _db.AttendanceLogs
                         join d in _db.DeviceConfigs on a.DeviceId equals d.DeviceId
                         join u in _db.UserInfos on a.EnrollNumber equals u.EnrollNumber
                         select new UserAttendanceLogByDeviceDetails
                         {
                             DeviceId = d.DeviceId,
                             EnrollNumber = a.EnrollNumber,
                             DeviceName = d.Name,
                             Username = u.Name,
                             InputDate = a.InputDate,
                             InOutMode = a.InOutMode,
                             IsActive = d.IsActive,
                         }).ToListAsync();

            return Ok(await query);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<AttendanceLog>>> GetUpdatedAttendanceLogNew( [FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            List<DeviceConfig> deviceConfigs = _db.DeviceConfigs.ToList();
            List<AttendanceLog> attendanceLogs = new List<AttendanceLog>();

            foreach (var deviceConfig in deviceConfigs)
            {
                attendanceLogs = GetAttendanceLogsCZKEMAtOnce(deviceConfig);

                var existingLogs = _db.AttendanceLogs.Where(log => log.DeviceId == deviceConfig.DeviceId);
                _db.AttendanceLogs.RemoveRange(existingLogs);

                _db.AttendanceLogs.AddRange(attendanceLogs);              
            }
            _db.SaveChanges();

            //inner join of DeviceConfig, UserInfo and AttendanceLog
            // var query = (from a in _db.AttendanceLogs
            //              join d in _db.DeviceConfigs on a.DeviceId equals d.DeviceId
            //              join u in _db.UserInfos on a.EnrollNumber equals u.EnrollNumber
            //              select new UserAttendanceLogByDeviceDetails
            //              {
            //                  DeviceId = d.DeviceId,
            //                  EnrollNumber = a.EnrollNumber,
            //                  DeviceName = d.Name,
            //                  Username = u.Name,
            //                  InputDate = a.InputDate,
            //                  InOutMode = a.InOutMode,
            //                  IsActive = d.IsActive,
            //              }).Distinct();

            //var pagedData = await query
            //   .OrderByDescending(x => x.InputDate)
            //   .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            //   .Take(validFilter.PageSize)
            //   .ToListAsync();

            //var totalRecords = await query.CountAsync(); ;
            //var pagedResponse = PaginationHelper.CreatePagedReponse<UserAttendanceLogByDeviceDetails>(pagedData, validFilter, totalRecords);
            //return Ok(pagedResponse);

            return Ok(await _db.AttendanceLogs.ToListAsync());

        }

        static private int UpdateAttendanceLogs(string deviceName, List<AttendanceLog> attendanceLogs, string ipAddress, int port, 	DateTime? syncedDate)
        {
            int rowsCount = 0;
            List<AttendanceLog> attenLog;

            if (syncedDate == null)
            {
                attenLog = attendanceLogs;
            }
            else
            {
                attenLog = attendanceLogs.FindAll(x => x.InputDate >= syncedDate);
            }

            var lastCreatedDates = _db.AttendanceLogs
                                    .GroupBy(log => log.DeviceId)
                                    .Select(group => new
                                    {
                                        DeviceId = group.Key,
                                        LastCreatedOn = group.Max(log => log.CreatedOn)
                                    })
                                    .ToDictionary(item => item.DeviceId, item => item.LastCreatedOn);

            foreach (var newLog in attenLog)
            {
                if (!lastCreatedDates.TryGetValue(newLog.DeviceId, out var lastCreatedOn) || newLog.InputDate > lastCreatedOn)
                {
                    _db.AttendanceLogs.Add(newLog);
                }
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


                //czkem.ReadTimeGLogData(deviceConfig.DeviceId, string sTime, string eTime);
                //out keyword is used to pass arguments as referens, Used when method returns multiple value
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

            return attendanceLogs;
        }

        public List<AttendanceLog> GetAttendanceLogsCZKEMAtOnce(DeviceConfig deviceConfig)
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

                //if (!ShouldSkipDevice(deviceConfig.DeviceId))
                //{
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
                //}

                //while (czkem.SSR_GetGeneralLogData(75, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode))
                //{
                //    attendanceLogs.Add(new AttendanceLog
                //    {
                //        DeviceId = 75,
                //        EnrollNumber = dwEnrollNumber,
                //        InputDate = new DateTime(dwYear, dwMonth, dwDay, dwHour, dwMinute, dwSecond),
                //        CreatedOn = DateTime.Now,
                //        InOutMode = dwInOutMode
                //    });
                //}
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
        public List<UserInfo> GetUserInfoLIVE()
        {
            var userInfo = new List<UserInfo>();
            var deviceConfigs = _db.DeviceConfigs.ToList();
            var czkem = new CZKEM();

            foreach (var deviceConfig in deviceConfigs)
            {
                // Connects to Biometric Device using IP and Port
                var isDeviceActive = czkem.Connect_Net(deviceConfig.Ipaddress, deviceConfig.Port);
                if (isDeviceActive != true)
                {
                    continue;
                }
                string dwEnrollNumber = "";
                string dwName = "";
                string dwPassword = "";
                int dwUserPrivilege = 0;
                bool dwEnable = false;

                while (czkem.SSR_GetAllUserInfo(deviceConfig.DeviceId, out dwEnrollNumber, out dwName, out dwPassword, out dwUserPrivilege, out dwEnable))
                {
                    userInfo.Add(new UserInfo
                    {
                        DeviceId = deviceConfig.DeviceId,
                        EnrollNumber = dwEnrollNumber, // Use the original enrollment number from attendance log
                        Name = dwName,
                        Password = dwPassword,
                        UserPrivilege = dwUserPrivilege,
                    });
                }

                var log = _db.UserInfos.ToList();
                foreach (var user in userInfo)
                {
                    var existingUser = _db.UserInfos.FirstOrDefault(u => u.DeviceId == user.DeviceId && u.EnrollNumber == user.EnrollNumber);
                    if (existingUser == null)
                    {
                        _db.UserInfos.Add(user);
                        _db.SaveChanges();
                    }
                }
            }
            return _db.UserInfos.ToList();
        }

        //bool ShouldSkipDevice(int deviceId)
        //{
        //    //List<int> deviceIdsToSkip = new List<int> {  };
        //    List<int> deviceIdsToSkip = new List<int>
        //    {
        //     3, 13, 14, 15, 17, 19, 23, 24, 31, 32, 33, 34, 35, 36, 37, 38, 47,
        //     83, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96
        //    };
        //    return deviceIdsToSkip.Contains(deviceId);
        //}
    }
}
