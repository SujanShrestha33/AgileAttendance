using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class DeviceConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ipaddress { get; set; }
        public int Port { get; set; }
        public int DeviceId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? LastSyncDate { get; set; }
    }
}
