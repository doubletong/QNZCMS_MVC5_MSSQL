﻿using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TZGCMS.SiteWeb.Startup))]
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace TZGCMS.SiteWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
