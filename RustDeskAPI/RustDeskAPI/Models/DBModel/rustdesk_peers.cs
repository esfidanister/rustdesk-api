using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RustDeskAPI.Models.DBModel
{
    public class rustdesk_peers
    {
        public int id { get; set; }
        public int uid { get; set; }
        public string client_id { get; set; }
        public string username { get; set; }
        public string hostname { get; set; }
        public string alias { get; set; }
        public string platform { get; set; }
        public List<string> tags { get; set; }
        public string forceAlwaysRelay { get; set; }
        public string rdpPort { get; set; }
        public string rdpUsername { get; set; }
    }
}