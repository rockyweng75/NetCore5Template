using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCore5_Domain.Models
{
    [Serializable]
    public class UserInfo
    {
        public string UserId { get; set; }
        public string Password { get; set; }

        public string UserName { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();

        public string DeptCode { get; set; }

        public string Status { get; set; }

        public string JobTitle { get; set; }

        public string Email { get; set; }

    }
}