using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using WixManager.Extensions;

namespace WixManager;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("*** Listening on http://0.0.0.0:5000 ***");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
             .UseSerilog(((context, services) =>
             {
                 StartupExtension.InternalizeSerilog(context, services, "WixManager.log");
             }))    
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("http://0.0.0.0:5000");
                webBuilder.ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5000, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1;
                    });
                });

                var builder = WebApplication.CreateBuilder(args);

            });

}