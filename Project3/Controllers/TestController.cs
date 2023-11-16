using BiometricAttendanceSystem.Helper;
using BiometricAttendanceSystem.Pagination;
using BiometricAttendanceSystem.ReturnDTOs;
using Core;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace BiometricAttendance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly AttendanceRepository _repo;

        public TestController(AttendanceRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetJoinedLogs()
        {         
            try
            {
                var results = await _repo.GetJoinedLogs();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetJoinedLogsByDeviceId(int deviceId)
        {
            try
            {
                var results = await _repo.GetJoinedLogsByDeviceId(deviceId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetJoinedLogsByMultipleDeviceIds([FromQuery] string deviceIds, [FromQuery] PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
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

    }
}
