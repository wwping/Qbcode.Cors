using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Bumblebee.Events;
using Bumblebee.Plugins;
using System.Linq;
using BeetleX;
using BeetleX.FastHttpApi;
using Bumblebee;

namespace Qbcode.Cors
{
    [RouteBinder(ApiLoader = false)]
    public class Plugin : IRequestingHandler, IPlugin, IPluginStatus, IPluginInfo
    {
        public string Name
        {
            get
            {
                return "qbcode.cors";
            }
        }

        public string Description
        {
            get
            {
                return "路由聚合跨域";
            }
        }

        public PluginLevel Level
        {
            get
            {
                return PluginLevel.High9;
            }
        }

        public bool Enabled { get; set; } = true;

        public string IconUrl
        {
            get
            {
                return "";
            }
        }

        public string EditorUrl
        {
            get
            {
                return "";
            }
        }

        public string InfoUrl
        {
            get
            {
                return "";
            }
        }

        public void Execute(EventRequestingArgs e)
        {
            e.Response.Header["qbcode-cors"] = "qbcode.cn";
            if (setting.Enabled == false)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(e.Request.Header["Origin"]))
            {
                if (!setting.Origin.Any() || setting.Origin.IndexOf(e.Request.Header["Origin"]) >= 0)
                {
                    e.Response.Header["Access-Control-Allow-Origin"] = e.Request.Header["Origin"];
                }
                if (!setting.Methods.Any())
                {
                    e.Response.Header["Access-Control-Allow-Methods"] = e.Request.Method;
                }
                else
                {
                    e.Response.Header["Access-Control-Allow-Methods"] = string.Join(",", e.Request.Method);
                }
                if (!setting.Headers.Any())
                {
                    e.Response.Header["Access-Control-Allow-Headers"] = e.Request.Header["Access-Control-Request-Headers"];
                }
                else
                {
                    e.Response.Header["Access-Control-Allow-Headers"] = string.Join(",", e.Request.Header["Access-Control-Request-Headers"]);
                }
                e.Response.Header["Access-Control-Allow-Credentials"] = setting.Credentials;
            }

            if (e.Request.Method.Equals("OPTIONS"))
            {
                e.Cancel = true;
                e.ResultType = ResultType.Completed;
                e.Gateway.Response(e.Response, "ok");
                this.mGateway.RequestIncrementCompleted(e.Request, 200, (long)(TimeWatch.GetTotalMilliseconds() - e.Request.RequestTime), null);
            }

        }

        public void Init(Gateway gateway, Assembly assembly)
        {
            this.mGateway = gateway;
            gateway.HttpServer.ResourceCenter.LoadManifestResource(assembly);

        }

        public void LoadSetting(JToken setting)
        {
            if (setting != null)
            {
                this.setting = setting.ToObject<SettingInfo>();
            }
        }
        public object SaveSetting()
        {
            return this.setting;
        }


        private Gateway mGateway;

        private SettingInfo setting = new SettingInfo();
    }

    public class SettingInfo
    {
        public bool Enabled { get; set; } = true;

        public List<string> Origin { get; set; } = new List<string>();
        public string Credentials { get; set; } = "true";
        public string MaxAge { get; set; } = "1800";
        public List<string> Methods { get; set; } = new List<string>();
        public List<string> Headers { get; set; } = new List<string>();

        public string Message { get; } = "当为空时，则允许全部";
    }

}
