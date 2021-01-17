using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TvMaze.Scraper.Abstractions.Repositories;
using TvMaze.Scraper.Abstractions.Services;
using TvMaze.Scraper.API.HostedService;
using TvMaze.Scraper.API.Mappers;
using TvMaze.Scraper.Implementations;
using TvMaze.Scraper.Implementations.Clients.TvMazeApi;
using TvMaze.Scraper.Implementations.Repositories;
using TvMaze.Scraper.Implementations.Services;

namespace TvMaze.Scraper.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAutoMapper(conf => conf.AddProfile<ShowMapperProfile>());

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name)));

            services.AddScoped<IShowRepository, ShowRepository>();

            services.AddHttpClient<ITvMazeApiClient, TvMazeApiClient>(c => c.BaseAddress = new Uri(Configuration.GetSection("TvMazeApiHostAddress").Value));

            services.AddScoped<ITvMazeSyncService, TvMazeSyncService>();

            services.AddHostedService<TvMazeSyncHostedService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
