using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagementSystem.Data;
using OrderManagementSystem.Services;
using OrderManagementSystem.Repositories;
using OrderManagementSystem.Repositories.Repository;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Management API", Version = "v1" });
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Enter JWT token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        // you can also use SQL Server

        /*
        builder.Services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("myconn"), sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure();
            }));*/
        builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

        // Configure JWT authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "a5F7d8K!2zX9q$R@Vm3nP0eWsLu#C1bY"))
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<OrderServices>();
        builder.Services.AddScoped<CustomerRepository>();
        builder.Services.AddScoped<OrderRepository>();
        builder.Services.AddScoped<ProductRepository>();
        builder.Services.AddScoped<UserRepository>();
        builder.Services.AddScoped<InvoiceRepository>();

        builder.Services.AddScoped<OrderServices>();
        builder.Services.AddScoped<AuthService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
