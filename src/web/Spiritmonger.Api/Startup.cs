using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Spiritmonger.Api.Config;
using Spiritmonger.Mapping.Modules;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace Spiritmonger.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IHostingEnvironment Env { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services = CoreConfiguration.Load(services, Configuration); 
            services = CoreModule.Load(services, Configuration);
            services.AddTransient<ControllerContext>();

            AutoMapperConfig.Register();

            services.AddMemoryCache();

            if (!Env.IsDevelopment())
            {
                services.Configure<MvcOptions>(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                });
            }
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost4200",
                builder => builder.WithOrigins("http://localhost:4200", "https://spiritmonger.azurewebsites.net")
                .AllowAnyHeader()
                .AllowAnyMethod());
            });

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddMvc(options =>
            {
                options.RespectBrowserAcceptHeader = true;
                options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddSwaggerGen(swagger =>
            {     
                swagger.DescribeAllEnumsAsStrings();
                swagger.DescribeAllParametersInCamelCase();
                swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Spiritmonger.Api.xml"));
                swagger.SwaggerDoc("v1", new Info
                {
                    Title = "Spiritmonger",
                    Description = "The NicFit library",
                    Version = "V1",
                    Contact = new Contact() { Email = "bmeijwaard@gmail.com", Name = "Spiritmonger", Url = "" },
                    TermsOfService = $"Terms of Service - {Guid.NewGuid()}",
                    License = new License() { Name = "License", Url = "" }
                });
                // swagger.AddSecurityDefinition("JWT", new ApiKeyScheme() { In = "header", Description = "Spiritmonger", Name = "Authorization", Type = "apiKey" });
            });

            
            //services.ConfigureSwaggerGen(swagger =>
            //{
            //    swagger.OperationFilter<FormFileOperationFilter>(); //Register File Upload Operation Filter
            //});
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("AllowLocalhost4200");

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Spiritmonger");
            });

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
