using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using MyCourse.Models.Services.Application;
using MyCourse.Models.Services.Infrastructure;
using MyCourse.Models.Options;

namespace MyCourse
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration; 
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching(); // utilizziamo dei servizi di caching

            services.AddMvc(options => {
                var homeProfile = new CacheProfile();
                // se i nomi non corrispondono devo utilizzare le due righe qui sotto
                // homeProfile.Duration = Configuration.GetValue<int>("ResponseCache:Home:Duration");
                // homeProfile.Location = Configuration.GetValue<ResponseCacheLocation>("ResponseCache:Home:Location");
                
                // con questo indichiamo che nel servizio di caching bisogna distinguere anche le chiavi passate all'URL (altrimenti page 2 per lui sarebbe come page 1)
                //andiamo però a indicare anche questo in appsettings.json
                // homeProfile.VaryByQueryKeys = new string [] {"page"}; 

                // se i nomi corrispondono uso questa riga
                Configuration.Bind("ResponseCache:Home", homeProfile);
                
                options.CacheProfiles.Add("Home", homeProfile);
            }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
            #if DEBUG
            .AddRazorRuntimeCompilation()
            #endif
            ;

            // AddTransient crea nuove istanze del serivizio ogni volta che un componente ne ha bisogno e dopo che sono state utilizzate le rimuove.
            // AddScoped tiene viva l'istanza fino a quando rimaniamo nello stessa richiesta http, poi la distrugge
            // AddSingleton crea una sola istanza e la inietta a tutti i componenti che ne hanno bisogno, anche in richieste http diverse
            
            // services.AddTransient<ICourseService, EfCoreCourseService>();
            services.AddTransient<ICourseService, AdoNetCourseService>();
            services.AddTransient<IDatabaseAccessor, SqliteDatabaseAccessor>();
            services.AddTransient<ICachedCourseService, MemoryCacheCourseService>();

            // services.AddDbContext<MyCourseDbContext>();

            // preferibile a comando remmato sopra, in quanto può utilizzare context già pronti accorciando i tempi
            services.AddDbContextPool<MyCourseDbContext>(optionsBuilder => 
            {
                string connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");
                optionsBuilder.UseSqlite(connectionString);                
            }); 

            // options
            services.Configure<ConnectionStringsOptions>(Configuration.GetSection("ConnectionStrings"));
            services.Configure<CoursesOptions>(Configuration.GetSection("Courses"));
            services.Configure<MemoryCacheOptions>(Configuration.GetSection("MemoryCache"));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            // if (env.IsDevelopment())
            if (env.IsEnvironment("Development"))
            {
                app.UseDeveloperExceptionPage();

                //Aggiorniamo un file per notificare al BrowserSync che deve aggiornare la pagina
                lifetime.ApplicationStarted.Register(() =>
                {
                    string filePath = Path.Combine(env.ContentRootPath, "bin/reload.txt");
                    File.WriteAllText(filePath, DateTime.Now.ToString());
                });
            } else {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            // endpoint Routing Middleware
            app.UseRouting();

            app.UseResponseCaching(); // utilizza il caching di ASP.NET che permette di salvare in memoria per un tempo stabilito una view

            // endpoint Middleware
            app.UseEndpoints(routeBuilder => {
                routeBuilder.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            // app.UseMvcWithDefaultRoute();
            /* app.UseMvc(routeBuilder => {
                routeBuilder.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            }); */

            // app.Run(async (context) =>
            // {
            //     string nome = context.Request.Query["nome"];
            //     await context.Response.WriteAsync($"Hello {nome.ToUpper()}!");
            // });
        }
    }
}
