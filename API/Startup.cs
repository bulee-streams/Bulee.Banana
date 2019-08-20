using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Context;
using AutoMapper;
using API.Repositories;
using API.Repositories.Interfaces;
using API.Email.Interfaces;

namespace API
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
            var conn = Configuration["ConnectionStrings:BananaConnectionMssql"];

            services.AddDbContext<UserContext>(options =>
                            options.UseSqlServer(conn));

            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerDocument(swagCon => 
            {
                swagCon.PostProcess = document =>
                {
                    document.Info.Title = "Bulee.Banana API";
                    document.Info.Description = "User management API";
                };
            });

            services.AddScoped<PasswordEncryption>();
            services.AddScoped<IEmail, Email.Email>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddApplicationInsightsTelemetry();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpContext();
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
