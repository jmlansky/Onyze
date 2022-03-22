using Framework.Helpers;
using Framework.Repositories;
using Framework.Repositories.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totem.Sync.Repositories;
using Totem.Sync.Repositories.Contracts;
using Totem.Sync.Services;
using Totem.Sync.Services.Contracts;
using Totems.Sync.Repositories;
using Totems.Sync.Repositories.Contracts;
using Touch.Core.Totems;
using Touch.Core.Totems.Contracts;

namespace Totem.Sync.Api
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
            ConfigureDependenciesToServices(services);
            ConfigureDependenciesToRepositories(services);

            services.AddControllers();
            services.AddMvc();

            // JWT Token Generation from Server Side.  
            AddSwagger(services);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])), //Configuration["JwtToken:SecretKey"] ,

                    // Allow to use seconds for expiration of token
                    // Required only when token lifetime less than 5 minutes
                    // THIS ONE
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ActionFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "SimpliSale API V1"); });
            app.UseCors(x => x.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials().WithExposedHeaders("Token", "Rol"));

            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void AddSwagger(IServiceCollection services)
        {
            // Enable Swagger   
            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation  
                var groupName = "v1";
                swagger.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"SimpliSales {groupName}",
                    Version = groupName,
                    Description = "SimpliSales API",
                    Contact = new OpenApiContact
                    {
                        Name = "SimpliSales",
                        Email = string.Empty,
                        Url = new Uri("https://foo.com/"),
                    }
                });
                swagger.OperationFilter<HeaderFilterForRole>();


                // To Enable authorization using Swagger (JWT)  
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer."
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },

                        new string[] {}
                    }
                });
            });
        }

        private void ConfigureDependenciesToServices(IServiceCollection services)
        {
            services.AddScoped<ITotemsService, TotemsService>();
            services.AddScoped<IProgramacionItemService<IProgramacionItem>, ProgramacionItemPlaylist>();
        }

        private void ConfigureDependenciesToRepositories(IServiceCollection services)
        {
            services.AddScoped<ITotemsRepository, TotemsRepository>();
            services.AddScoped<ISectoresDelTotemRepository, SectoresDelTotemRepository>();
            services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();
            services.AddScoped<IMultimediaRepository, MultimediaRepository>();
            services.AddScoped<ITipoMultimediaRepository, TipoMultimediaRepository>();
            services.AddScoped<IProgramacionesRepository, ProgramacionesRepository>();
            services.AddScoped<IItemsProgramacionRepository, ItemsProgramacionRepository>();
            services.AddScoped<IPeriodosRepository, PeriodosRepository>();
            services.AddScoped<IFranjasHorariasRepository, FranjasHorariasRepository>();
            services.AddScoped(typeof(ISingleEntityComunRepository<>), typeof(SingleEntityComunRepository<>));


            services.AddScoped(typeof(IQueryStatement<>), typeof(QuerySelect<>));
            services.AddScoped(typeof(IQueryStatement<>), typeof(QueryUpdate<>));
            services.AddScoped(typeof(IQueryStatement<>), typeof(QueryInsert<>));
            services.AddScoped(typeof(IQueryStatement<>), typeof(QueryDelete<>));
        }
    }
}
