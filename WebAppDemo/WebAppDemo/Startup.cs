using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WebAppDemo.Context;
using WebAppDemo.DTOs.Mappings;
using WebAppDemo.Extensions;
using WebAppDemo.Logging;

namespace WebAppDemo
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
            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    });

            services.AddDbContext<AppDbContext>(
                options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

            services.AddSwaggerGen(
                c => { c.SwaggerDoc("v1", new OpenApiInfo {Title = "Curso Web Api Essential", Version = "v1"}); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Curso Web Api Essential"); });

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            //adiciona o middleware de tratamento de erro
            app.ConfigureExceptionHandler();

            //adiciona o middleware de roteamento
            app.UseRouting();

            //adiciona middleware que habilita a autoriza??o
            app.UseAuthorization();

            //adciona os endpoints para as Actions dos controladores sem especificar rotas
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}