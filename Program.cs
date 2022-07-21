
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using pfm.Database;
using pfm.Database.Entities;
using pfm.Database.Repositories;
using pfm.Formatter;
using pfm.Services;

namespace pfm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IRepository<CategoryEntity, string>, CategoryRepository>();
            builder.Services.AddScoped<IRepository<TransactionEntity, string>, TransactionRepository>();
            builder.Services.AddScoped<IRepository<TransactionSplitsEntity, Tuple<string, string>>, SplitsRepository>();

            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddDbContext<PFMDbContext>(options =>
            {
                options.UseNpgsql(GetConnectionString());
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            builder.Services.AddMvc(options =>
            {
                options.InputFormatters.Add(new CsvFormatter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            InitializeDatabase(app);

            app.MapControllers();

            app.Run();
        }

        private static void InitializeDatabase(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.GetService<IServiceScopeFactory>().CreateScope();

                scope.ServiceProvider.GetRequiredService<PFMDbContext>().Database.Migrate();
            }
        }

        private static string GetConnectionString()
        {
            //Not secure purely for demonstrational purposes
            var username = Environment.GetEnvironmentVariable("DATABASE_USERNAME") ?? "postgres";
            var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "123";
            var database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "PFM";
            var host = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "5432";

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = int.Parse(port),
                Database = database,
                Username = username,
                Password = password,
                Pooling = true,
            };

            return builder.ConnectionString;
        }
    }
}