using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticseatchApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElasticseatchApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string MyAllowSpecificOrigins = "CorsPolicy";
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            //    options.CheckConsentNeeded = context => true;
            //    options.MinimumSameSitePolicy = SameSiteMode.None;
            //});

            //////配置跨域访问           
            //services.AddCors(options =>
            //{
            //    options.AddPolicy(MyAllowSpecificOrigins,
            //        builder => builder.AllowAnyOrigin()
            //          .AllowAnyMethod()
            //          .AllowAnyHeader()
            //          .AllowCredentials()
            //    .Build());
            //});
            //连接 mysql 数据库，添加数据库上下文
            services.AddDbContext<BuildShopDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            //添加cache支持
            services.AddDistributedMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseHsts();
            }

            app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

            //// 跨域
            //app.UseHttpsRedirection().UseCors(builder =>
            //builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // 返回错误码
            app.UseStatusCodePages();//把错误码返回前台，比如是404
            // 使用静态文件
            app.UseStaticFiles();
            // 使用cookie
            app.UseCookiePolicy();
            // 跳转使用https
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
