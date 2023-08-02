using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RustDeskAPI.Models.DBModel
{
    public class rustdesk_users
    {
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string note { get; set; }
        public int status { get; set; }
        public string group { get; set; }
        public bool is_admin { get; set; }
        public DateTime create_time { get; set; }
        public DateTime? delete_time { get; set; }
        public bool isdeleted { get; set; }
    }
}