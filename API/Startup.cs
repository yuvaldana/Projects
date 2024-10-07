using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Common.Interfaces;
using UserService;
using DAL.Data;
using DAL.Repositories;
using PubSub.EventHandlers;
using PubSub.Subscriber;
using PubSub.Publisher;

namespace API
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;
            });
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>{
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience= true,
                            ValidateLifetime= true,
                            ValidateIssuerSigningKey= true,
                            ValidIssuer = _config["Jwt:Issuer"],
                            ValidAudience = _config["Jwt:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]))
                        };
                });

            services.AddControllers();
            services.AddTransient<IUserService, UserService.UserService> ();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddDbContext<UserDBContext>(dbContextOptions => dbContextOptions
                .UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddTransient<IOrderService, OrderService.OrderService>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddDbContext<OrderDBContext>(dbContextOptions => dbContextOptions
                .UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddTransient<IProductService, ProductService.ProductService>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddDbContext<ProductDBContext>(dbContextOptions => dbContextOptions
                .UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddHttpContextAccessor();
            services.AddScoped<IPaymentService, PaymentService.PaymentService>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddDbContext<PaymentDBContext>(dbContextOptions => dbContextOptions
                .UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IPublisher>(provider => new RedisPublisher("RedisConnection"));
            services.AddScoped<OrderCreatedHandler>();
            services.AddScoped<ISubscribe>(provider =>
            {
                var redisSubscriber = new RedisSubscriber("your_connection_string");
                var orderCreatedHandler = provider.GetRequiredService<OrderCreatedHandler>();

                redisSubscriber.Subscribe("OrderCreated", orderCreatedHandler.HandleOrderCreated);
                

                return redisSubscriber;
            });
            services.AddScoped<InventoryReservedHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
