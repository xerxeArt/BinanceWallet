using Data.BinanceApi;
using Data.Business;
using Data.Configuration;
using Data.Crypto;
using Data.EF.Contexts;
using Data.EF.Repositories;
using Data.HttpTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace BinanceWallet
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
            var binanceConfig = Configuration.GetSection("BinanceConfig").Get<BinanceConfig>();
            var dbConfig = Configuration.GetSection("DbConfig").Get<DbConfig>();

            services.AddControllersWithViews();
            services.AddSingleton(binanceConfig);
            //services.AddSingleton(dbConfig);
            services.AddScoped<IHttpUtilities, HttpUtilities>();
            services.AddScoped<IBusinessFlow, BusinessFlow>();
            services.AddScoped<IApiRequest, ApiRequest>();
            services.AddScoped<IDataGathering, DataGathering>();
            services.AddScoped<IDataTransformation, DataTransformation>();
            services.AddScoped<ISignatureGenerator, SignatureGenerator>();
            services.AddScoped<IBinanceRepository, BinanceRepository>();

            services.AddHttpClient(Constants.BINANCE_HTTP_CLIENT, client =>
            {
                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = binanceConfig.Scheme ?? "https";
                uriBuilder.Host = binanceConfig.BaseUrl;
                uriBuilder.Port = binanceConfig.Port == 0 ? 80 : binanceConfig.Port;
                Uri uri = uriBuilder.Uri;
                client.BaseAddress = uri;
                client.Timeout = TimeSpan.FromMilliseconds(binanceConfig.Timeout == 0 ? 10000 : binanceConfig.Timeout);
            })
            .ConfigurePrimaryHttpMessageHandler(handler =>
                new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip
                });

            services.AddDbContext<BinanceContext>(options => options.UseNpgsql(dbConfig.BinanceDbConnectionString));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
