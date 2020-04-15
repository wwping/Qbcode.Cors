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
            if (setting.Enabled == false)
            {
                return;
            }
            e.Response.Header["qbcode-cors"] = "qbcode.cn";
            e.Response.Header["Gateway"] = "qbcode.cn";
            e.Response.Header["Server"] = "qbcode.cn";
            if (!string.IsNullOrWhiteSpace(e.Request.Header["Origin"]))
            {
                if (setting.Origin.Length > 0)
                {
                    if (setting.Origin != "*")
                    {
                        e.Response.Header["Access-Control-Allow-Origin"] = setting.Origin;
                        e.Response.Header["Vary"] = setting.Vary;
                    }
                    else
                    {
                        e.Response.Header["Access-Control-Allow-Origin"] = e.Request.Header["Origin"];
                    }
                }
                if (setting.Methods.Length > 0)
                {
                    e.Response.Header["Access-Control-Allow-Methods"] = setting.Methods;
                }
                mGateway.HttpServer.Log(BeetleX.EventArgs.LogType.Error, "Headers:" + setting.Headers);
                if (setting.Headers.Length > 0)
                {
                    e.Response.Header["Access-Control-Allow-Headers"] = setting.Headers;
                }

                if (setting.Credentials)
                {
                    e.Response.Header["Access-Control-Allow-Credentials"] = "true";
                }
                else
                {
                    e.Response.Header["Access-Control-Allow-Credentials"] = "false";
                }

                if (setting.MaxAge > 0)
                {
                    e.Response.Header["Access-Control-Max-Age"] = setting.MaxAge.ToString();
                }

            }

            if (string.Compare(e.Request.Method, "OPTIONS", true) == 0)
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
                var _setting = setting.ToObject<SettingInfo>();

                if (_setting.Origin.Length == 0)
                {
                    _setting.Origin = new string[] { "*" };
                }
                if (_setting.Headers.Length == 0)
                {
                    _setting.Headers = new string[] { "*" };
                }

                if (_setting.Methods.Length == 0)
                {
                    _setting.Methods = new string[] { "*" };
                }

                this.setting = new SettingInfo2
                {
                    Credentials = _setting.Credentials,
                    Enabled = _setting.Enabled,
                    Headers = string.Join(",", _setting.Headers),
                    MaxAge = _setting.MaxAge,
                    Origin = string.Join(",", _setting.Origin),
                    Methods = string.Join(",", _setting.Methods),
                    Vary = _setting.Vary
                };
            }
        }
        public object SaveSetting()
        {
            return new SettingInfo
            {
                Credentials = this.setting.Credentials,
                Enabled = this.setting.Enabled,
                Headers = this.setting.Headers.Split(','),
                MaxAge = this.setting.MaxAge,
                Methods = this.setting.Methods.Split(','),
                Origin = this.setting.Origin.Split(','),
                Vary = this.setting.Vary,
            };
        }

        private Gateway mGateway;

        private SettingInfo2 setting = new SettingInfo2();
    }

    public class SettingInfo
    {
        public bool Enabled { get; set; } = true;

        public string[] Origin { get; set; } = new string[] { "*" };

        public bool Credentials { get; set; } = true;

        public int MaxAge { get; set; } = 1800;

        public string[] Methods { get; set; } = new string[] { "get", "post" };

        public string[] Headers { get; set; } = new string[] { };

        public string Vary { get; set; } = "Origin";


        public string Message { get; } = "当为空时，则允许全部";
    }

    public class SettingInfo2
    {
        public bool Enabled { get; set; } = true;

        public string Origin { get; set; } = string.Empty;

        public bool Credentials { get; set; } = true;

        public int MaxAge { get; set; } = 1800;

        public string Methods { get; set; } = string.Empty;

        public string Headers { get; set; } = string.Empty;

        public string Vary { get; set; } = "Origin";
    }

}
