using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zkemkeeper;

namespace Infrastructure.Services
{
    public interface IDeviceConfigRepository
    {
        public List<DeviceConfig> GetDeviceConfigLIVE();
    }

    public class DeviceConfigRepository : IDeviceConfigRepository
    {
        private readonly BiometricAttendanceReaderDBContext _db;
        public DeviceConfigRepository(BiometricAttendanceReaderDBContext db)
        {
            _db = db;
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
