using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ABB.Extensions;
using ABB.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ABB.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAdBlockBypass(opt =>
            {
                opt.CacheExpireTimeSec = 10;
                opt.DefaultFilePath = Directory.GetCurrentDirectory() + "/wwwroot/";
                opt.AddCSSFiles(
                    new ABBFile()
                    {
                        FileName = "adblockme.css",
                        KeysToReplace = new List<string>() { "adBlockTest", "adBlockTest2" }
                    },
                    new ABBFile()
                    {
                        FileName = "myadd.css",
                        FilePath = Directory.GetCurrentDirectory() + "/wwwroot/myfolder",//this is where adblockbypass looks for myadd.css(it will not use DefaultFilePath)
                        KeysToReplace = new List<string>() { "adBlockTest" }
                    }
                );
                opt.AddJSFiles(
                    new ABBFile()
                    {
                        FileName = "adblockme.js",
                        KeysToReplace = new List<string>() { "adBlockTest" }
                    }
                );
                // define what you want. which requests will you bypass
                opt.AddPages(
                    new ABBPage("/myadd").AddJsFiles("adblockme.js").AddCssFiles("adblockme.css", "myadd.css")
                        .AddAnotherKeysToBypass("myTestClass", "myTestId", "myTestClass2"),

                    new ABBPage("/").AddJsFiles("adblockme.js").AddCssFiles("adblockme.css", "myadd.css")
                        .AddAnotherKeysToBypass("myAddArea", "myTestClass", "myTestId", "myTestClass2"),

                    new ABBPage("/Home/Index").AddJsFiles("adblockme.js").AddCssFiles("adblockme.css", "myadd.css")
                        .AddAnotherKeysToBypass("myAddArea", "myTestClass", "myTestId", "myTestClass2")
                );
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAdBlockBypass();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
