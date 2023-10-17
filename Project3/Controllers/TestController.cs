using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Project3.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly BiometricAttendanceReaderDBContext _db;
        public TestController( BiometricAttendanceReaderDBContext db)
        {    
            _db = db;
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<DeviceConfig>>> GetDeviceConfig()
        {
            return await _db.DeviceConfigs.ToListAsync();
        }
    }
}
