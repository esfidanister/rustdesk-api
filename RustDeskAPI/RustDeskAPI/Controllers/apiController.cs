using RustDeskAPI.Models;
using RustDeskAPI.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace RustDeskAPI.Controllers
{
    public class apiController : Controller
    {
        // GET: api
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string login(string username, string password, string id, string uuid)
        {
            string pwd = Utility.GetMd5(password + "rustdesk");
            string sql = "select * from rustdesk_users where `username` ='" + username + "' and `password` = '" + pwd + "' ;";
            DataTable dt = MySQLHelper.QueryDataTable(sql);
            List<rustdesk_users> rulst = JsonConvert.DeserializeObject<List<rustdesk_users>>(JsonConvert.SerializeObject(dt));
            if (rulst.Count > 0)
            {
                var ru = rulst.First();
                DateTime dTime = DateTime.Now;
                string Token = Utility.GetMd5("rustdesk" + dTime.ToString("yyyyMMddHHmmss"));
                var result = new
                {
                    type = "access_token",
                    access_token = Token,
                    user = new
                    {
                        name = username,
                        ru.email,
                        ru.note,
                        ru.status,
                        grp = ru.group,
                        ru.is_admin,
                    },
                };
                sql = string.Format("insert into `rustdesk_token` (`username`, `uid`,`id`,`client_id`, `uuid`, `access_token`, `create_time`) VALUES " +
                    "('{0}', '{1}','{2}','{3}', '{4}', '{5}','{6}') ON DUPLICATE KEY UPDATE `access_token` = values(`access_token`)", username, ru.id, id, id, uuid, Token, dTime.ToString("yyyy-MM-dd HH:mm:ss"));
                MySQLHelper.Execute(sql);
                return JsonConvert.SerializeObject(result);
            }
            return "";
        }

        [HttpGet]
        public string users(string username, string password, string id, string uuid)
        {
            return "";
        }

        [HttpPost]
        public string currentUser(string id, string uuid)
        {
            var Token = Request.Headers["authorization"].Replace("Bearer ", "").Trim();
            string sql = string.Format("SELECT * FROM `rustdesk_token` where client_id = '{0}' and uuid='{1}' and access_token='{2}';", id, uuid, Token);
            DataTable dt = MySQLHelper.QueryDataTable(sql);
            List<rustdesk_token> tklst = JsonConvert.DeserializeObject<List<rustdesk_token>>(JsonConvert.SerializeObject(dt));
            if (tklst.Count > 0)
            {
                var tk = tklst.First();
                return JsonConvert.SerializeObject(tk);
            }
            return "{\"code\":100, \"data\": \"退出成功\" }";
        }

        [HttpPost]
        public string ab(string value, string data)
        {
            var Token = Request.Headers["authorization"].Replace("Bearer ", "").Trim();
            string sql = "";
            if (string.IsNullOrEmpty(value))
            {
                abRequest abr = JsonConvert.DeserializeObject<abRequest>(data);
                if (abr == null)
                {
                    return "{\"code\":99,\"data\":\"数据错误！\"}";
                }
                sql = string.Format("SELECT username,uid FROM `rustdesk_token` where access_token='{0}'", Token);
                DataTable dt = MySQLHelper.QueryDataTable(sql);
                List<rustdesk_token> tklst = JsonConvert.DeserializeObject<List<rustdesk_token>>(JsonConvert.SerializeObject(dt));
                if (tklst.Count == 0)
                {
                    return "{\"error\":\"Wrong credentials\",\"msg\":\"提供的登录信息错误\"}";
                }
                var uid = tklst.First().uid;

                // 循环添加tags
                sql = string.Format("DELETE FROM `rustdesk_tags` WHERE `uid` = '{0}';", uid);
                MySQLHelper.Execute(sql);
                foreach (var tag in abr.tags)
                {
                    sql = string.Format("INSERT INTO `rustdesk_tags` (`uid`, `tag`) VALUES ('{0}', '{1}')", uid, tag);
                    MySQLHelper.Execute(sql);
                }

                // 循环添加tags
                sql = string.Format("DELETE FROM `rustdesk_peers` WHERE `uid` = '{0}';", uid);
                MySQLHelper.Execute(sql);
                foreach (var peer in abr.peers)
                {
                    //rustdesk_peers peer = JsonConvert.DeserializeObject<rustdesk_peers>(v);
                    sql = string.Format("INSERT INTO `rustdesk_v2`.`rustdesk_peers` (`uid`, `client_id`, `username`, `hostname`, `alias`, `platform`, `tags`,`forceAlwaysRelay`,`rdpPort`,`rdpUsername`) "
                        + "VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", uid, peer.id, peer.username, peer.hostname, peer.alias, peer.platform,
                        JsonConvert.SerializeObject(peer.tags), peer.forceAlwaysRelay, peer.rdpPort, peer.rdpUsername);
                    MySQLHelper.Execute(sql);
                }
                return "{\"code\":100,\"data\": \"成功\"}";
            }
            else if (value == "get")
            {
                sql = string.Format("SELECT username,uid FROM `rustdesk_token` where access_token='{0}'", Token);
                DataTable dt = MySQLHelper.QueryDataTable(sql);
                List<rustdesk_token> tklst = JsonConvert.DeserializeObject<List<rustdesk_token>>(JsonConvert.SerializeObject(dt));
                if (tklst.Count == 0)
                {
                    return "{\"error\":\"Wrong credentials\",\"msg\":\"提供的登录信息错误\"}";
                }
                var tk = tklst.First();
                sql = string.Format("SELECT tag FROM `rustdesk_tags` where uid = '{0}'", tk.uid);
                dt = MySQLHelper.QueryDataTable(sql);
                var tags = dt.AsEnumerable().Select(o => o.Field<string>("tag")).ToList();

                sql = string.Format("SELECT * FROM `rustdesk_peers` where uid='{0}'", tk.uid);
                dt = MySQLHelper.QueryDataTable(sql);
                var peers = dt.AsEnumerable().Select(o => new
                {
                    id = o.Field<int>("id"),
                    uid = o.Field<int>("uid"),
                    client_id = o.Field<string>("client_id"),
                    username = o.Field<string>("username"),
                    hostname = o.Field<string>("hostname"),
                    alias = o.Field<string>("alias"),
                    platform = o.Field<string>("platform"),
                    tags = JsonConvert.DeserializeObject<List<string>>(o.Field<string>("tags")),
                    forceAlwaysRelay = o.Field<string>("forceAlwaysRelay"),
                    rdpPort = o.Field<string>("rdpPort"),
                    rdpUsername = o.Field<string>("rdpUsername"),
                }).ToList();
                var result = new
                {
                    data = new
                    {
                        tags,
                        peers
                    }
                };
            }
            return "";
        }

        [HttpGet]
        public string ab()
        {
            var Token = Request.Headers["authorization"].Replace("Bearer ", "").Trim();
            string sql = string.Format("SELECT username,uid FROM `rustdesk_token` where access_token='{0}'", Token);
            DataTable dt = MySQLHelper.QueryDataTable(sql);
            List<rustdesk_token> tklst = JsonConvert.DeserializeObject<List<rustdesk_token>>(JsonConvert.SerializeObject(dt));
            if (tklst.Count == 0)
            {
                return "{\"error\":\"Wrong credentials\",\"msg\":\"提供的登录信息错误\"}";
            }
            var tk = tklst.First();
            sql = string.Format("SELECT tag FROM `rustdesk_tags` where uid = '{0}'", tk.uid);
            dt = MySQLHelper.QueryDataTable(sql);
            var tags = dt.AsEnumerable().Select(o => o.Field<string>("tag")).ToList();

            sql = string.Format("SELECT * FROM `rustdesk_peers` where uid='{0}'", tk.uid);
            dt = MySQLHelper.QueryDataTable(sql);
            var peers = dt.AsEnumerable().Select(o => new
            {
                id = o.Field<int>("id"),
                uid = o.Field<int>("uid"),
                client_id = o.Field<string>("client_id"),
                username = o.Field<string>("username"),
                hostname = o.Field<string>("hostname"),
                alias = o.Field<string>("alias"),
                platform = o.Field<string>("platform"),
                tags = JsonConvert.DeserializeObject<List<string>>(o.Field<string>("tags")),
                forceAlwaysRelay = o.Field<string>("forceAlwaysRelay"),
                rdpPort = o.Field<string>("rdpPort"),
                rdpUsername = o.Field<string>("rdpUsername"),
            }).ToList();
            var result = new
            {
                data = new
                {
                    tags,
                    peers
                }
            };
            return "";
        }

        public string audit(string username, string password, string id, string uuid)
        {
            return "";
        }

        [HttpPost]
        public string logout(string username, string password, string id, string uuid)
        {
            return "";
        }

        [HttpGet]
        public string peers(string username, string password, string id, string uuid)
        {
            return "";
        }

        public string record(string username, string password, string id, string uuid)
        {
            return "";
        }

        public string heartbeat(string username, string password, string id, string uuid)
        {
            return "";
        }


    }
}