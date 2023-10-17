using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zkemkeeper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using System;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using BiometricAttendanceSystem.Pagination;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Core.Entities;

namespace BiometricAttendanceSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeviceConfigController : ControllerBase
    {
        private readonly BiometricAttendanceReaderDBContext _db;

        public DeviceConfigController(BiometricAttendanceReaderDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<IReadOnlyList<DeviceConfig>>> GetDeviceConfig()
        {
            return await _db.DeviceConfigs.ToListAsync();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AddNewDevice(DeviceConfig deviceConfig)
        {
            var device = new DeviceConfig()
            {
                Name = deviceConfig.Name,
                Ipaddress = deviceConfig.Ipaddress,
                Port = deviceConfig.Port,
                DeviceId = deviceConfig.DeviceId,
                IsActive = false,
                LastSyncDate = null
            };
     
            _db.Add(device);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("id")]
        [Route("[action]")]
        public async Task<IActionResult> RemoveDevice(int id)
        {
            var device = await _db.DeviceConfigs.FirstOrDefaultAsync(a => a.DeviceId == id);

            _db.Remove(device);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("id")]
        [Route("[action]")]
        public async Task<IActionResult> EditDeviceConfig(int id, JsonPatchDocument<DeviceConfig> deviceConfig)
        {          
            var device = await _db.DeviceConfigs.FirstOrDefaultAsync(a => a.DeviceId == id);
            deviceConfig.ApplyTo(device, ModelState);
            if (ModelState.IsValid)
            {
                _db.Update(device);
                _db.SaveChanges();
            }

            var model = new
            {
                deviceConfig = device
            };
            return Ok(model);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<List<DeviceConfig>> GetDeviceConfigCZKEM()
        {
            GetDeviceConfigLIVE();
            return await _db.DeviceConfigs.ToListAsync();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<List<DeviceConfig>> GetMultipleDevices(List<int> deviceIds)
        {
            return await _db.DeviceConfigs.Where(d => deviceIds.Contains(d.DeviceId)).ToListAsync();         
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<List<DeviceConfig>> GetMultipleDevicesCZKEM(List<int> deviceIds)
        {

            //GetDeviceConfigLIVE();
            var deviceDbData = _db.DeviceConfigs.Where(d => deviceIds.Contains(d.DeviceId)).ToList();
            var czkem = new CZKEM();

            foreach (var item in deviceDbData)
            {
                var isDeviceActive = czkem.Connect_Net(item.Ipaddress, item.Port); //Connects to Biometric Devic using IP and Port             
                var currentDevice = _db.DeviceConfigs.Find(item.Id);

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
            return await _db.DeviceConfigs.ToListAsync();
        }

        public List<DeviceConfig> GetDeviceConfigLIVE()
        {
            var deviceDbData = _db.DeviceConfigs.ToList();
            var czkem = new CZKEM();

            foreach (var item in deviceDbData)
            {
                var isDeviceActive = czkem.Connect_Net(item.Ipaddress, item.Port); //Connects to Biometric Devic using IP and Port             
                var currentDevice = _db.DeviceConfigs.Find(item.Id);

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
    
    }
}
