using System.Data;
using System.Data.SQLite;
using Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Service.Services;
using Services.Mappers;
using WebAPI.Mappers;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "POSOperations" });
            });

            services.AddSingleton<IDbConnection>(c => new SQLiteConnection(Configuration.GetSection("SQLiteConnectionString").Value));
            services.AddScoped<IPosDbRepository, PosDbRepository>();
            services.AddScoped<IPaymentInfoMapper, PaymentInfoMapper>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAccountBalanceService, AccountBalanceService>();
            services.AddScoped<IPaymentMessageMapper, PaymentMessageMapper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "POSOperations v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
