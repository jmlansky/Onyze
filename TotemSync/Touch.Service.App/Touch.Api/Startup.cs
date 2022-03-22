using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Touch.Repositories.Articulos;
using Touch.Service.Articulos;
using Microsoft.OpenApi.Models;
using Touch.Service.Comun;
using Touch.Repositories.Comun;
using Touch.Service.Publicaciones;
using Touch.Repositories.Publicaciones;
using Touch.Repositories.Gondolas;
using Touch.Service.Gondolas;
using Touch.Service.Promociones;
using Touch.Repositories.Promociones;
using Touch.Service.Programaciones;
using Touch.Repositories.Programaciones;
using Touch.Core.Programaciones.Items;
using Touch.Service.Archivos.Contracts;
using Touch.Service.Archivos.Services;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Archivos.Repositories;
using Touch.Service.Archivos.Storage;
using Touch.Service.Usuarios.Contracts;
using Touch.Service.Usuarios;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Repositories.Usuarios;
using EnviadorDeMail;
using Touch.Repositories.Articulos.Contracts;
using Framework.Auth.Services.Contracts;
using Framework.Auth.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Touch.Repositories.Gondolas.Contracts;
using Framework.Helpers;
using Touch.Service.Playlists;
using Touch.Repositories.Playlists;
using Touch.Repositories.Clientes.Contracts;
using Touch.Service.Clientes;
using Touch.Service.Totems;
using Touch.Service.Sucursales;
using Touch.Repositories.Clientes;
using Touch.Repositories.Sucursales.Contracts;
using Touch.Repositories.Totems.Contracts;
using Touch.Repositories.Totems;
using Touch.Repositories.Sucursales;

