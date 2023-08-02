using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RustDeskAPI.Models.DBModel
{
    public class rustdesk_token
    {
        public int id { get; set; }
        public string username { get; set; }
        public string uid { get; set; }
        public string client_id { get; set; }
        public string uuid { get; set; }
        public string access_token { get; set; }
        public DateTime? login_time { get; set; }
    }
}