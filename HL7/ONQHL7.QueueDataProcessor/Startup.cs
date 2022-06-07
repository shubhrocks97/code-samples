using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ONQHL7.GlobalDb;
using ONQHL7.GlobalDb.Classes;
using ONQHL7.GlobalDb.Interfaces;
using ONQHL7.QueueDataProcessor.Models;
using ONQHL7.QueueDataProcessor.ScheduledTask;
using ONQHL7.QueueDataProcessor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONQHL7.QueueDataProcessor
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
			services.AddScoped<IProcessQueueData, ProcessQueueData>();
            services.AddScoped<IProcessMessageQueueData, ProcessMessageQueueData>();
            services.AddSingleton<IHostedService, ScheduledProcessQueueData>();
			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
			services.AddSingleton<DapperContext>();
			services.AddScoped<IOHDataRepository, OHDataRepository>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
			});
		}
	}
}
