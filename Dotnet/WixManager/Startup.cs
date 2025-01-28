using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using System;
using WixManager.Service;

namespace WixManager
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddMvc()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

          
           
            services.AddHostedService<MainService>();
            services.AddGrpc();
            services.AddHttpClient();
            services.AddSwaggerGen();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        void OnShutdown()
        {
            
        }
    }
}
