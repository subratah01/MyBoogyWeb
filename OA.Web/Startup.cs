using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OA.Data;
using OA.Repo.Contracts;
using OA.Repo.Contracts.Common;
using OA.Repo.Repositories;
using OA.Repo.Repositories.Common;
using OA.Service;
using OA.Service.Contracts;
using OA.Web.Helper;
using OA.Web.Hubs;
using System.Text;

namespace OA.Web
{
    public class Startup
    {
        public string db_con { get; set; }
        public string jwt_key { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            db_con = configuration["AppSettings:ConnectionStrings:IlsheguriDb"];
            jwt_key = Configuration["AppSettings:Jwt:Key"];
            issuer = Configuration["AppSettings:Jwt:Issuer"];
            audience = Configuration["AppSettings:Jwt:Audiance"];
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddDbContext<MyboogydbContext>(options => options.UseLazyLoadingProxies().UseMySql(Configuration.GetConnectionString("MySql"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql")));

            
            //services.AddDbContext<IlsheguriDbContext>(option => option.UseLazyLoadingProxies().UseSqlServer(@"Data Source=WIN-LIVFRVQFMKO;Initial Catalog=Ilsheguri;user id=sa;password=parrot@1234;Integrated Security=false;"));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<IExtendedHubHelper, ExtendedHubHelper>();

            #region Services
            services.AddTransient<IUserAuthenticationService, UserAuthenticationService>();
            services.AddTransient<IUtilityServiceProvider, UtilityServiceProvider>();
            #endregion

            #region Repositories
            services.AddTransient<IAuthenticationRepository, AuthenticationRepository>();
            //services.AddTransient<ICartRepository, CartRepository>();
            //services.AddTransient<IOrderRepository, OrderRepository>();
            #endregion

            var key = Encoding.UTF8.GetBytes(jwt_key);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            #region Swagger

            services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            #endregion

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                });

            #region Add CORS
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
            }));
            #endregion

            services.AddSignalR();
            //services.AddControllers();
            services.AddControllersWithViews();

            
                //options.Events = new JwtBearerEvents
                //{
                //    OnMessageReceived = context =>
                //     {
                //         var accessToken = context.Request.Query["access_token"];

                //         //If the request is for our hub...
                //         var path = context.HttpContext.Request.Path;
                //         if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/notificationsHub")))
                //         {
                //             //Read the token out of the query string
                //             context.Token = accessToken;
                //         }
                //         return Task.CompletedTask;
                //     },
                //    OnTokenValidated = context =>
                //     {
                //         var path = context.HttpContext.Request.Path;
                //         if (!path.StartsWithSegments("/notificationsHub"))
                //         {
                //             Uri referer = new Uri(context.Request.Headers["referer"]);
                //             if (context.Principal.Claims.FirstOrDefault(c => c.Type == referer.Authority) == null)
                //             {
                //                 context.Fail("Unauthorized - User is authenticated, but does not have access to this portal" + referer.ToString());
                //                 context.Response.Headers.Add("X-challenge", new[] { "403" });
                //             }
                //         }
                //         return Task.FromResult<object>(null);
                //     }
                //};
            //});



            

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
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            //Enable Cors
            app.UseCors("CorsPolicy");


            #region Swagger

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            #endregion

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<NotificationsHub>("/notificationsHub");

                //endpoints.MapControllers();
            });
        }
    }
}
