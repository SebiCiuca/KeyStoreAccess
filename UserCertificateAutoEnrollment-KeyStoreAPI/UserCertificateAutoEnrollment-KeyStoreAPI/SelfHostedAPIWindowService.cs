using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace UserCertificateAutoEnrollment_KeyStoreAPI
{
    partial class SelfHostedAPIWindowService : ServiceBase
    {
        public SelfHostedAPIWindowService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            config.Routes.MapHttpRoute(
                name: "KeyStoreAPI",
                routeTemplate: "{controller/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });

            HttpSelfHostServer server = new HttpSelfHostServer(config);

            server.OpenAsync().Wait();
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
