using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System.IO;

namespace WixManager.Extensions
{
    public static class StartupExtension
    {

        public static void InternalizeSerilog(HostBuilderContext context, LoggerConfiguration services, string fileName)
        {
            bool DebugMode = true;
            var template = "{Timestamp:yyyy-MM-dd HH:mm:ss}  {Level:u4}  {Message:lj}{NewLine}{Exception}";

            var logFolder =  "Logs";
          

            services
                .Enrich.FromLogContext()
               
                .WriteTo.Console(outputTemplate: template)
                .WriteTo.Debug(outputTemplate: template)              
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Grpc.Net.Client", LogEventLevel.Fatal)
                .ReadFrom.Configuration(context.Configuration)
                .WriteTo.File(Path.Combine(logFolder, fileName), outputTemplate: template, rollingInterval: RollingInterval.Day);

            if (DebugMode)
            {
                services.MinimumLevel.Verbose();
            }
        }

    }
}
