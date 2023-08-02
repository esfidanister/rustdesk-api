using RustDeskAPI.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RustDeskAPI.Models
{
    public class abRequest
    {
        public List<string> tags { get; set; }
        public List<rustdesk_peers> peers { get; set; }
    }

}