﻿using BiometricAttendanceSystem.Helper;
using BiometricAttendanceSystem.Pagination;
using BiometricAttendanceSystem.ReturnDTOs;
using Core;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zkemkeeper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BiometricAttendanceSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private static BiometricAttendanceReaderDBContext _db;
        private readonly AttendanceRepository _repo;
        public UserController(BiometricAttendanceReaderDBContext db,AttendanceRepository repo)
        {
            _db = db;
            _repo = repo;
        }

        //Get all UserInfo from database
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<UserInfo>>> GetUserInfo()
        {
            //var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            //var query = (from u in _db.UserInfos
            //             join d in _db.DeviceConfigs on u.DeviceId equals d.DeviceId
            //             select new DeviceUserInfo
            //             {
            //                 DeviceId = u.DeviceId,
            //                 EnrollNumber = u.EnrollNumber,
            //                 DeviceName = d.Name,
            //                 UserName = u.Name
            //             }).ToListAsync();

            var query = await _db.UserInfos.OrderBy(x => x.DeviceId).ToListAsync();

            // Apply pagination
            //var pagedData = await query
            //    .OrderBy(x => x.DeviceId)
            //    .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            //    .Take(validFilter.PageSize)
            //    .ToListAsync();

            //var totalRecords = await query.CountAsync(); ;
            //var pagedResponse = PaginationHelper.CreatePagedReponse<DeviceUserInfo>(pagedData, validFilter, totalRecords);
            return Ok(query);

        }

        //Get all UserInfo from device - Live
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<UserInfo>>> GetUserInfoCZKEM()
        {          
            var deviceConfigs = _db.DeviceConfigs.ToList();
            GetUserInfoLIVE(deviceConfigs);

            var query = await _db.UserInfos.OrderBy(x=>x.DeviceId).ToListAsync();

            return Ok(query);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<UserInfo>>> GetUserInfoOfMultipleDevicesCZKEM(List<int> deviceIds)
        {

            var deviceConfigs = _db.DeviceConfigs.Where(d => deviceIds.Contains(d.DeviceId)).ToList();
            if (deviceConfigs.Count > 0)
            {
                var userInfo = GetUserInfoLIVE(deviceConfigs);
            }

            var query = (from u in _db.UserInfos
                         where deviceIds.Contains(u.DeviceId)
                         select u).ToListAsync();
            
            return Ok(await query);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserAttendanceLog([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

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

        public List<UserInfo> GetUserInfoLIVE(List<DeviceConfig> deviceConfigs)
        {
            var userInfo = new List<UserInfo>();
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

                var existingUsers = _db.UserInfos.Where(x => x.DeviceId == deviceConfig.DeviceId);
                var userCount = existingUsers.Count();
                _db.UserInfos.RemoveRange(existingUsers);
            }
            _db.UserInfos.AddRange(userInfo);              
            _db.SaveChanges();

            return _db.UserInfos.ToList();
        }
    }
}
