using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public partial class UserInfo
    {
        public int Id { get; set; }
        public string? EnrollNumber { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public int? UserPrivilege { get; set; }
        public int DeviceId { get; set; }
        public bool? Enable { get; set; }
    }
}
