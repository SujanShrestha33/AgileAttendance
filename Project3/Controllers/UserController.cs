using BiometricAttendanceSystem.Helper;
using BiometricAttendanceSystem.Pagination;
using BiometricAttendanceSystem.ReturnDTOs;
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
        public UserController(BiometricAttendanceReaderDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<UserInfo>>> GetUserInfo()
        {
            var query = (from u in _db.UserInfos
                         join d in _db.DeviceConfigs on u.DeviceId equals d.DeviceId
                         select new DeviceUserInfo
                         {
                             DeviceId = d.DeviceId,
                             EnrollNumber = u.EnrollNumber,
                             DeviceName = d.Name,
                             UserName = u.Name
                         }).ToListAsync();

            return Ok(await query);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<UserInfo>>> GetUserInfoCZKEM([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            GetUserInfoLIVE();

            var query = (from u in _db.UserInfos
                         join d in _db.DeviceConfigs on u.DeviceId equals d.DeviceId
                         select new DeviceUserInfo { 
                            DeviceId = d.DeviceId,
                            EnrollNumber = u.EnrollNumber,
                            DeviceName = d.Name,
                            UserName = u.Name
                         });

            // Apply pagination
            var pagedData = await query
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalRecords = await query.CountAsync(); ;
            var pagedResponse = PaginationHelper.CreatePagedReponse<DeviceUserInfo>(pagedData, validFilter, totalRecords);
            return Ok(pagedResponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<UserInfo>>> GetUserInfoOfMultipleDevicesCZKEM(List<int> deviceIds, [FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var deviceConfigs = _db.DeviceConfigs.Where(d => deviceIds.Contains(d.DeviceId)).ToList();
            if (deviceConfigs.Count > 0)
            {
                foreach (var deviceConfig in deviceConfigs)
                {
                    var userInfo = GetUserInfoLIVE();
                    if (userInfo.Count > 0)
                    {
                        UpdateUserInfo(userInfo,deviceConfig.DeviceId);
                    }
                }
            }

            var query = (from u in _db.UserInfos
                         join d in _db.DeviceConfigs on u.DeviceId equals d.DeviceId                       
                         select new DeviceUserInfo
                         {
                             DeviceId = d.DeviceId,
                             EnrollNumber = u.EnrollNumber,
                             DeviceName = d.Name,
                             UserName = u.Name
                         });

            // Apply pagination
            var pagedData = await query
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalRecords = await query.CountAsync(); ;
            var pagedResponse = PaginationHelper.CreatePagedReponse<DeviceUserInfo>(pagedData, validFilter, totalRecords);
            return Ok(pagedResponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserAttendanceLog([FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var query = (from d in _db.DeviceConfigs
                      join a in _db.AttendanceLogs on d.DeviceId equals a.DeviceId
                      join u in _db.UserInfos on a.EnrollNumber equals u.EnrollNumber
                      select new UserAttendanceLog
                      {
                          DeviceId = d.DeviceId,
                          EnrollNumber = a.EnrollNumber,
                          InputDate = a.InputDate,
                          CreatedOn = a.CreatedOn,
                          DeviceName = d.Name,
                          Username = u.Name,
                          IsActive = d.IsActive
                      }).Distinct();        

            // Apply pagination
            var pagedData = await query
                .OrderByDescending(x => x.InputDate)
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToListAsync();

            var totalRecords = await query.CountAsync(); ;
            var pagedResponse = PaginationHelper.CreatePagedReponse<UserAttendanceLog>(pagedData, validFilter, totalRecords);
            return Ok(pagedResponse);
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

        static private int UpdateUserInfo(List<UserInfo> userInfo, int deviceId)
        {
            int rowsCount = 0;
            List<UserInfo> userInfos;
           
            //get all users of particular device
            userInfos = userInfo.FindAll(x => x.DeviceId == deviceId);

            foreach (var user in userInfos)
            {          
                //check if user exists in the database
                bool userExists = _db.UserInfos.Any(u => u.EnrollNumber == user.EnrollNumber);

                if (!userExists)
                {        
                    _db.UserInfos.Add(user);
                    rowsCount++;
                }
            }

            _db.SaveChanges();
            return rowsCount;
        }
      
    }
}
