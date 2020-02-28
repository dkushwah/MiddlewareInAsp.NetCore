using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MiddlewareDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger logger)
        {

            
            app.UseMiddleware(typeof(RequestExecutionTime),logger);
            
            app.UseWhen(
                ctx =>
            ctx.Request.Path.Value.Contains("/ip", StringComparison.OrdinalIgnoreCase), app =>
             {
                 app.Use(async (context, next) =>
                 {
                     var watch = new Stopwatch();
                     watch.Start();
                     await next();
                     watch.Stop();
                     Console.WriteLine("Total Request Time in Milliseconds:" + watch.Elapsed.TotalMilliseconds);

                 });
             });

            app.Use(async (context, next) =>
            {
                Console.WriteLine("Executing the dummy pipeline");
                await next();
            });
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.Map(new PathString("/ip"), a => a.Run(async context =>
            {
                await context.Response.WriteAsync("My IP  Is :" + context.Request.HttpContext.Connection.RemoteIpAddress);
            }));
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }

    

    }
}