namespace Touch.Service
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
            ConfigureDependencyToRepositories(services);
            ConfigureDependencyToServices(services);
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

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

        private void ConfigureDependencyToServices(IServiceCollection services)
        {
            services.AddScoped<IArticulosService, ArticulosService>();
            services.AddScoped<IAtributosService, AtributosService>();
            services.AddScoped<IBusquedaDeArticulosService, BusquedaDeArticulosService>();
            services.AddScoped<ITiposDeAtributoService, TiposDeAtributoService>();
            services.AddScoped<ITipoDeArticuloService, TipoDeArticuloService>();
            services.AddScoped<ICategoriasDeArticulosService, CategoriasDeArticulosService>();
            services.AddScoped<IFabricantesService, FabricantesService>();
            services.AddScoped<IProvinciasService, ProvinciasService>();
            services.AddScoped<ILocalidadesService, LocalidadesService>();
            services.AddScoped<IZonasService, ZonasService>();
            services.AddScoped<IBarriosService, BarriosService>();
            services.AddScoped(typeof(ISingleEntityComunService<>), typeof(SingleEntityComunService<>));
            services.AddScoped<IArchivosService, ArchivosService>();
            services.AddScoped<ISponsoreadosService, SponsoreadosService>();
            services.AddScoped<ICodigosDeBarraService, CodigosDeBarraService>();
            services.AddScoped<IPublicacionesService, PublicacionesService>();
            services.AddScoped<IGondolasService, GondolasService>();
            services.AddScoped<IEstantesService, EstantesService>();
            services.AddScoped<IPromocionesService, PromocionesService>();
            services.AddScoped<IRegionesService, RegionesService>();
            services.AddScoped<IProgramacionesService, ProgramacionesService>();
            services.AddScoped<IUsuariosService, UsuariosService>();
            services.AddScoped<IGetServices, GetServices>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IPromociones, DescuentoPorcentual>();
            services.AddScoped<IPromociones, MontoFijo>();

            services.AddScoped<IAlmacenamientoDeArchivos, AlmacenamientoDeArchivosAzure>();
            services.AddScoped<IAlmacenamientoDeArchivos, AlmacenamientoDeArchivosLocal>();

            services.AddScoped<IMailSender, Sender>();
            services.AddScoped<IPlaylistsService, PlaylistsService>();
            services.AddScoped<IClientesService, ClientesService>();
            services.AddScoped<ISucursalesService, SucursalesService>();
            services.AddScoped<ITotemsService, TotemsService>();

        }

        private void ConfigureDependencyToRepositories(IServiceCollection services)
        {
            services.AddScoped<ICategoriasDeArticuloRepository, CategoriasDeArticuloRepository>();
            services.AddScoped<ITipoDeArticuloRepository, TipoDeArticuloRepository>();
            services.AddScoped<ITiposDeAtributoRepository, TiposDeAtributoRepository>();
            services.AddScoped<IArticulosRepository, ArticulosRepository>();
            services.AddScoped<IFabricantesRepository, FabricantesRepository>();
            services.AddScoped<IAtributosRepository, AtributosRepository>();
            services.AddScoped<IArticuloMultipleRepository, ArticuloMultipleRepository>();
            services.AddScoped<IProvinciasRepository, ProvinciasRepository>();
            services.AddScoped<ILocalidadesRepository, LocalidadesRepository>();
            services.AddScoped<IBarriosRepository, BarriosRepository>();
            services.AddScoped<IZonasRepository, ZonasRepository>();
            services.AddScoped<IArchivosRepository, ArchivosRepository>();
            services.AddScoped(typeof(ISingleEntityComunRepository<>), typeof(SingleEntityComunRepository<>));
            services.AddScoped(typeof(IDestinatarioDePromocionRepository<>), typeof(DestinatarioDePromocionRepository<>));
            services.AddScoped<ISponsoreoRepository, SponsoreoRepository>();
            services.AddScoped<ICodigosDeBarraRepository, CodigosDeBarraRepository>();
            services.AddScoped<IPublicacionesRepository, PublicacionesRepository>();
            services.AddScoped<IGondolasRepository, GondolasRepository>();
            services.AddScoped<IEstantesRepository, EstantesRepository>();
            services.AddScoped<IObjetoAPublicitarRepository, ObjetoAPublicitarRepository>();
            services.AddScoped<ITipoObjetoPublicitarRepository, TipoObjetoPublicitarRepository>();
            services.AddScoped<IPromocionesRepository, PromocionesRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();

            services.AddScoped<IProgramacionFranjaHorariaRepository, ProgramacionFranjaHorariaRepository>();

            services.AddScoped<ICategoriaAsociadaAlArticuloRepository, CategoriaAsociadaAlArticuloRepository>();
            services.AddScoped<IPromocionDeProvinciasRepository, PromocionDeProvinciasRepository>();
            services.AddScoped<IDetallesDePromocionRepository, DetallesDePromocionRepository>();
            services.AddScoped<IPromocionDeRegionesRepository, PromocionDeRegionesRepository>();
            services.AddScoped<IPromocionDeClientesRepository, PromocionDeClientesRepository>();
            services.AddScoped<IPromocionDeGruposRepository, PromocionDeGruposRepository>();
            services.AddScoped<IPromocionDeCategoriaRepository, PromocionDeCategoriaRepository>();
            services.AddScoped<IProgramacionPeriodoRepository, ProgramacionPeriodoRepository>();
            services.AddScoped<IPromocionDeFabricantesRepository, PromocionDeFabricantesRepository>();
            services.AddScoped<IProgramacionItemRepository, ProgramacionItemRepository>();
            services.AddScoped<IRegionesRepository, RegionesRepository>();
            services.AddScoped<IProgramacionPeriodoRepository, ProgramacionPeriodoRepository>();
            services.AddScoped<IProgramacionesRepository, ProgramacionesRepository>();
            services.AddScoped<IArticulosPorEstanteRepository, ArticulosPorEstanteRepository>();
            services.AddScoped<IUsuariosRepository, UsuariosRepository>();
            services.AddScoped<IGetArticulosRepository, GetArticulosRepository>();
            services.AddScoped<IGrillasRepository, GrillasRepository>();
            services.AddScoped<IEstantesDecoracionesRepository, EstantesDecoracionesRepository>();
            services.AddScoped<IArticulosDestacadosRepository, ArticulosDestacadosRepository>();
            services.AddScoped<IArticulosDecoracionesRepository, ArticulosDecoracionesRepository>();

            services.AddScoped<IMultimediaRepository, MultimediaRepository>();
            services.AddScoped<ITipoMultimediaRepository, TipoMultimediaRepository>();
            services.AddScoped<IPlaylistDeSectorRepository, PlaylistDeSectorRepository>();
            services.AddScoped<IPlaylistsRepository, PlaylistsRepository>();

            services.AddScoped<IClientesRepository, ClientesRepository>();
            services.AddScoped<ITotemsRepository, TotemsRepository>();
            services.AddScoped<ISucursalesRepository, SucursalesRepository>();

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

    }
}
