﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Fiver.Mvc.Testing.OtherLayers;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fiver.Mvc.Testing
{
    public class Startup
    {
        public void ConfigureServices(
            IServiceCollection services)
        {
            // for integration testing use Try...
            services.TryAddSingleton<IMovieService, MovieService>(); 
            
            services.AddMvc();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvcWithDefaultRoute();
        }
    }
}
