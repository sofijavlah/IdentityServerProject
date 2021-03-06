﻿using System;
using System.Collections.Generic;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.IdentityServer4;
using Castle.Facilities.Logging;
using Castle.Windsor.Installer;
using IdentityServer4.Models;
using IdentityServerForAbp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestProject.Authentication.JwtBearer;
using TestProject.Authorization.Users;
using TestProject.Configuration;
using TestProject.EntityFrameworkCore;
using TestProject.Identity;
using Resources = IdentityServerForAbp.Models.Resources;

namespace IdentityServerForAbp
{
    public class Startup
    {
        //for making clients via config file
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            _configuration = env.GetAppConfiguration();
            
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            IdentityRegistrar.Register(services);

            services.AddIdentityServer()
                .AddInMemoryClients(_configuration.GetSection("IdentityServerProject:Clients"))
                //.AddInMemoryClients(Clients.Get())
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                //.AddTestUsers(Users.Get())
                .AddDeveloperSigningCredential()
                .AddAbpPersistedGrants<TestProjectDbContext>()
                .AddAbpIdentityServer<User>();

            return services.AddAbp<IdentityServerForAbpModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAbp();

            app.UseStaticFiles();

            app.UseIdentityServer();
            
            app.UseMvcWithDefaultRoute();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            app.Run(async (context) =>
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            {
                //await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
