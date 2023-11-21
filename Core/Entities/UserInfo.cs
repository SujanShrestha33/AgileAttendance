using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class UserInfo
    {
        public string EnrollNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int UserPrivilege { get; set; }
        public bool? Enable { get; set; }
        public int DeviceId { get; set; }
        public int Id { get; set; }
    }
}
