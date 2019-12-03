using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetLive.House.Search.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetLive.House.Search
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

            //连接 mysql 数据库，添加数据库上下文
            services.AddDbContext<BuildShopDbContext>(options => options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            //添加cache支持
            services.AddDistributedMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// http的请求管道处理，也就是http处理过程
        /// 
        /// 
        /// 
        /// /// </summary>
        /// <param name="app">IApplicationBuilder： </param>
        /// <param name="env"></param>
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
                app.UseHsts();
            }


            ///中间管到案例》》 应用场景： 记录日志>反爬虫>性能监控>缓存>黑白名单 ....
            ///Netcore最极简的设计
            //app.Use(y =>
            //{
            //    return async c =>
            //    {
            //        await c.Response.WriteAsync("步骤一开始");
            //        await y.Invoke(c);
            //        await c.Response.WriteAsync("步骤一结束");
            //    };
            //});
            //app.Use(y =>
            //{
            //    return async c =>
            //    {
            //        await c.Response.WriteAsync("步骤二开始");
            //        await y.Invoke(c);
            //        await c.Response.WriteAsync("步骤二结束");
            //    };
            //});
            //app.Use(y =>
            //{
            //    return async c =>
            //    {
            //        await c.Response.WriteAsync("步骤三开始");              
            //        await c.Response.WriteAsync("步骤三结束");
            //    };
            //});

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=BuildShopView}/{id?}");
            });
        }
    }
}
