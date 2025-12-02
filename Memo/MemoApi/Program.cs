
using Microsoft.OpenApi;

namespace MemoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加服务到容器
            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // 配置Swagger服务
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v2025", new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "MemoAPI",
                    Version = "v2025",
                    Description = "MemoAPI",
                    Contact = new Microsoft.OpenApi.OpenApiContact
                    {
                        Name = "个人开发项目",
                        Email = "xxxxxxx@xxx.com"
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // 配置HTTP请求管道
            if (app.Environment.IsDevelopment())
            {
                // 启用Swagger中间件
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2025/swagger.json", "MemoAPI");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
