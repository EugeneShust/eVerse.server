using Core.Mongo;
using API.Server.Repository;
using Core.Firebase;
using System.Text.Json;
using Internal.Middleware;
using API.Server.Core.Firebase;
using API.Server.Internal.Utils;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using API.Server.Mapping;

namespace api.server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader() 
                          .AllowAnyMethod()
                          .WithExposedHeaders("Location");
                });
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddFirebaseApp(builder.Configuration);
            builder.Services.AddMongoDb(builder.Configuration);
            builder.Services.AddGridFS(builder.Configuration);

            builder.Services.AddRepository<AccountRepository>();
            builder.Services.AddRepository<UserRepository>();

            builder.Services.AddSingleton<JwtSecurityService>();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            var app = builder.Build();
            
            app.UseCors("AllowSpecificOrigins");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); 
                    c.RoutePrefix = string.Empty; 
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<FirebaseAuthenticationMiddleware>();
            app.UseMiddleware<JwtAuthenticationMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}
